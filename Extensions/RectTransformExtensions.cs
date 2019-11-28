using System;
using JetBrains.Annotations;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class RectTransformExtensions {
		static public Rect WorldRect([NotNull] this RectTransform rectTransform) {
			if (rectTransform == null) throw new ArgumentNullException(nameof(rectTransform));
			Vector3 topLeft = GetWorldCorners(rectTransform, 0);
			Vector2 scaledSize = new Vector2(rectTransform.rect.size.x, rectTransform.rect.size.y);
			return new Rect(topLeft, scaledSize);
		}

		/// <summary>
		/// Garbageless GetWorldCorners method
		/// 0 = Bottom Left
		/// 1 = Top Left
		/// 2 = Top Right
		/// 3 = Bottom Right
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="cornerIndex"></param>
		/// <returns></returns>
		public static Vector3 GetWorldCorners([NotNull] this RectTransform rectTransform, int cornerIndex) {
			if (rectTransform == null) throw new ArgumentNullException(nameof(rectTransform));
			Matrix4x4 localToWorldMatrix = rectTransform.localToWorldMatrix;
			return localToWorldMatrix.MultiplyPoint(GetLocalCorners(rectTransform, cornerIndex));
		}

		/// <summary>
		/// Garbageless GetLocalCorners method
		/// 0 = Bottom Left
		/// 1 = Top Left
		/// 2 = Top Right
		/// 3 = Bottom Right
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="cornerIndex"></param>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static Vector3 GetLocalCorners([NotNull] this RectTransform rectTransform, int cornerIndex) {
			if (rectTransform == null) throw new ArgumentNullException(nameof(rectTransform));
			Rect rect = rectTransform.rect;
			float x = rect.x;
			float y = rect.y;
			float xMax = rect.xMax;

			float yMax = rect.yMax;
			switch (cornerIndex) {
				case 0:
					return new Vector3(x, y, 0.0f);
				case 1:
					return new Vector3(x, yMax, 0.0f);
				case 2:
					return new Vector3(xMax, yMax, 0.0f);
				case 3:
					return new Vector3(xMax, y, 0.0f);
				default:
					throw new IndexOutOfRangeException();
			}
		}
	}
}