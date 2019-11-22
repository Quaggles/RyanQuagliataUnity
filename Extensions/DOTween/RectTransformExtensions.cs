using DG.Tweening;
using UnityEngine;

// ReSharper disable InconsistentNaming, following DOTween naming conventions instead

namespace RyanQuagliataUnity.Extensions.DOTween {
	public static class RectTransformExtensions {
		public static Tweener DOMoveAnchoredPosition(this RectTransform target, Vector2 endValue, float duration) =>
			DG.Tweening.DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, endValue, duration)
				.SetTarget(target);

		public static Tweener DOMoveAnchoredPosition3D(this RectTransform target, Vector3 endValue, float duration) =>
			DG.Tweening.DOTween
				.To(() => target.anchoredPosition3D, x => target.anchoredPosition3D = x, endValue, duration)
				.SetTarget(target);
	}
}