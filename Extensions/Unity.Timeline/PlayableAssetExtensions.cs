using System.Linq;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace RyanQuagliataUnity.Extensions.Unity.Timeline {
	public static class PlayableAssetExtensions {
		public static TrackAsset FindAnimationTrack(this PlayableAsset that, string trackName) => that.outputs.FirstOrDefault(x => string.Equals(x.streamName, trackName)).sourceObject as TrackAsset;
	}
}