using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming, following DOTween naming conventions instead

namespace RyanQuagliata.PluginExtensions.DOTween {
	public static class LayoutElementExtensions {
		public static Tweener DOPreferredSize(this LayoutElement target, Vector2 endValue, float duration) =>
			DG.Tweening.DOTween.To(() => new Vector2(target.preferredWidth, target.preferredHeight), vec => {
					target.preferredWidth = vec.x;
					target.preferredHeight = vec.y;
				}, endValue, duration)
				.SetTarget(target);
	}
}