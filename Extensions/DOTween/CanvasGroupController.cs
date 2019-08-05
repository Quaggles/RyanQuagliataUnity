using DG.Tweening;
using DG.Tweening.Plugins.Options;
using RyanQuagliataUnity.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliata.PluginExtensions.DOTween {
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasGroupController : MonoBehaviour {
		[MinMaxSlider(0, 1, true)]
		public Vector2 TransparencyRange = Vector2.up;

		public float InvisibleAlpha => TransparencyRange.x;
		public float VisibleAlpha => TransparencyRange.y;

		[InlineEditor]
		[InlineButton(nameof(CreateTweenerSettingsInstance), "new ()")]
		public TweenerSettings FadeTweenSettings;

		void CreateTweenerSettingsInstance() => CreateTweenerSettingsIfNull(ref FadeTweenSettings);

		public bool DisableGameObjectWhenInvisible = true;

		private CanvasGroup canvasGroup;

		/// <summary>
		/// If the CanvasGroup doesn't exist try to find it, if that fails make a new one
		/// </summary>
		public CanvasGroup CanvasGroup => canvasGroup != null ? canvasGroup : canvasGroup = this.GetOrAddComponent<CanvasGroup>();

		private Tween fadeTween;

		public void ToggleVisibility() => Visible = !Visible;

		[PropertyOrder(-1)]
		[ShowInInspector]
		public bool Visible {
			get {
				// If a tween is playing check its end value to see if its becoming visible or not
				if (fadeTween != null && fadeTween.IsActive() && fadeTween.IsPlaying()) return fadeTween.GetEndValue<float, float, FloatOptions>().ApproximatelyEqual(VisibleAlpha);

				// Otherwise check if the actual value is 1
				return CanvasGroup.alpha.ApproximatelyEqual(VisibleAlpha);
			}
			set {
				if (Visible == value) return;
				// If in edit mode tween instantly
				SetVisibility(value, !Application.isPlaying);
			}
		}

		public Tween SetVisibility(bool visible, bool instant = false) {
			CanvasGroup.blocksRaycasts = visible;
			CanvasGroup.interactable = visible;
			return SetAlpha(visible ? VisibleAlpha : InvisibleAlpha, instant);
		}

		public Tween SetAlpha(float alpha, bool instant = false) {
			CreateTweenerSettingsIfNull(ref FadeTweenSettings);

			// If the alpha is greater than 0 enable the gameobject
			if (alpha > 0 && !CanvasGroup.gameObject.activeSelf) CanvasGroup.gameObject.SetActive(true);

			CanvasGroup.DOKill();

			// If instant just set the values
			if (instant) {
				CanvasGroup.alpha = alpha;
				if (DisableGameObjectWhenInvisible && alpha == 0) CanvasGroup.gameObject.SetActive(false);
			} else { // If not instant then tween
				fadeTween = CanvasGroup.DOFade(alpha, FadeTweenSettings.Duration).ApplySettings(FadeTweenSettings).SetUpdate(true);
				fadeTween = CanvasGroup.DOFade(alpha, FadeTweenSettings.Duration).ApplySettings(FadeTweenSettings).SetUpdate(true);
				if (DisableGameObjectWhenInvisible && alpha == 0) fadeTween.onComplete += () => canvasGroup.gameObject.SetActive(false);
			}

			return fadeTween;
		}

		void CreateTweenerSettingsIfNull(ref TweenerSettings tweenerSettings) {
			if (tweenerSettings == null) {
				tweenerSettings = ScriptableObject.CreateInstance<TweenerSettings>();
				Debug.LogWarning(
					$"Warning: A tweener setting has not been set on \"{tweenerSettings}\" a default linear tween with a duration of 1 second will be used",
					this);
			}
		}
	}
}