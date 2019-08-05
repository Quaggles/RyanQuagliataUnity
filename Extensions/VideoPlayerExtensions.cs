using UniRx.Async;
using UnityEngine.Video;

namespace RyanQuagliataUnity.Extensions {
    public static class VideoPlayerExtensions {
        public static double GetNormalizedTime(this VideoPlayer that) => that.time.Remap(0, that.length, 0, 1);

        public static double SetNormalizedTime(this VideoPlayer that, double time) => that.time = time.Remap(0, 1, 0, that.length);

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

        public static bool TogglePlayState(this VideoPlayer that) {
            if (that.isPlaying)
                that.Pause();
            else
                that.Play();
            return that.isPlaying;
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