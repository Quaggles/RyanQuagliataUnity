using UnityEngine;
using UnityEngine.Playables;

namespace RyanQuagliataUnity.Extensions.Unity.Timeline {
	public static class PlayableDirectorExtensions {
		public static Object GetGenericBinding(this PlayableDirector that, string trackName) => that.GetGenericBinding(that.playableAsset.FindAnimationTrack(trackName));
	}
}