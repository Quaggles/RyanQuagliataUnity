using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace RyanQuagliata.PluginExtensions.DOTween {
	public class CompositeGraphic : Graphic {
		public List<Graphic> TargetGraphics = new List<Graphic>();
		private Tween colourTween;
		public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha) {
			colourTween?.Kill();
			foreach (var graphic in TargetGraphics) {
				if (graphic == null) continue;
				if (UseDoTweenInsteadOfUnityTween)
					colourTween = DOTweenExtensions.DOColor(graphic, targetColor, duration);
				else
					graphic?.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
			}
		}

		public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha, bool useRGB) {
			colourTween?.Kill();
			foreach (var graphic in TargetGraphics) {
				if (graphic == null) continue;
				if (UseDoTweenInsteadOfUnityTween)
					colourTween = DOTweenExtensions.DOColor(graphic, targetColor, duration);
				else
					graphic.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, useRGB);
			}
		}

		public override void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale) {
			colourTween?.Kill();
			foreach (var graphic in TargetGraphics) {
				if (graphic == null) continue;
				if (UseDoTweenInsteadOfUnityTween) {
					var newColor = graphic.color;
					newColor.a = alpha;
					colourTween = DOTweenExtensions.DOColor(graphic, newColor, duration);
				} else
					graphic.CrossFadeAlpha(alpha, duration, ignoreTimeScale);
			}
		}

		public bool UseDoTweenInsteadOfUnityTween = false;

		public void SetColor(Color colour) {
			foreach (var targetGraphic in TargetGraphics) {
				targetGraphic.color = colour;
			}
		} 
		
		[BoxGroup(nameof(FetchChildrenGraphics))]
		public bool FetchIncludeInactive = false;
		
		[BoxGroup(nameof(FetchChildrenGraphics))]
		[Button]
		private void FetchChildrenGraphics() {
			GetComponentsInChildren(FetchIncludeInactive, TargetGraphics);
			TargetGraphics.Remove(this);
		}
	}
}
