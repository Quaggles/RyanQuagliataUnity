using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace RyanQuagliataUnity.Extensions.Unity.Timeline.Video {
	[Serializable]
	public class VideoScriptPlayableAsset : PlayableAsset, ITimelineClipAsset {
		[SerializeField, NotKeyable]
		public VideoClip videoClip;

		[SerializeField, NotKeyable]
		public bool mute = false;

		/// <summary>
		/// If the videoClip is provided return the length, otherwise return a default 15 seconds
		/// </summary>
		public override double duration {
			get {
				if (videoClip != null) return videoClip.length;
				return 15;
			}
		}

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go) {
			ScriptPlayable<VideoPlayableBehaviour> playable = ScriptPlayable<VideoPlayableBehaviour>.Create(graph);
			VideoPlayableBehaviour playableBehaviour = playable.GetBehaviour();

			playableBehaviour.videoClip = videoClip;
			playableBehaviour.mute = mute;

			return playable;
		}

        public ClipCaps clipCaps => ClipCaps.SpeedMultiplier | ClipCaps.Looping | ClipCaps.ClipIn | ClipCaps.Extrapolation | ClipCaps.Blending;
    }
}