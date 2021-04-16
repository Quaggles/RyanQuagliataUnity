using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class TransformExtensions {
		/// <summary>
		/// Transforms rotation from world space to local space
		/// </summary>
		/// <param name="that"></param>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public static Quaternion InverseTransformRotation(this Transform that, Quaternion rotation) {
			return Quaternion.Inverse(that.rotation) * rotation;
		}

		/// <summary>
		/// Transforms position from local space to world space
		/// </summary>
		/// <param name="that"></param>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public static Quaternion TransformRotation(this Transform that, Quaternion rotation) {
			return that.rotation * rotation;
		}
		
		public static bool HasParent(this Transform that, Transform parent) {
			Transform newParent = that;
			while (newParent) {
				newParent = newParent.parent;
				if (newParent == parent) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns the path of a transform within its scene
		/// Format: {Scene.Name}/[Root]/path/to/transform/{Transform.Name}
		/// </summary>
		/// <param name="current"></param>
		/// <returns></returns>
		public static string GetPath(this Transform current, bool relativeToScene = false) {
			if (current.parent == null) return relativeToScene ? current.name : $"{current.gameObject.scene.name}/[Root]/{current.name}";
			return $"{current.parent.GetPath(relativeToScene)}/{current.name}";
		}
	}
}