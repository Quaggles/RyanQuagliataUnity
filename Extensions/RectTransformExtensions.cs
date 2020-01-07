using System;
using JetBrains.Annotations;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class RectTransformExtensions {
		/// <summary>
		/// Gets the rect in screen space
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="canvas">Optional, if provided avoids an expensive GetComponentInParent call</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static Rect ScreenSpaceRect(this RectTransform rectTransform, Canvas canvas = null) {
			if (rectTransform == null) throw new ArgumentNullException(nameof(rectTransform));
			var screenRect = new Rect();
			if (!canvas) canvas = rectTransform.GetComponentInParent<Canvas>();
			if (!canvas) throw new InvalidOperationException($"{nameof(RectTransform)} has no parent canvas");
			var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
			var min = RectTransformUtility.WorldToScreenPoint(cam, rectTransform.GetWorldCorners(0));
			var max = RectTransformUtility.WorldToScreenPoint(cam, rectTransform.GetWorldCorners(2));
			screenRect.min = min;
			screenRect.max = max;
			return screenRect;
		}

		static public Rect WorldRect([NotNull] this RectTransform rectTransform) {
			if (rectTransform == null) throw new ArgumentNullException(nameof(rectTransform));
			var worldRect = new Rect();
			worldRect.min = rectTransform.GetWorldCorners(0);
			worldRect.max = rectTransform.GetWorldCorners(2);
			return worldRect;
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