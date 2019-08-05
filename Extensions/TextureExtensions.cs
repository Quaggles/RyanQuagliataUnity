using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class TextureExtensions {
		public static float GetAspectRatio(this Texture that) {
			return (float) that.width / (float) that.height;
		}
	}
}