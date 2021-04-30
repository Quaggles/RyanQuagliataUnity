using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace RyanQuagliataUnity.Extensions.DOTween {
	public static class DOTweenExtensions {
		public static TweenerCore<Color, Color, ColorOptions> DOColor(this Graphic that, Color endValue, float duration, bool snapping = false) {
			var t = DG.Tweening.DOTween.To(() => that.color, x => that.color = x, endValue, duration);
			t.SetOptions(snapping).SetTarget(that);
			return t;
		}
		
		public static TweenerCore<float, float, FloatOptions> DOFade(this CanvasGroup that, float endAlpha, float duration, bool snapping = false) {
			var t = DG.Tweening.DOTween.To(() => that.alpha, x => that.alpha = x, endAlpha, duration);
			t.SetOptions(snapping).SetTarget(that);
			return t;
		}

		/// <summary>Tweens a property or field to the given value using default plugins</summary>
		/// <param name="getter">A getter for the field or property to tween.
		/// <para>Example usage with lambda:</para><code>()=&gt; myProperty</code></param>
		/// <param name="setter">A setter for the field or property to tween
		/// <para>Example usage with lambda:</para><code>x=&gt; myProperty = x</code></param>
		/// <param name="endValue">The end value to reach</param>
		/// <param name="settings">Tweener Settings to use for this tween</param>
		public static TweenerCore<float, float, FloatOptions> To(DOGetter<float> getter, DOSetter<float> setter, float endValue, TweenerSettings settings) {
			TweenerCore<float, float, FloatOptions> tweener = DG.Tweening.DOTween.To(getter, setter, endValue, settings.Duration).ApplySettings(settings);
			return tweener;
		}

		/// <summary>
		/// Applies the TweenerSettings to this tweener, this will clear OnX callbacks <code>e.g. OnStart(<b>action</b>) if the relevant TweenerSetting events have listeners</code>
		/// </summary>
		/// <param name="tweener">This</param>
		/// <param name="settings">Tweener settings to apply</param>
		/// <returns>The modified tweener</returns>
		public static Tweener ApplySettings([NotNull] this Tweener tweener, TweenerSettings settings) {
			if (tweener == null) throw new ArgumentNullException(nameof(tweener));
			if (settings == null) return tweener;
			tweener.SetDelay(settings.Delay);

			if (settings.StopMotionEase)
				tweener.SetEase(EaseFactory.StopMotion(settings.StopMotionEaseFrameRate, settings.EaseFunction));
			else
				tweener.SetEase(settings.EaseFunction, settings.EaseOvershoot, settings.EasePeriod);

			tweener.SetAutoKill(settings.AutoKill);

			tweener.SetLoops(settings.Loops, settings.LoopType);
			tweener.SetUpdate(settings.UpdateType);

			// Events
			if (settings.OnStart.GetPersistentEventCount() > 0) tweener.OnStart(() => settings.OnStart?.Invoke());
			if (settings.OnKill.GetPersistentEventCount() > 0) tweener.OnKill(() => settings.OnKill?.Invoke());
			if (settings.OnPlay.GetPersistentEventCount() > 0) tweener.OnPlay(() => settings.OnPlay?.Invoke());
			if (settings.OnPause.GetPersistentEventCount() > 0) tweener.OnPause(() => settings.OnPause?.Invoke());
			if (settings.OnRewind.GetPersistentEventCount() > 0) tweener.OnRewind(() => settings.OnRewind?.Invoke());
			if (settings.OnStepComplete.GetPersistentEventCount() > 0) tweener.OnStepComplete(() => settings.OnStepComplete?.Invoke());
			if (settings.OnComplete.GetPersistentEventCount() > 0) tweener.OnComplete(() => settings.OnComplete?.Invoke());
			return tweener;
		}
		
		/// <summary>
		/// Applies the TweenerSettings to this tweener, this will clear OnX callbacks <code>e.g. OnStart(<b>action</b>) if the relevant TweenerSetting events have listeners</code>
		/// </summary>
		/// <param name="tweener">This</param>
		/// <param name="settings">Tweener settings to apply</param>
		/// <returns>The modified tweener</returns>
		public static Tweener ApplySettings([NotNull] this Tweener tweener, TweenerSettingsV2 settings) {
			if (tweener == null) throw new ArgumentNullException(nameof(tweener));
			if (settings == null) return tweener;
			tweener.SetDelay(settings.Delay);

			if (settings.StopMotionEase)
				tweener.SetEase(EaseFactory.StopMotion(settings.StopMotionEaseFrameRate, settings.EaseFunction));
			else
				tweener.SetEase(settings.EaseFunction, settings.EaseOvershoot, settings.EasePeriod);

			tweener.SetAutoKill(settings.AutoKill);

			tweener.SetLoops(settings.Loops, settings.LoopType);
			tweener.SetUpdate(settings.UpdateType);

			// Events
			if (settings.OnStart.GetPersistentEventCount() > 0) tweener.OnStart(() => settings.OnStart?.Invoke());
			if (settings.OnKill.GetPersistentEventCount() > 0) tweener.OnKill(() => settings.OnKill?.Invoke());
			if (settings.OnPlay.GetPersistentEventCount() > 0) tweener.OnPlay(() => settings.OnPlay?.Invoke());
			if (settings.OnPause.GetPersistentEventCount() > 0) tweener.OnPause(() => settings.OnPause?.Invoke());
			if (settings.OnRewind.GetPersistentEventCount() > 0) tweener.OnRewind(() => settings.OnRewind?.Invoke());
			if (settings.OnStepComplete.GetPersistentEventCount() > 0) tweener.OnStepComplete(() => settings.OnStepComplete?.Invoke());
			if (settings.OnComplete.GetPersistentEventCount() > 0) tweener.OnComplete(() => settings.OnComplete?.Invoke());
			return tweener;
		}

		/// <summary>
		/// Applies the TweenerSettings to this tweener, this will clear OnX callbacks <code>e.g. OnStart(<b>action</b>) if the relevant TweenerSetting events have listeners</code>
		/// </summary>
		/// <param name="tweener">This</param>
		/// <param name="settings">Tweener settings to apply</param>
		/// <param name="inEase"></param>
		/// <returns>The modified tweener</returns>
		public static Tweener ApplySettingsVariant([NotNull] this Tweener tweener, TweenerSettingsV2 settings, bool inEase) {
			if (settings == null) return tweener;
			ApplySettings(tweener, settings);
			if (!settings.StopMotionEase)
				tweener.SetEase(settings.GetEaseVariant(settings.EaseFunction, inEase), settings.EaseOvershoot, settings.EasePeriod);
			return tweener;
		}

		private static TweenerCore<T1, T2, TPlugOptions> ApplySettings<T1, T2, TPlugOptions>(this TweenerCore<T1, T2, TPlugOptions> tweenerCore, TweenerSettings settings)
			where TPlugOptions : struct, IPlugOptions {
			var tweener = tweenerCore as Tweener;
			tweener.ApplySettings(settings);
			return tweenerCore;
		}
		
		private static TweenerCore<T1, T2, TPlugOptions> ApplySettings<T1, T2, TPlugOptions>(this TweenerCore<T1, T2, TPlugOptions> tweenerCore, TweenerSettingsV2 settings)
			where TPlugOptions : struct, IPlugOptions {
			var tweener = tweenerCore as Tweener;
			tweener.ApplySettings(settings);
			return tweenerCore;
		}

		public static T2 GetEndValue<T1, T2, TPlugOptions>(this Tween that) where TPlugOptions : struct, IPlugOptions {
			TweenerCore<T1, T2, TPlugOptions> tweenerCore = that as TweenerCore<T1, T2, TPlugOptions>;
			return tweenerCore.endValue;
		}
	}
}