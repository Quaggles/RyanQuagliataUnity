using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class ColliderExtensions {
		/// <summary>
		/// Returns an AABB in local space (Like Mesh.bounds)
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static Bounds GetLocalAABB(this Collider that) {
			var transform = that.transform;
			var originalRotation = transform.rotation;
			transform.rotation = Quaternion.identity;
			var bounds = that.bounds;
			transform.rotation = originalRotation;
			bounds.center = Vector3.zero;
			bounds.size = bounds.size.Divide(transform.localScale);
			return bounds;
		}
	}
}