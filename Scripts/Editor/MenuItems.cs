#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RyanQuagliataUnity.Extensions.Editor;
using RyanQuagliataUnity.Scripts;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity.Editor {
	public static class MenuItems {
		/// <summary>
		/// Forces a recompile by adding a temporary new symbol to the PlayerSettings and then removing it
		/// </summary>
		[MenuItem("RyanQuagliata/Force Recompile _F6", priority = 0)]
		public static void ForceRecompile() {
			var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
			var newSymbols = "RECOMPILE;" + symbols;
			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newSymbols);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, symbols);
		}
		
		[MenuItem("RyanQuagliata/Force Update Canvases _F7", priority = 0)]
		public static void ForceUpdateCanvases() => Canvas.ForceUpdateCanvases();

		[MenuItem("RyanQuagliata/Set All UI Dirty _F8", priority = 0)]
		public static void ForceSetAllUIDirty() {
			foreach (var graphic in Object.FindObjectsOfType<Graphic>()) graphic.SetAllDirty();
		}
		
		[MenuItem("RyanQuagliata/Reset Player Prefs", priority = 0)]
		public static void ResetPlayerPrefs() => PlayerPrefs.DeleteAll();

#if !UNITY_2018_3_OR_NEWER
        [MenuItem("RyanQuagliata/Open Player Settings &p", priority = 0)]
        public static void OpenPlayerSettings() {
		    EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
        }

        [MenuItem("RyanQuagliata/Open Build Settings &b", priority = 0)]
        public static void OpenBuildSettings() =>
            EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));

        [MenuItem("RyanQuagliata/Open Time Settings &t", priority = 0)]
        public static void OpenTimeSettings() => EditorApplication.ExecuteMenuItem("Edit/Project Settings/Time");

        [MenuItem("RyanQuagliata/Open Quality Settings &q", priority = 0)]
        public static void OpenQualitySettings() => EditorApplication.ExecuteMenuItem("Edit/Project Settings/Quality");
#else
		[MenuItem("RyanQuagliata/Open Project Settings &p", priority = 0)]
		public static void OpenProjectSettings() => EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
