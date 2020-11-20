using System;
using JetBrains.Annotations;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class RectTransformExtensions {
		public static Rect ScreenSpaceRect(this RectTransform rectTransform) {
			var screenRect = new Rect();
			var canvas = rectTransform.GetComponentInParent<Canvas>();
			if (!canvas) throw new InvalidOperationException($"{nameof(RectTransform)} has no parent canvas");
			var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
			var min = RectTransformUtility.WorldToScreenPoint(cam, rectTransform.GetWorldCorners(0));
			var max = RectTransformUtility.WorldToScreenPoint(cam, rectTransform.GetWorldCorners(2));
			screenRect.min = min;
			screenRect.max = max;
			return screenRect;
		}

		/// <summary>
		/// Gets the rect in screen space
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="canvas">Optional, if provided avoids an expensive GetComponentInParent call</param>
		/// <param name="camera"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static Rect ScreenSpaceRect(this RectTransform rectTransform, Camera camera) {
			var screenRect = new Rect();
			var min = RectTransformUtility.WorldToScreenPoint(camera, rectTransform.GetWorldCorners(0));
			var max = RectTransformUtility.WorldToScreenPoint(camera, rectTransform.GetWorldCorners(2));
			screenRect.min = min;
			screenRect.max = max;
			return screenRect;
		}
		
		public static Bounds ScreenSpaceBounds(this RectTransform rectTransform, Camera camera) {
			var bounds = new Bounds();
			var min = RectTransformUtility.WorldToScreenPoint(camera, rectTransform.GetWorldCorners(0));
			var max = RectTransformUtility.WorldToScreenPoint(camera, rectTransform.GetWorldCorners(2));
			bounds.min = min;
			bounds.max = max;
			return bounds;
		}

		static public Rect WorldRect([NotNull] this RectTransform rectTransform) {
			return new Rect {
				min = rectTransform.GetWorldCorners(0),
				max = rectTransform.GetWorldCorners(2)
			};
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
		public static Vector3 GetWorldCorners([NotNull] this RectTransform rectTransform, int cornerIndex) => 
			rectTransform.localToWorldMatrix.MultiplyPoint(GetLocalCorners(rectTransform, cornerIndex));

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