using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR

#endif

namespace RyanQuagliataUnity.Extensions {
	public static class SceneManagerExtensions {
		/// <summary>
		/// Returns if a scene is loaded by comparing scene names
		/// </summary>
		/// <param name="sceneName">Name of the scene</param>
		/// <returns></returns>
		public static bool IsSceneLoaded(string sceneName) {
			for (int i = 0; i < SceneManager.sceneCount; i++) {
				var scene = SceneManager.GetSceneAt(i);
				if (sceneName == scene.name) return true;
			}
			return false;
		}
		
		/// <summary>
		/// Returns if a scene is loaded by comparing scene paths
		/// </summary>
		/// <param name="scenePath">Path of the scene</param>
		/// <returns></returns>
		public static bool IsSceneLoadedByPath(string scenePath) {
			for (int i = 0; i < SceneManager.sceneCount; i++) {
				var scene = SceneManager.GetSceneAt(i);
				if (scenePath == scene.path) return true;
			}
			return false;
		}
		
		public static bool IsSceneLoadedByBuildIndex(int buildIndex) {
			var refScene = SceneManager.GetSceneByBuildIndex(buildIndex);
			for (int i = 0; i < SceneManager.sceneCount; i++) {
				var scene = SceneManager.GetSceneAt(i);
				if (refScene.path == scene.path) return true;
			}
			return false;
		}
		
		/// <summary>
		/// Returns if a scene is loaded by comparing scene paths
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public static bool IsSceneLoaded(Scene scene) {
			for (int i = 0; i < SceneManager.sceneCount; i++) {
				var sceneIterator = SceneManager.GetSceneAt(i);
				if (sceneIterator.path == scene.path) return true;
			}
			return false;
		}
		
		/// <summary>
		/// Returns the first loaded scene that contains the supplied gameObject, useful for finding the parent scene an object exists within
		/// </summary>
		/// <param name="gameObject"></param>
		/// <returns>The scene containing the gameobject, otherwise returns an invalid scene</returns>
		public static Scene GetLoadedSceneContainingGameObject(GameObject gameObject) {
			for (int i = 0; i < SceneManager.sceneCount; i++) {
				var scene = SceneManager.GetSceneAt(i);
				var rootGameObjects = scene.GetRootGameObjects();
				
				// If the supplied gameobject is a child of any root object it exists within the scene
				if (rootGameObjects.Any(x => gameObject.transform.IsChildOf(x.transform))) return scene;
			}
			return new Scene(); // Return null invalid scene
		}
		
		/// <summary>
		/// Returns true if the current active scene contains the gameobject
		/// </summary>
		/// <param name="gameObject"></param>
		/// <returns></returns>
		public static bool DoesActiveSceneContainGameObject(GameObject gameObject) {
			var activeScene = SceneManager.GetActiveScene();
			foreach (var go in activeScene.GetRootGameObjects()) if (gameObject == go) return true;
			return false;
		}
	}

#if UNITY_EDITOR
	public static class SceneManagerEditorExtensions {
		public static bool IsSceneInBuildList(SceneAsset sceneAsset) {
			return IsSceneInBuildList(AssetDatabase.GetAssetPath(sceneAsset));
		}

		public static bool IsSceneInBuildList(string scenepath) {
			return EditorBuildSettings.scenes.Any(x => x.path == scenepath);
		}
		
		/// <summary>
		/// Adds the scene to the project build settings
		/// </summary>
		/// <param name="sceneAsset">Scene Asset reference</param>
		public static void AddSceneToBuildSettings(SceneAsset sceneAsset) {
			AddSceneToBuildSettings(AssetDatabase.GetAssetPath(sceneAsset));
		}
		
		/// <summary>
		/// Adds the scene to the project build settings
		/// </summary>
		/// <param name="scenePath">Path to the scene</param>
		public static void AddSceneToBuildSettings(string scenePath) {
			if (IsSceneInBuildList(scenePath)) return; // If already in the build list stop
			var original = EditorBuildSettings.scenes;
			var newSettings = new EditorBuildSettingsScene[original.Length + 1];
			System.Array.Copy(original, newSettings, original.Length);
			var sceneToAdd = new EditorBuildSettingsScene(scenePath, true);
			newSettings[newSettings.Length - 1] = sceneToAdd;
			EditorBuildSettings.scenes = newSettings;
		}

		public static void RemoveSceneFromBuildSettings(SceneAsset sceneAsset) {
			RemoveSceneFromBuildSettings(AssetDatabase.GetAssetPath(sceneAsset));
		}

		public static void RemoveSceneFromBuildSettings(string scenePath) {
			if (!IsSceneInBuildList(scenePath)) return; // If the element isn't in the build list stop
			var newSettings = EditorBuildSettings.scenes.Where(x => x.path != scenePath);
			EditorBuildSettings.scenes = newSettings.ToArray();
		}
	}
#endif
}