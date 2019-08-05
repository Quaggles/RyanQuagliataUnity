using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace RyanQuagliataUnity.Extensions {
	public static class SceneExtensions {
		public static T GetComponentInScene<T>(this Scene scene) {
			if (!scene.IsValid()) throw new ArgumentException("Value should be valid.", nameof(scene));
			foreach (var rootGameObject in scene.GetRootGameObjects()) {
				var result = rootGameObject.GetComponentInChildren<T>();
				if (!EqualityComparer<T>.Default.Equals(result, default(T))) return result;
			}
			return default(T);
		}
		
		public static object GetComponentInScene(this Scene scene, Type type) {
			if (!scene.IsValid()) throw new ArgumentException("Value should be valid.", nameof(scene));
			foreach (var rootGameObject in scene.GetRootGameObjects()) {
				var result = rootGameObject.GetComponentInChildren(type);
				if (result != null) return result;
			}
			return default(object);
		}
		
		public static T GetComponentInScenes<T>([NotNull] this IEnumerable<Scene> scenes) {
			if (scenes == null) throw new ArgumentNullException(nameof(scenes));
			foreach (var scene in scenes) {
				var result = scene.GetComponentInScene<T>();
				if (!EqualityComparer<T>.Default.Equals(result, default(T))) return result;
			}
			return default(T);
		}

		public static ICollection<T> GetComponentsInScene<T>(this Scene scene, ICollection<T> collection = null) {
			if (!scene.IsValid()) throw new ArgumentException("Value should be valid.", nameof(scene));
			collection?.Clear();
			collection = collection ?? new List<T>();
			scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<T>()).ForEach(x => collection.Add(x));
			return collection;
		}
		
		public static ICollection<T> GetComponentsInScenes<T>([NotNull] this IEnumerable<Scene> scenes, ICollection<T> collection = null) {
			if (scenes == null) throw new ArgumentNullException(nameof(scenes));
			collection?.Clear();
			collection = collection ?? new List<T>();
			foreach (var scene in scenes) {
				collection.AddRange(scene.GetComponentsInScene<T>());
			}
			return collection;
		}
	}
}
