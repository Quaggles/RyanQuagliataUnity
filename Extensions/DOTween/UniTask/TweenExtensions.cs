using DG.Tweening;
using DG.Tweening.Core;

namespace RyanQuagliataUnity.Extensions.DOTween.UniTask {
	public static class TweenExtensions {
		/// <summary>
		/// Returns an async <see cref="Task"/> that waits until the tween is killed or complete.
		/// It can be used inside an async operation.
		/// <para>Example usage:</para><code>await myTween.WaitForCompletion();</code>
		/// </summary>
		public static async Cysharp.Threading.Tasks.UniTask AsyncWaitForCompletion(this Tween that) {
			if (!that.active) {
				if (Debugger.logPriority > 0) Debugger.LogInvalidTween(that);
				return;
			}

			while (that.active && !that.IsComplete()) await Cysharp.Threading.Tasks.UniTask.Yield();
		}

		/// <summary>
		/// Returns an async <see cref="Task"/> that waits until the tween is killed.
		/// It can be used inside an async operation.
		/// <para>Example usage:</para><code>await myTween.AsyncWaitForKill();</code>
		/// </summary>
		public static async Cysharp.Threading.Tasks.UniTask AsyncWaitForKill(this Tween that) {
			if (!that.active) {
				if (Debugger.logPriority > 0) Debugger.LogInvalidTween(that);
				return;
			}

			while (that.active) await Cysharp.Threading.Tasks.UniTask.Yield();
		}
	}
}