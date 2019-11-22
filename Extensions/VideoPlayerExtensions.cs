using UnityEngine.Video;

namespace RyanQuagliataUnity.Extensions {
	public static class VideoPlayerExtensions {
		public static double GetNormalizedTime(this VideoPlayer that) => that.time.Remap(0, that.length, 0, 1);

		public static double SetNormalizedTime(this VideoPlayer that, double time) => that.time = time.Remap(0, 1, 0, that.length);

		public static bool TogglePlayState(this VideoPlayer that) {
			if (that.isPlaying)
				that.Pause();
			else
				that.Play();
			return that.isPlaying;
		}
	}
}