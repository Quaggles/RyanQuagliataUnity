using UnityEngine;

namespace RyanQuagliata.Extensions {
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
	}
}