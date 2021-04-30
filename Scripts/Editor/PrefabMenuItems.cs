using UnityEditor;
using UnityEngine;

namespace RyanQuagliataUnity.Editor {
	public static class PrefabMenuItems {
		public static bool ValidPrefab(Object obj) {
			if (!obj) return false;
			return obj switch {
				Component component => PrefabUtility.IsPartOfPrefabInstance(component),
				GameObject gameObject => PrefabUtility.IsAnyPrefabInstanceRoot(gameObject) || PrefabUtility.IsPartOfPrefabInstance(gameObject),
				_ => false
			};
		}

		private static void RevertObject(Object obj) {
			if (!obj) return;
			if (obj is Component component) {
				PrefabUtility.RevertObjectOverride(component, InteractionMode.UserAction);
			} else if (obj is GameObject gameObject) {
				PrefabUtility.RevertObjectOverride(gameObject, InteractionMode.UserAction);
				foreach (var child in gameObject.GetComponents<Component>()) RevertObject(child);
			}
		}
		
		private static void RevertObjectChildren(Object obj) {
			if (!obj) return;
			if (obj is Component component) {
				RevertObject(component);
				PrefabUtility.RevertObjectOverride(component, InteractionMode.UserAction);
				var type = component.GetType();
				foreach (var child in component.GetComponentsInChildren(type, true)) RevertObject(child);
			} else if (obj is GameObject gameObject) {
				RevertObject(gameObject);
				foreach (Transform child in gameObject.transform) RevertObject(child);
			}
		}
		
		private static void SelectSourcePrefab(GameObject gameObject) {
			if (!gameObject) return;
			SerializedObject serializedObject = new SerializedObject(PrefabUtility.GetPrefabInstanceHandle(gameObject));
			SerializedProperty serializedProperty = serializedObject.FindProperty("m_SourcePrefab");
			var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GetAssetPath(serializedProperty.objectReferenceValue));
			if (!asset) return;
			EditorUtility.FocusProjectWindow();
			EditorGUIUtility.PingObject(asset);
		}

		private const int PRIORITY = 49;

		private const string REVERT_COMPONENT_OVERRIDES_NAME = "CONTEXT/Component/Revert Overrides";
		[MenuItem(REVERT_COMPONENT_OVERRIDES_NAME, false, PRIORITY)]
		public static void ResetMonoBehaviourOverrides(MenuCommand menuCommand) => RevertObject(menuCommand.context);
		[MenuItem(REVERT_COMPONENT_OVERRIDES_NAME, true, PRIORITY)]
		public static bool ResetMonoBehaviourOverridesValidate(MenuCommand menuCommand) => ValidPrefab(menuCommand.context);

		private const string REVERT_COMPONENT_CHILDREN_OVERRIDES_NAME = "CONTEXT/Component/Revert Child Overrides";
		[MenuItem(REVERT_COMPONENT_CHILDREN_OVERRIDES_NAME, false, PRIORITY)]
		public static void ResetOverrides(MenuCommand menuCommand) => RevertObjectChildren(menuCommand.context);
		[MenuItem(REVERT_COMPONENT_CHILDREN_OVERRIDES_NAME, true, PRIORITY)]
		public static bool ResetOverridesValidate(MenuCommand menuCommand) => ValidPrefab(menuCommand.context);

		private const string RESET_GAME_OBJECTS_NAME = "GameObject/Prefab/Revert Overrides";
		[MenuItem(RESET_GAME_OBJECTS_NAME, false, PRIORITY)]
		public static void ResetGameObjectOverrides(MenuCommand menuCommand) => RevertObject(menuCommand.context);
		[MenuItem(RESET_GAME_OBJECTS_NAME, true, PRIORITY)]
		public static bool ResetGameObjectOverridesValidate() => ValidPrefab(Selection.activeGameObject);
		
		private const string RESET_CHILD_OVERRIDES_NAME = "GameObject/Prefab/Revert Child Overrides";
		[MenuItem(RESET_CHILD_OVERRIDES_NAME, false, PRIORITY)]
		public static void ResetChildOverrides(MenuCommand menuCommand) => RevertObjectChildren(menuCommand.context);
		[MenuItem(RESET_CHILD_OVERRIDES_NAME, true, PRIORITY)]
		public static bool ResetChildOverridesValidate() => ValidPrefab(Selection.activeGameObject);
		
		private const string SELECT_PREFAB_BASE = "GameObject/Prefab/Select Prefab Base";
		private const string SELECT_PREFAB_BASE_ASSETS = "Assets/Prefab/Select Prefab Base";
		[MenuItem(SELECT_PREFAB_BASE, false, PRIORITY)]
		[MenuItem(SELECT_PREFAB_BASE_ASSETS, false, PRIORITY)]
		public static void SelectPrefabBase() => SelectSourcePrefab(Selection.activeGameObject);
		[MenuItem(SELECT_PREFAB_BASE, true, PRIORITY)]
		[MenuItem(SELECT_PREFAB_BASE_ASSETS, true, PRIORITY)]
		public static bool SelectPrefabBaseValidate() => ValidPrefab(Selection.activeGameObject);
	}
}