﻿using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace RyanQuagliataUnity.Extensions.UniTask {
	public static class UnityWebRequestExtensions {
		public static async Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest that, IProgress<float> progress = null) {
			var task = that.SendWebRequest();
			if (progress != null) {
				float lastReportProgress = -1;
				while (!task.isDone) {
					await Cysharp.Threading.Tasks.UniTask.DelayFrame(1);
										
					// Don't rereport the same progress
					if (lastReportProgress == task.progress) continue;
					lastReportProgress = task.progress;
					progress.Report(task.progress);
				}
			}
			await task;
			if (that.isHttpError) throw new Exception($"HTTP Error Code {that.responseCode} {that.error}");
			if (that.isNetworkError) throw new Exception($"Network Error {that.responseCode} {that.error}");
			return that;
		}
		
		public static async UniTask<UnityWebRequest> SendWebRequestUniAsync(this UnityWebRequest that, IProgress<float> progress = null) {
			var task = that.SendWebRequest();
			if (progress != null) {
				float lastReportProgress = -1;
				while (!task.isDone) {
					await Cysharp.Threading.Tasks.UniTask.DelayFrame(1);
					
					// Don't rereport the same progress
					if (lastReportProgress == task.progress) continue;
					lastReportProgress = task.progress;
					progress.Report(task.progress);
				}
			}
			await task;
			if (that.isHttpError) throw new Exception($"HTTP Error Code {that.responseCode} {that.error}");
			if (that.isNetworkError) throw new Exception($"Network Error {that.responseCode} {that.error}");
			return that;
		}
	}
}
