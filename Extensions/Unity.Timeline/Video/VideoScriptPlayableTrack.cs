using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace RyanQuagliataUnity.Extensions.Unity.Timeline.Video {
	[Serializable]
	[TrackClipType(typeof(VideoScriptPlayableAsset))]
	[TrackColor(0.008f, 0.698f, 0.655f)]
	[TrackBindingType(typeof(VideoPlayer))]
	public class VideoScriptPlayableTrack : TrackAsset {
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
			PlayableDirector playableDirector = go.GetComponent<PlayableDirector>();

			ScriptPlayable<VideoSchedulerPlayableBehaviour> playable = ScriptPlayable<VideoSchedulerPlayableBehaviour>.Create(graph, inputCount);

			VideoSchedulerPlayableBehaviour videoSchedulerPlayableBehaviour = playable.GetBehaviour();

			if (videoSchedulerPlayableBehaviour != null) {
				videoSchedulerPlayableBehaviour.VideoPlayer = playableDirector.GetGenericBinding(this) as VideoPlayer;
				videoSchedulerPlayableBehaviour.director = playableDirector;
				videoSchedulerPlayableBehaviour.clips = GetClips();
			}

			return playable;
		}
	}
}