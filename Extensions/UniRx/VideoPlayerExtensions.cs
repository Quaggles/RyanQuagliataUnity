﻿using UniRx.Async;
using UnityEngine.Video;

namespace RyanQuagliataUnity.Extensions.UniRx {
    public static class VideoPlayerExtensions {

        public static async UniTask PrepareAsync(this VideoPlayer that) {
            if (that.clip == null) return;
            var prepareTcs = new UniTaskCompletionSource();
            void PrepareCompleted(VideoPlayer x) => prepareTcs.TrySetResult();
            that.prepareCompleted += PrepareCompleted;
            try {
                that.Prepare();
                await prepareTcs.Task;
            } finally {
                that.prepareCompleted -= PrepareCompleted;
            }
        }

        public static async UniTask SeekAsync(this VideoPlayer that, double time) {
            if (!that.canSetTime) return;
            if (that.clip == null) return;
            if (!that.isPrepared) await that.PrepareAsync();
            var seekTcs = new UniTaskCompletionSource();
            void SeekCompleted(VideoPlayer x) => seekTcs.TrySetResult();
            that.seekCompleted += SeekCompleted;
            try {
                that.time = time;
                await seekTcs.Task;
            } finally {
                that.seekCompleted -= SeekCompleted;
            }
        }

        public static async UniTask StepForwardAsync(this VideoPlayer that) => await that.StepAsync(1);

        public static async UniTask StepAsync(this VideoPlayer that, int frames) {
            var tcs = new UniTaskCompletionSource();
            void SeekCompleted(VideoPlayer source) => tcs.TrySetResult();
            that.seekCompleted += SeekCompleted;
            that.frame += frames;
            await tcs.Task;
#if UNITY_WEBGL
			that.Play();
			await UniTask.DelayFrame(1);
			that.Pause();
#endif
            that.seekCompleted -= SeekCompleted;
        }
    }
}