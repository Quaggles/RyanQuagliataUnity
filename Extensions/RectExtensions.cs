using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class RectExtensions {
		public static Rect Scale(this Rect rect, float scaleFactor) => Scale(rect, Vector2.one * scaleFactor);

		public static Rect Scale(this Rect rect, Vector2 scale) => new Rect(rect.position, Vector2.Scale(rect.size, scale));
	}
}