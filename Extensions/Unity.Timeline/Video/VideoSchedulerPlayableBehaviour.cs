using System;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace RyanQuagliataUnity.Extensions.Unity.Timeline.Video {
    public sealed class VideoSchedulerPlayableBehaviour : PlayableBehaviour {
        private IEnumerable<TimelineClip> m_Clips;
        private PlayableDirector m_Director;
        public VideoPlayer VideoPlayer;

        internal PlayableDirector director {
            get => m_Director;
            set => m_Director = value;
        }

        internal IEnumerable<TimelineClip> clips {
            get => m_Clips;
            set => m_Clips = value;
        }


        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            if (m_Clips == null) return;

            int inputPort = 0;

            TimelineClip previousClip = null;
            bool inRange = false;
            foreach (var clip in m_Clips) {
                var videoPlayableBehaviour = ((ScriptPlayable<VideoPlayableBehaviour>) playable.GetInput(inputPort)).GetBehaviour();

                if (videoPlayableBehaviour != null) {
                    videoPlayableBehaviour.videoPlayer = VideoPlayer;
                    videoPlayableBehaviour.TimelineClip = clip;
                    var preloadTime = Math.Min(previousClip?.end ?? 0f, clip.start);
                    // We're in an empty space before a video so start preparing
                    if (m_Director.time > preloadTime && m_Director.time < clip.start) videoPlayableBehaviour.PrepareVideo();
                    // A clip is playing so video player should be visible
                    if (m_Director.time >= clip.start && m_Director.time <= clip.end) inRange = true;
                }

                ++inputPort;
                previousClip = clip;
            }

            if (!inRange) VideoPlayer.targetCameraAlpha = 0;
        }
    }
}