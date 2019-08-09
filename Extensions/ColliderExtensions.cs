using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class ColliderExtensions {
		/// <summary>
		/// Returns a smaller axis aligned bounding box by resetting the rotation first, getting the AABB then resetting the rotation
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static Bounds GetSmallerAABB(this Collider that) {
			var transform = that.transform;
			var originalRotation = transform.rotation;
			transform.rotation = Quaternion.identity;
			var bounds = that.bounds;
			transform.rotation = originalRotation;
			return bounds;
		}
	}
}