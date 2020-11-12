using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.UniTask {
	public static class UniTaskExtensions {
		/// <summary>
		/// Waits for the given timespan and then throws an exception if the timeout was reached before the task finished
		/// </summary>
		/// <param name="that"></param>
		/// <param name="timeout"></param>
		/// <param name="timeoutProgress"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="TaskCanceledException">Thrown if the timeout is cancelled</exception>
		/// <exception cref="TimeoutException"></exception>
		public static async UniTask<T> Timeout<T>(this UniTask<T> that, TimeSpan timeout, IProgress<float> timeoutProgress, bool unscaledTime = false, CancellationToken cancellationToken = default) {
			if (timeout.IsInfinite()) {
				timeoutProgress?.Report(0);
				return await that;
			}
			var startTime = unscaledTime ? Time.unscaledTime : Time.time;
			var endTime = startTime + (float)timeout.TotalSeconds;
			
			// When the task hasn't been completed report its progress
			while (that.Status == UniTaskStatus.Pending && !cancellationToken.IsCancellationRequested) {
				var fillAmount = Mathf.InverseLerp(startTime, endTime, unscaledTime ? Time.unscaledTime : Time.time);
				timeoutProgress?.Report(fillAmount);
				await Cysharp.Threading.Tasks.UniTask.Delay(1000/60, true, cancellationToken: cancellationToken);
				if ((unscaledTime ? Time.unscaledTime : Time.time) > endTime) throw new TimeoutException();
			}
			if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

			return await that;
		}

		public static bool IsCompleted(this Cysharp.Threading.Tasks.UniTask that) => that.GetAwaiter().IsCompleted;
		public static bool IsCompleted<T>(this UniTask<T> that) => that.GetAwaiter().IsCompleted;
	}
}