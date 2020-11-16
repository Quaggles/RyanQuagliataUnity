#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using Debug = UnityEngine.Debug;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace RyanQuagliataUnity.Editor {
	public class GitPackageUpdater : OdinEditorWindow {
		public static readonly Regex PlainGitUrlRegex = new Regex(@"[a-z]+:\/\/.+.git", RegexOptions.Compiled);
		public static readonly Regex GetLastCommitSha1Regex = new Regex(@"^[a-z0-9]+", RegexOptions.Compiled);

		public const string GIT_EXECUTABLE_NAME = "git.exe";
		public static bool WindowIsOpen => GetWindow<GitPackageUpdater>() != null;

		public static Task CurrentTask;

		[ShowInInspector]
		public static bool CheckingForUpdate => !CurrentTask?.IsCompleted ?? false;
		
		[ShowInInspector, GUIColor(1f, 0.2f, 0.2f), ShowIf(nameof(HasError)), MultiLineProperty(Lines = 20)]
		public static string CurrentError => CurrentTask?.Exception?.ToString() ?? "None";

		public static bool HasError => CurrentTask?.Exception != null;

		public static DateTime LastTimeFetched;

		[ShowInInspector, DisplayAsString, HideIf(nameof(CheckingForUpdate))]
		public static string TimeSinceRefresh => $"{(DateTime.Now - LastTimeFetched).ToString(@"hh\:mm\:ss")}s ago";

		[ShowInInspector, DisplayAsString, HideIf(nameof(CheckingForUpdate))]
		public static string LastRefreshTime => $"{LastTimeFetched:f}";

		[HideIf(nameof(CheckingForUpdate)), NonSerialized, ShowInInspector, ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, Expanded = true),
		 PropertyOrder(2000)]
		public static List<Package> GitPackages = new List<Package>();

		public static bool AnyUpdates => GitPackages.Any(x => x.HasUpdate);

		[InitializeOnLoadMethod]
		static void StartUpdateCheckLoop() {
			// Disabled Background update check for now
			// CheckForUpdatesLoopAsync().Forget(Debug.LogError);
		}

		static async Task CheckForUpdatesLoopAsync() {
			while (WindowIsOpen) {
				if (!CheckingForUpdate) {
					Debug.Log("Running update check");
					await RefreshPackageListAsync();
				}

				await Task.Delay(TimeSpan.FromMinutes(1));
				if (Application.isPlaying) {
					while (Application.isPlaying) {
						await Task.Delay(TimeSpan.FromSeconds(10));
					}

					Debug.Log("Exited play mode, can check again");
				}
			}
		}

		[MenuItem("RyanQuagliata/Git Package Updater")]
		private static void OpenWindow() => GetWindow<GitPackageUpdater>().Show();

		protected override void OnEnable() {
			base.OnEnable();
			RefreshPackageList();
		}

		public struct Package {
			[HideInInspector]
			public PackageInfo PackageInfo;

			[ShowInInspector, DisplayAsString, PropertyOrder(0)]
			public string Name => PackageInfo.displayName;

			[ShowInInspector, DisplayAsString, PropertyOrder(1)]
			public string GitUrl => PackageInfo.packageId.Split('@')[1];

			[ReadOnly, PropertyOrder(Int32.MaxValue), DisplayAsString]
			public bool HasUpdate;

			[ShowIf(nameof(HasUpdate)), Button, PropertyOrder(3)]
			public void UpdatePackage() => CurrentTask = UpdatePackageAsync(PackageInfo);
		}

		[Button, ShowIf(nameof(AnyUpdates))]
		public static void UpdateAllPackages() => CurrentTask = UpdateAllPackagesAsync();

		public static async Task UpdateAllPackagesAsync() {
			foreach (var package in GitPackages) {
				if (package.HasUpdate) await UpdatePackageAsync(package.PackageInfo, false);
			}
			Client.Resolve();
		}

		public static async Task UpdatePackageAsync(PackageInfo packageInfo, bool resolveOnFinish = true) {
			var add = Client.Add(packageInfo.packageId.Split('@')[1]);
			while (!add.IsCompleted) {
				EditorUtility.DisplayProgressBar($"Updating {packageInfo.name}", $"Updating {packageInfo.name}", 0.5f);
				// Wait
				await Task.Delay(1000 / 60);
			}

			EditorUtility.ClearProgressBar();
			if (resolveOnFinish) Client.Resolve();
		}

		[Button, DisableIf(nameof(CheckingForUpdate))]
		public static void RefreshPackageList() {
			if (!CheckingForUpdate) {
				CurrentTask = RefreshPackageListAsync();
			}
		}

		public static async Task RefreshPackageListAsync() {
			GitPackages.Clear();
			var listPackages = Client.List();
			while (!listPackages.IsCompleted) {
				// Wait
				await Task.Delay(1000 / 60);
			}

			foreach (var packageInfo in listPackages.Result) {
				if (packageInfo.source != PackageSource.Git) continue;
				var package = new Package {PackageInfo = packageInfo};
				// Can't use ref struct with async :(
				package = await RefreshPackageAsync(package);
				GitPackages.Add(package);
			}

			LastTimeFetched = DateTime.Now;
		}

		public static async Task<Package> RefreshPackageAsync(Package package) {
			var sha1 = await GetLastCommitSha1Async(PlainGitUrlRegex.Match(package.GitUrl).Value, package.PackageInfo.git.revision);
			package.HasUpdate = !string.Equals(sha1, package.PackageInfo.git.hash);
			return package;
		}

		public static async Task<string> GetLastCommitSha1Async(string remote, string reference) {
			var startInfo = new ProcessStartInfo {
				FileName = GIT_EXECUTABLE_NAME,
				Arguments = $"ls-remote {remote} {reference}",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true,
				UseShellExecute = false,
			};
			var process = Process.Start(startInfo);
			process.Start();
			while (!process.HasExited) await Task.Delay(1000 / 60);
			process.WaitForExit();
			var stdError = await process.StandardError.ReadToEndAsync();
			if (!string.IsNullOrWhiteSpace(stdError)) throw new InvalidOperationException(stdError);
			return GetLastCommitSha1Regex.Match(await process.StandardOutput.ReadToEndAsync()).Value;
		}
	}
}
#endif