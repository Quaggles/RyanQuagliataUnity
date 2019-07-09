using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class CameraExtensions {
		public static void SetClippingPlanes(this Camera that, Vector2 clippingPlanes) {
			that.nearClipPlane = clippingPlanes.x;
			that.farClipPlane = clippingPlanes.y;
		}
	}
}