#endif

		/// <summary>
		/// Saves the scene and project settings
		/// </summary>
		[MenuItem("RyanQuagliata/Save All %&s", priority = 0)]
		public static void SaveAll() {
#if UNITY_2018_3_OR_NEWER
			EditorApplication.ExecuteMenuItem("File/Save");
#else
            EditorApplication.ExecuteMenuItem("File/Save Scene");
#endif
			EditorApplication.ExecuteMenuItem("File/Save Project");
		}

		static SerializedObject source;

		/// <summary>
		/// http://answers.unity.com/answers/1538287/view.html
		///
		/// Copies properties from one object to another even if they are of different types
		/// </summary>
		/// <param name="command"></param>
		[MenuItem("CONTEXT/Component/Copy Serialized")]
		public static void CopySerializedFromBase(MenuCommand command) {
			source = new SerializedObject(command.context);
		}

		/// <summary>
		/// http://answers.unity.com/answers/1538287/view.html
		///
		/// Copies properties from one object to another even if they are of different types
		/// </summary>
		/// <param name="command"></param>
		[MenuItem("CONTEXT/Component/Paste Serialized")]
		public static void PasteSerializedFromBase(MenuCommand command) {
			if (source == null) {
				Debug.LogError("No serialized data stored on the clipboard, copy first");
				return;
			}

			EditorUtilityExtensions.CopySerializedPolymorphic(source, new SerializedObject(command.context));
		}

		/// <summary>
		/// Returns false if there are no properties that would match this command.context for copying
		/// </summary>
		/// <param name="command"></param>
		[MenuItem("CONTEXT/Component/Paste Serialized", true)]
		public static bool PasteSerializedFromBaseValid(MenuCommand command) {
			var dest = new SerializedObject(command.context);
			return EditorUtilityExtensions.MatchingPropertyCount(source, dest) > 0;
		}

		[MenuItem("RyanQuagliata/Open Package Manifest", priority = 0)]
		public static void OpenPackageManifest() => Application.OpenURL(Directory.GetParent(Application.dataPath).FullName + "\\Packages\\manifest.json");

		[MenuItem("RyanQuagliata/Open Package Lock", priority = 0)]
		public static void OpenPackageLock() => Application.OpenURL(Directory.GetParent(Application.dataPath).FullName + "\\Packages\\packages-lock.json");

		[MenuItem("RyanQuagliata/Remove Package Locks", priority = 0)]
		public static void RemovePackageLocks() {
			if (!EditorUtility.DisplayDialog("Confirm",
				"Are you sure you want to remove all package locks? Any packages using git will be upgraded to the latest commit", "Yes", "No"))
				return;
			try {
				var path = Directory.GetParent(Application.dataPath).FullName + "\\Packages\\manifest.json";
				var regex = new Regex(" *\"dependencies\": {.*?}", RegexOptions.Singleline);
				var match = regex.Match(File.ReadAllText(path));
				if (match.Success) {
					var output = string.Concat("{\n", match.Value, "\n}");
					File.WriteAllText(path, output);
					AssetDatabase.Refresh();
				} else {
					throw new Exception($"Regex could not find the dependency array in {path}");
				}
			} catch (Exception ex) {
				EditorUtility.DisplayDialog("Error", ex.Message, "ok");
			}
		}

		[MenuItem("RyanQuagliata/Clear Missing Build Scenes")]
		public static void ClearMissingBuildScenes() {
			var oldCount = EditorBuildSettings.scenes.Length;
			var valid = EditorBuildSettings.scenes.Where(x => !x.guid.Empty());
			if (!EditorUtility.DisplayDialog("Confirm", $"Are you sure you want to remove {oldCount - valid.Count()} missing scenes from the build list?",
				"Yes", "No"))
				return;
			EditorBuildSettings.scenes = valid.ToArray();
		}

		[MenuItem("RyanQuagliata/Restart")]
		public static void Restart() {
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorApplication.ExecuteMenuItem("File/Save Project");
			EditorApplication.OpenProject(Environment.CurrentDirectory);
		}

		[MenuItem("RyanQuagliata/Restart With Empty Scene")]
		public static void RestartWithEmptyScene() {
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorApplication.ExecuteMenuItem("File/Save Project");
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
			EditorApplication.OpenProject(Environment.CurrentDirectory);
		}

		/// <summary>
		/// Checks that asmdef files assembly names match their filenames
		/// </summary>
		[MenuItem("RyanQuagliata/Make asmdef filenames match assembly names")]
		public static void FixAssemblyDefinitionNames() {
			string[] search = {
				"Assets",
				"Packages",
			};
			var asmDefs = AssetDatabase.FindAssets($"t:{nameof(AssemblyDefinitionAsset)}", search);
			
			foreach (var asmDefGuid in asmDefs) {
				var asmDefPath = AssetDatabase.GUIDToAssetPath(asmDefGuid);
				var asmDef = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmDefPath);
				
				// Parse JSON
				var asmDefClass = asmDef.Convert();
				
				var filename = Path.GetFileNameWithoutExtension(asmDefPath);
				var assemblyName = asmDefClass.name;
				
				// Skip the obvious Unity ones
				if (filename.StartsWith("com.unity")) continue;
				if (assemblyName.StartsWith("Unity.")) continue;
				
				// If the asm name doesn't match the filename
				if (!string.Equals(filename, assemblyName, StringComparison.Ordinal)) {
					var result = EditorUtility.DisplayDialogComplex("Confirm", $"asmdef filename \"{filename}\" does not match assembly name \"{assemblyName}\", change assembly name?",
						"Yes", "No", "No to all");
					if (result == 1) continue;
					if (result == 2) break;
					asmDefClass.name = filename;
					var absolutePath = Path.GetFullPath(asmDefPath);
					File.WriteAllText(absolutePath, asmDefClass.ToString());
					EditorUtility.SetDirty(asmDef);
				}
			}
			AssetDatabase.Refresh();
		}
	}
}
#endif