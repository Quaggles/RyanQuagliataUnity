using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RyanQuagliataUnity.Extensions {
	public static class ObjectExtensions {
		public static Object Instantiate(Object obj, Transform parent = null, bool worldPositionStays = false) {
#if UNITY_EDITOR
			var instance = UnityEditor.PrefabUtility.InstantiatePrefab(obj);

			// If a parent transform was provided and the object we are instantiating is a component set the parent
			if (parent != null && instance is Component component) component.transform.SetParent(parent, worldPositionStays);
			return instance;
#else
			return Object.Instantiate(obj);
#endif
		}

		public static T Instantiate<T>(T obj, Transform parent = null, bool worldPositionStays = false) where T : Object {
#if UNITY_EDITOR
			var instance = UnityEditor.PrefabUtility.InstantiatePrefab(obj) as T;

			// If a parent transform was provided and the object we are instantiating is a component set the parent
			if (parent != null && instance is Component component) component.transform.SetParent(parent, worldPositionStays);
			return instance;
#else
			return Object.Instantiate(obj, parent, worldPositionStays);
#endif
		}

		public static void DestroySmart(this Object that) {
#if UNITY_EDITOR
			if (Application.isPlaying)
				Object.Destroy(that);
			else
				Object.DestroyImmediate(that);
#else
			Object.Destroy(that);
#endif
		}

		/// <summary>
		/// Returns the first object of type that can be found in loaded scenes, this will not include objects that are DontDestroyOnLoad since that scene is inaccessible
		/// </summary>
		/// <param name="includeInactive"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T FindSceneObjectOfType<T>(bool includeInactive = false) where T : Component {
			List<GameObject> rootList = new List<GameObject>();
			for (int i = 0; i < SceneManager.sceneCount; i++) {
				var s = SceneManager.GetSceneAt(i);
				if (s.isLoaded) {
					s.GetRootGameObjects(rootList);
					for (int j = 0; j < rootList.Count; j++) {
						var go = rootList[j];
						var result = go.GetComponentInChildren<T>(includeInactive);
						if (result) return result;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Returns objects of type that can be found in loaded scenes, this will not include objects that are DontDestroyOnLoad since that scene is inaccessible
		/// </summary>
		/// <param name="includeInactive"></param>
		/// <param name="resultList"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<T> FindSceneObjectsOfType<T>(bool includeInactive = false, List<T> resultList = null) where T : Component {
			if (resultList == null) resultList = new List<T>();
			List<GameObject> rootList = new List<GameObject>();
			for (int i = 0; i < SceneManager.sceneCount; i++) {
				var s = SceneManager.GetSceneAt(i);
				if (s.isLoaded) {
					s.GetRootGameObjects(rootList);
					for (int j = 0; j < rootList.Count; j++) {
						var go = rootList[j];
						resultList.AddRange(go.GetComponentsInChildren<T>(includeInactive));
					}
				}
			}

			return resultList;
		}

		/// <summary>
		/// Returns the first object that can be found of type, this will include objects that are DontDestroyOnLoad since it searches all objects in memory then filters out those not existing in a scene
		/// </summary>
		/// <param name="includeInactive"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T FindLoadedObjectOfType<T>(bool includeInactive = false) where T : Component {
			var objects = Resources.FindObjectsOfTypeAll<T>();
			for (var i = 0; i < objects.Length; i++) {
				var obj = objects[i];
				// Skip inactive objects if that was requested
				if (!includeInactive && !obj.gameObject.activeInHierarchy) continue;
				// Make sure the object exists in a scene to avoid returning random objects in memory
				if (string.IsNullOrWhiteSpace(obj.gameObject.scene.name)) continue;
				return obj;
			}

			return null;
		}

		/// <summary>
		/// Returns objects that can be found of type, this will include objects that are DontDestroyOnLoad since it searches all objects in memory then filters out those not existing in a scene
		/// </summary>
		/// <param name="includeInactive"></param>
		/// <param name="resultList"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<T> FindLoadedObjectsOfType<T>(bool includeInactive = false, List<T> resultList = null) where T : Component {
			if (resultList == null) resultList = new List<T>();
			var objects = Resources.FindObjectsOfTypeAll<T>();
			for (var i = 0; i < objects.Length; i++) {
				var obj = objects[i];
				// Skip inactive objects if that was requested
				if (!includeInactive && !obj.gameObject.activeInHierarchy) continue;
				// Make sure the object exists in a scene to avoid returning random objects in memory
				if (string.IsNullOrWhiteSpace(obj.gameObject.scene.name)) continue;
				resultList.Add(obj);
			}

			return resultList;
		}
	}
}