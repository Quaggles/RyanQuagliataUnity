using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QFSW.QC;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

namespace RyanQuagliataUnity.Extensions.OdinInspector {
	[GlobalConfig(ASSET_FOLDER)]
	[CommandPrefix("Asset")]
	public class StandaloneAssetDatabaseV2 : SerializedGlobalConfig<StandaloneAssetDatabaseV2>
#if UNITY_EDITOR
		, IPreprocessBuildWithReport, IPostprocessBuildWithReport
#endif
	{
		[ShowInInspector]
		public Dictionary<Type, IList> Assets = new Dictionary<Type, IList>();

		[ShowInInspector, ListDrawerSettings(Expanded = true, HideAddButton = true)]
		public List<Type> SaveTypes = new List<Type>();

		[Command]
		public static void ListAll() {
			foreach (var pair in Instance.Assets) {
				Debug.Log($"({pair.Value.Count}) {pair.Key.FullName}");
				for (var i = 0; i < pair.Value.Count; i++) {
					var o = pair.Value[i];
					Debug.Log($"    [{i}] {o}");
				}
			}
		}

		[Command]
		public static List<T> GetAll<T>(string searchTerm = "", SearchType searchType = default, bool ignoreCase = true) where T : Object {
			if (string.IsNullOrEmpty(searchTerm)) {
				if (Instance.Assets.TryGetValue(typeof(T), out var list)) return (List<T>) list;
			} else {
				if (Instance.Assets.TryGetValue(typeof(T), out var list))
					return ((List<T>) list).Where(x => x.name.Filter(searchTerm, searchType, ignoreCase)).ToList();
			}
			return null;
		}

		[Command]
		public static T GetByGuid<T>(string guid) where T : Object, IGuid => TryGetByGuid<T>(guid, out var value) ? value : default;

		public static bool TryGetByGuid<T>(string guid, out T value) where T : Object, IGuid {
			var objects = GetAll<T>();
			for (var i = 0; i < objects.Count; i++) {
				var item = objects[i];
				if (item.Guid.Equals(guid, StringComparison.Ordinal)) {
					value = item;
					return true;
				}
			}

			value = default;
			return false;
		}
		
		[Command]
		public static void List<T>(string searchTerm = "", SearchType searchType = default, bool ignoreCase = true) where T : Object {
			var items = GetAll<T>(searchTerm, searchType, ignoreCase);
			Debug.Log(StringExtensions.FilterToString(items?.Count ?? 0, searchTerm, searchType, ignoreCase));
			if (items == null) return;
			for (var i = 0; i < items.Count; i++) {
				var item = items[i];
				if (item is IGuid iguid)
					Debug.Log($"[{i}] {iguid.Guid} {item.name}");
				else
					Debug.Log($"[{i}] {item.name}");
			}
		}
		
		[Command]
		public static T Get<T>(string searchTerm = "", SearchType searchType = default, bool ignoreCase = true) where T : Object {
			foreach (var item in GetAll<T>())
				if (item.name.Filter(searchTerm, searchType, ignoreCase))
					return item;
			Debug.LogError($"No {typeof(T).Name} with name \"{searchTerm}\" in search mode \"{searchType.ToString()}\" could be found");
			return null;
		}
		
#if UNITY_EDITOR
		public int callbackOrder { get; }
		public void OnPreprocessBuild(BuildReport report) => StoreAllTypes();
		public void OnPostprocessBuild(BuildReport report) => Clear();
		
		[Button]
		public static void AddSavedType(Type type) {
			var saveTypes = Instance.SaveTypes;
			if (saveTypes.Contains(type)) return;
			saveTypes.Add(type);
		}

		public static IEnumerable<Type> GuidTypes => TypeCache.GetTypesDerivedFrom<IGuid>().Where(x => x.IsClass && !x.IsAbstract);
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Initialize() {
			// If in the editor play mode then store the objects and write GUIDs for usage 
			StoreAllTypes();
			// If exiting play mode clear
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}
		
		private static void OnPlayModeStateChanged(PlayModeStateChange state) {
			if (state != PlayModeStateChange.ExitingPlayMode) return;
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
			Clear();
		}
		
		[Command, Button]
		public static void StoreAllTypes() {
			Instance.Assets.Clear();
			foreach (var type in Instance.SaveTypes) Store(type);
			foreach (var type in GuidTypes) Store(type);
		}

		public static void Store(Type type) {
			// Create list of correct type, store as IList
			if (!Instance.Assets.ContainsKey(type)) {
				var constructedListType = typeof(List<>).MakeGenericType(type);
				Instance.Assets.Add(type, (IList)Activator.CreateInstance(constructedListType));
			}

			if (Instance.Assets.TryGetValue(type, out var list)) {
				foreach (var instance in GetAssetInstances(type)) {
					if (instance is IGuid guid) guid.Guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(instance));
					list.Add(instance);
				}
			}
		}

		[Command, Button]
		public static void Clear() {
			Instance.Assets.Clear();
			foreach (var type in GuidTypes)
				foreach (IGuid instance in GetAssetInstances(type))
					instance.Guid = "";
		}
		
		public static Object[] GetAssetInstances(Type type) {
			string[] guids = AssetDatabase.FindAssets("t:" + type.Name); //FindAssets uses tags check documentation for more info
			var results = new Object[guids.Length];
			for (int i = 0; i < guids.Length; i++) results[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), type);
			return results;
		}

		public static T[] GetAssetInstances<T>() where T : Object {
			string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name); //FindAssets uses tags check documentation for more info
			var results = new T[guids.Length];
			for (int i = 0; i < guids.Length; i++) results[i] = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[i]));
			return results;
		}
#endif
	}
}