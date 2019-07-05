#if false
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using RyanQuagliata.Extensions;
using RyanQuagliata.PluginExtensions.OdinInspector;
using SharpConfig;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
#endif

namespace RyanQuagliata.PluginExtensions.SharpConfig {
	// ReSharper disable once ClassNeverInstantiated.Global
	// Needs to not be static to allow OnPreprocessBuild function to work
	public class SharpConfigUnity
#if UNITY_EDITOR
		: UnityEditor.Build.IPreprocessBuildWithReport
#endif
	{
		public const string DEFAULT_CONFIG_FILE_NAME = "Config";
		public const string DEFAULT_CONFIG_FILE_EXTENSION = "ini";
#if UNITY_EDITOR
		public const bool DEBUG_MESSAGES = false;


		[Button("Save All")]
		public static void SaveAllButton() {
			SaveConfigFiles(GenerateConfigFiles().Select(x => (x.Key, x.Value)), Directory.GetParent(Application.dataPath).ToString());
		}

		[Button("Load All")]
		public static void LoadAllButton() {
			//LoadAll(Directory.GetParent(Application.dataPath).ToString());
		}

		public int callbackOrder => -1;


		/// <summary>
		/// When a build is made export the config files
		/// </summary>
		/// <param name="report"></param>
	    public void OnPreprocessBuild(BuildReport report) {
	        try {
	            var buildDirectory = Directory.GetParent(report.summary.outputPath).ToString() + '\\';
	            Debug.Log($"[{nameof(SharpConfigUnity)}] {report.summary.platform.ToString()} build triggered at {report.summary.outputPath}, exporting config files to {buildDirectory}...");
	            SaveConfigFiles(GenerateConfigFiles().Select(x => (x.Key, x.Value)), Directory.GetParent(Application.dataPath).ToString()); 
	            Debug.Log($"[{nameof(SharpConfigUnity)}] Exporting of config files successful");
	        } catch (Exception ex){
	            Debug.LogWarning($"[{nameof(SharpConfigUnity)}] Error during config export: {ex.Message}");
	            throw;
	        }
	    }

#endif

		[ShowInInspector]
		private static HashSet<Member2ConfigMap> cachedMember2ConfigMap = null;

		public static HashSet<Member2ConfigMap> CachedMember2ConfigMap {
			get {
				if (cachedMember2ConfigMap != null) return cachedMember2ConfigMap;
				cachedMember2ConfigMap = new HashSet<Member2ConfigMap>();
				return GetMember2ConfigMap(cachedMember2ConfigMap);
			}
		}

		private static IEnumerable<(string, Configuration)> cachedConfigFiles = null;

		public static IEnumerable<(string, Configuration)> CachedConfigFiles {
			get {
				if (cachedConfigFiles == null) cachedConfigFiles = LoadConfigFiles(Directory.GetParent(Application.dataPath).ToString());
				return cachedConfigFiles;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void HookSceneLoad() {
			SceneManager.sceneLoaded += SceneLoaded;
		}

		/// <summary>
		/// When a scene is loaded fill all gameobjects with the config values
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="mode"></param>
		private static void SceneLoaded(Scene scene, LoadSceneMode mode) => FillConfigValues(scene.GetRootGameObjects());

		/// <summary>
		/// Searches through the loaded config files and finds the value mapped from the member info
		/// </summary>
		/// <param name="member2ConfigMap"></param>
		/// <returns></returns>
		static object GetConfigValue(Member2ConfigMap member2ConfigMap) {
			var configFile = CachedConfigFiles.First(tuple => tuple.Item1 == member2ConfigMap.Config);
			return configFile.Item2[member2ConfigMap.Section][member2ConfigMap.Setting].GetValue(member2ConfigMap.MemberInfo.GetUnderlyingType());
		}

		static void FillConfigValue(object @object) {
			foreach (var map in CachedMember2ConfigMap) {
				if (@object.GetType() != map.MemberInfo.DeclaringType) return;
				var value = GetConfigValue(map);
				map.MemberInfo.SetValue(@object, value);
			}
		}

		static void FillConfigValues(IEnumerable<object> objects) {
			foreach (var o in objects) FillConfigValue(o);
		}

		[Button]
		public static HashSet<Member2ConfigMap> RefreshCachedMember2ConfigMap() {
			cachedMember2ConfigMap.Clear();
			return GetMember2ConfigMap(null, cachedMember2ConfigMap);
		}

		[Button]
		public static void PrintSkeletonValues() {
			var startTime = DateTime.Now;
			var skeleton = GetMember2ConfigMap();
			foreach (var skeletonSetting in skeleton) {
				Debug.Log($"{skeletonSetting.ToString()}");
			}
			Debug.Log($"{skeleton.Count} Skeleton Settings took {DateTime.Now - startTime}");
		}

		public static Dictionary<Member2ConfigMap, object> ConfigDefaultValues => SharpConfigUnityDefaultValues.Instance.ConfigDefaultValues;

		/// <summary>
		/// Generates configuration representations of the CachedMember2ConfigMap grabbing default values from the representative objects
		/// </summary>
		/// <returns></returns>
		public static IDictionary<string, Configuration> GenerateConfigFiles() {
			var configs = new Dictionary<string, Configuration>();
			foreach (var skeletonSetting in CachedMember2ConfigMap) {
				var config = configs.GetOrNew(skeletonSetting.Config);

				// Add the comment if it was provided
				if (!string.IsNullOrWhiteSpace(skeletonSetting.Comment)) config[skeletonSetting.Section][skeletonSetting.Setting].PreComment = skeletonSetting.Comment;
				
				if (!ConfigDefaultValues.TryGetValue(skeletonSetting, out var value)) {
					Debug.LogWarning($"[{nameof(SharpConfigUnity)}] No representative object found for member \"{skeletonSetting.MemberInfo.GetNiceName()}\", leaving it in but commenting out");
					config[skeletonSetting.Section][skeletonSetting.Setting].SetValue(null);
					continue;
				}
				config[skeletonSetting.Section][skeletonSetting.Setting].SetValue(skeletonSetting.MemberInfo.GetValue(value));
			}

			return configs;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="configurations">List of config file names and configuration assets</param>
		/// <param name="directory"></param>
		public static void SaveConfigFiles(IEnumerable<(string, Configuration)> configurations, [NotNull] string directory) {
			if (directory == null) throw new ArgumentNullException(nameof(directory));
			if (!directory.EndsWith("\\")) directory += "\\";
			if (!Directory.Exists(directory)) throw new DirectoryNotFoundException();

			foreach (var (configName, configuration) in configurations) {
				if (configuration.SectionCount == 0) continue;
				configuration.SaveToFile($"{directory}{configName}");
			}
		}

		public static IEnumerable<(string, Configuration)> LoadConfigFiles([NotNull] string directory) {
			if (directory == null) throw new ArgumentNullException(nameof(directory));
			if (!directory.EndsWith("\\")) directory += "\\";
			if (!Directory.Exists(directory)) throw new DirectoryNotFoundException();
			
			var configs = new List<(string, Configuration)>();
			foreach (var file in Directory.EnumerateFiles(directory, $"*.{DEFAULT_CONFIG_FILE_EXTENSION}", SearchOption.TopDirectoryOnly)) {
				configs.Add(
					(Path.GetFileName(file), Configuration.LoadFromFile(file))
				);
			}

			return configs;
		}

		/// <summary>
		/// A struct that stores the mapping between members and config entries
		/// </summary>
		[Serializable]
		public struct Member2ConfigMap {
			public MemberInfo MemberInfo;
			/// <summary>
			/// Name of the config file
			/// </summary>
			public string Config;
			/// <summary>
			/// Name of the section within the config file
			/// </summary>
			public string Section;
			/// <summary>
			/// Name of the setting within the config file
			/// </summary>
			public string Setting;
			/// <summary>
			/// Optional comment above the config file
			/// </summary>
			public string Comment;
#if UNITY_EDITOR
			public override string ToString() => $"<b>Config:</b> [{Config}][{Section}][{Setting}] <b>C# Field:</b> {MemberInfo.DeclaringType}.{MemberInfo.GetNiceName()}";
#else
			public override string ToString() => $"Config: [{Config}][{Section}][{Setting}] C# Field: {ParentType.Name}.{MemberInfo.GetNiceName()}";
#endif
		}
		
		/// <summary>
		/// Gets a map of all the members to the config file entries
		/// </summary>
		/// <param name="types">Set of types to scan, if null all types will be scanned</param>
		/// <param name="hashset">Hashset to fill with results</param>
		/// <returns>Hashset of results</returns>
		private static HashSet<Member2ConfigMap> GetMember2ConfigMap(IEnumerable<Type> types, HashSet<Member2ConfigMap> hashset = null) {
			if (types == null) types = AssemblyUtilities.GetTypes(AssemblyTypeFlags.All);

			if (hashset == null) hashset = new HashSet<Member2ConfigMap>();
			foreach (var parentType in types) {
				foreach (var memberInfo in parentType.GetMembers()) {
					if (TryGetMember2ConfigMap(memberInfo, out var member2ConfigMap)) {
						hashset.Add(member2ConfigMap);
					}
				}
			}

			return hashset;
		}

		/// <summary>
		/// Gets a map of all the members to the config file entries
		/// </summary>
		/// <param name="hashset">Hashset to fill with results</param>
		/// <returns>Hashset of results</returns>
		private static HashSet<Member2ConfigMap> GetMember2ConfigMap(HashSet<Member2ConfigMap> hashset = null) => GetMember2ConfigMap(AssemblyUtilities.GetTypes(AssemblyTypeFlags.All), hashset);

		public static bool TryGetMember2ConfigMap(MemberInfo memberInfo, out Member2ConfigMap member2ConfigMap) {
			var parentType = memberInfo.DeclaringType;
			ConfigurableAttribute configurableAttribute = null;
			try {
				configurableAttribute = memberInfo.GetCustomAttributes<ConfigurableAttribute>(true).FirstOrDefault();
			} catch (Exception ex) {
				Debug.LogWarning($"Error fetching attributes for {parentType.FullName} {memberInfo.Name}: {ex.Message}");
			}

			if (configurableAttribute == null) {
				member2ConfigMap = default;
				return false;
			}

			member2ConfigMap = new Member2ConfigMap {
				MemberInfo = memberInfo,
				Config = GetProperConfigurationName(configurableAttribute.ConfigFileName),
				Section = GetProperSectionName(configurableAttribute.SectionName, parentType),
				Setting = GetProperSettingName(configurableAttribute.SettingName, memberInfo),
				Comment = configurableAttribute.Comment,
			};

			// If the comment field was null try to get the tooltip attribute
			if (string.IsNullOrWhiteSpace(member2ConfigMap.Comment)) {
				TooltipAttribute tooltipAttribute = null;
				try {
					tooltipAttribute = memberInfo.GetCustomAttributes<TooltipAttribute>(true).FirstOrDefault();
				} catch (Exception ex) {
					Debug.LogWarning($"Error fetching attributes for {parentType.FullName} {memberInfo.Name}: {ex.Message}");
				}

				member2ConfigMap.Comment = tooltipAttribute?.tooltip;
			}
			
			return true;
		}

		public static string GetProperConfigurationName(string original) =>
			string.IsNullOrWhiteSpace(original) ? $"{DEFAULT_CONFIG_FILE_NAME}.{DEFAULT_CONFIG_FILE_EXTENSION}" : original;

		public static string GetProperSectionName(string original, Type parentType) =>
			string.IsNullOrWhiteSpace(original) ? parentType.FullName : original;

		public static string GetProperSettingName(string original, MemberInfo memberInfo) =>
			string.IsNullOrWhiteSpace(original) ? memberInfo.Name : original;
	}
	
	public class SharpConfigUnityDefaultValues : SerializedGlobalConfig<SharpConfigUnityDefaultValues> {
		[OdinSerialize]
		public Dictionary<SharpConfigUnity.Member2ConfigMap, object> ConfigDefaultValues = new Dictionary<SharpConfigUnity.Member2ConfigMap, object>();
	}
}
#endif