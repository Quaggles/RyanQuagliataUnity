//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using RyanQuagliata.Extensions;
//using SharpConfig;
//using Sirenix.OdinInspector;
//using Sirenix.Serialization;
//using Sirenix.Utilities;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//
//namespace RyanQuagliata.PluginExtensions.SharpConfig {
//	public static class Loader {
//		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//		private static void HookSceneLoads() {
//			ScanForMemberAttributes(AssemblyTypeFlags.All);
//			SceneManager.sceneLoaded += (x, y) => Debug.Log($"Scene loaded: {x.path}");
//		}
//
//		public static void SaveAllEditor() {
//			ScanForMemberAttributes(AssemblyTypeFlags.CustomTypes);
//			SaveAll(Directory.GetParent(Application.dataPath).ToString());
//		}
//
//		public static void SaveAll(string directory) {
//			if (!directory.EndsWith("\\")) directory += "\\";
//
//			var configs = new Dictionary<string, Configuration>();
//			foreach (var skeletonSetting in cachedSkeletonSettings) {
//				var config = configs.GetOrNew(skeletonSetting.Config);
//				object value = null;
//
//				// Find a value from a representative object to put in the config file
////				if (!RepresentativeObjects.TryGetValue(skeletonSetting.ParentType, out value)) {
////					Debug.LogWarning($"[{nameof(SharpConfigUnity)}] No representative object found for type \"{skeletonSetting.ParentType.FullName}\", skipping");
////					continue;
////				}
//				config[skeletonSetting.Section][skeletonSetting.Setting]
//					.SetValue(value == null ? null : skeletonSetting.MemberInfo.GetValue(value));
//			}
//
//			foreach (var keyValuePair in configs) {
//				if (keyValuePair.Value.SectionCount == 0) continue;
//				Debug.Log(
//					$"[Exporting config {directory + keyValuePair.Key} containing {keyValuePair.Value.SectionCount} sections, {keyValuePair.Value.GetEnumerator().ToList().Sum(x => x.SettingCount)} settings");
//				keyValuePair.Value.SaveToFile(directory + keyValuePair.Key + '.' + CONFIG_FILE_EXTENSION);
//			}
//		}
//
//		public static void LoadAllEditor() {
//			ScanForMemberAttributes(AssemblyTypeFlags.CustomTypes);
//			LoadAll(Directory.GetParent(Application.dataPath).ToString());
//		}
//		
//		public static void LoadAll(string directory) {
//			int configCount = 0, sectionCount = 0, settingCount = 0;
//			foreach (var file in Directory.EnumerateFiles(directory)) {
//				if (!string.Equals(Path.GetExtension(file).TrimStart('.'), CONFIG_FILE_EXTENSION, StringComparison.CurrentCultureIgnoreCase)) continue;
//				configCount++;
//				var config = Configuration.LoadFromFile(file);
//				foreach (var section in config) {
//					sectionCount++;
//					foreach (var setting in section) {
//						settingCount++;
//						Debug.Log($"Found{setting.ToString()}");
//					}
//				}
//			}
//			Debug.Log($"Processed {configCount} configs");
//			Debug.Log($"Processed {sectionCount} sections");
//			Debug.Log($"Processed {settingCount} settings");
//		}
//
//		private const string DEFAULT_CONFIG_FILE_NAME = "Config";
//		private const string CONFIG_FILE_EXTENSION = "cfg";
//
//		static string GetProperConfigurationName(string original) =>
//			string.IsNullOrWhiteSpace(original) ? DEFAULT_CONFIG_FILE_NAME : original;
//
//		static string GetProperSectionName(string original, Type parentType) =>
//			string.IsNullOrWhiteSpace(original) ? parentType.FullName : original;
//
//		static string GetProperSettingName(string original, MemberInfo memberInfo) =>
//			string.IsNullOrWhiteSpace(original) ? memberInfo.Name : original;
//
//		[ShowInInspector]
//		private static readonly List<SkeletonSetting> cachedSkeletonSettings = new List<SkeletonSetting>();
//
//		struct SkeletonSetting {
//			public Type ParentType;
//			public MemberInfo MemberInfo;
//			public string Config;
//			public string Section;
//			public string Setting;
//		}
//
//		[Button]
//		static IEnumerable<SkeletonSetting> ScanForMemberAttributes(AssemblyTypeFlags assemblyTypeFlags = AssemblyTypeFlags.CustomTypes) {
//			var startTime = DateTime.Now;
//			int typeCount = 0, memberCount = 0;
//			foreach (var type in AssemblyUtilities.GetTypes(assemblyTypeFlags)) {
//				typeCount++;
//				foreach (var memberInfo in type.GetMembers()) {
//					memberCount++;
//					StoreInConfigFileAttribute attribute = null;
//					try {
//						attribute = memberInfo.GetCustomAttributes<StoreInConfigFileAttribute>(true).FirstOrDefault();
//					} catch (Exception ex) {
//						Debug.LogWarning(
//							$"Error fetching attributes for {type.FullName} {memberInfo.Name}: {ex.Message}");
//					}
//
//					if (attribute == null) continue;
//
//					var configName = GetProperConfigurationName(attribute.ConfigFileName);
//					var sectionName = GetProperSectionName(attribute.SectionName, type);
//					var settingName = GetProperSettingName(attribute.SettingName, memberInfo);
//					var skeletonSetting = new SkeletonSetting {
//						MemberInfo = memberInfo,
//						ParentType = type,
//						Config = configName,
//						Section = sectionName,
//						Setting = settingName
//					};
//					if (!cachedSkeletonSettings.Contains(skeletonSetting)) cachedSkeletonSettings.Add(skeletonSetting);
//					Debug.Log(
//						$"Found attribute {attribute}, {nameof(configName)}[{configName}], {nameof(sectionName)}[{sectionName}], {nameof(settingName)}[{settingName}]");
//				}
//			}
//
//			Debug.Log($"Processing took {DateTime.Now - startTime}");
//			Debug.Log($"Processed {typeCount} types");
//			Debug.Log($"Processed {memberCount} members");
//			return cachedSkeletonSettings;
//		}
//	}
//}