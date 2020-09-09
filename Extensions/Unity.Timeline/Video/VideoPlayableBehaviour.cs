using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace RyanQuagliataUnity.Extensions.Unity.Timeline.Video {
	public class VideoPlayableBehaviour : PlayableBehaviour {
		public VideoPlayer videoPlayer;
		public VideoClip videoClip;
        public TimelineClip TimelineClip;
		public bool mute = false;
		public bool loop = false;

		private bool playedOnce = false;
		private bool preparing = false;

		public void PrepareVideo() {
			if (videoPlayer == null || videoClip == null) return;

			videoPlayer.targetCameraAlpha = 0.0f;

            if (videoPlayer.clip != videoClip) StopVideo();

            if (videoPlayer.isPrepared || preparing) return;
			//Debug.Log($"[{nameof(PrepareVideo)}] videoPlayer null? {videoClip == null}");

			videoPlayer.source = VideoSource.VideoClip;
			videoPlayer.clip = videoClip;
			videoPlayer.playOnAwake = false;
			videoPlayer.waitForFirstFrame = true;
			videoPlayer.isLooping = loop;

			for (ushort i = 0; i < videoClip.audioTrackCount; ++i) {
				if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
					videoPlayer.SetDirectAudioMute(i, mute || !Application.isPlaying);
				else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource) {
					AudioSource audioSource = videoPlayer.GetTargetAudioSource(i);
					if (audioSource != null) audioSource.mute = mute || !Application.isPlaying;
				}
			}

			videoPlayer.loopPointReached += LoopPointReached;
			videoPlayer.time = TimelineClip.clipIn;
			//Debug.LogError("PrepareCalled");
			videoPlayer.Prepare();
			preparing = true;
		}

		void LoopPointReached(VideoPlayer vp) {
			//Debug.Log($"[{nameof(LoopPointReached)}] videoPlayer null? {vp == null}");
			playedOnce = !loop;
		}

		public override void PrepareFrame(Playable playable, FrameData info) {
			//Debug.Log($"[{nameof(PrepareFrame)}] videoPlayer null? {videoClip == null}");

			if (videoPlayer == null || videoClip == null) return;

			videoPlayer.timeReference = Application.isPlaying ? VideoTimeReference.ExternalTime : VideoTimeReference.Freerun;

            if (videoPlayer.isPlaying && Application.isPlaying) {
                videoPlayer.externalReferenceTime = playable.GetTime() % videoClip.length;
                //Debug.Log($"External Time: {playable.GetTime() % videoClip.length}");
            } else if (!Application.isPlaying) SyncVideoToPlayable(playable);
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info) {
			//Debug.Log($"[{nameof(OnBehaviourPlay)}] videoPlayer null? {videoClip == null}");

			if (videoPlayer == null) return;

			if (!playedOnce) {
				PlayVideo();
				SyncVideoToPlayable(playable);
			}
		}

		public override void OnBehaviourPause(Playable playable, FrameData info) {
			//Debug.Log($"[{nameof(OnBehaviourPause)}] videoPlayer null? {videoClip == null}");

			if (videoPlayer == null) return;

			if (Application.isPlaying)
				PauseVideo();
			else
				StopVideo();
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            //Debug.Log($"[{nameof(ProcessFrame)}] videoPlayer null? {videoClip == null}");
			if (playerData is VideoPlayer videoPlayerBinding) videoPlayer = videoPlayerBinding;
			if (videoPlayer == null || videoPlayer.clip == null) return;
			videoPlayer.playbackSpeed = (float)playable.GetSpeed();
			videoPlayer.targetCameraAlpha = info.weight;

			if (Application.isPlaying) {
				for (ushort i = 0; i < videoPlayer.clip.audioTrackCount; ++i) {
					if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
						videoPlayer.SetDirectAudioVolume(i, info.weight);
					else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource) {
						AudioSource audioSource = videoPlayer.GetTargetAudioSource(i);
						if (audioSource != null) audioSource.volume = info.weight;
					}
				}
			}
		}

		public override void OnGraphStart(Playable playable) {
			//Debug.Log($"[{nameof(OnGraphStart)}] videoPlayer null? {videoClip == null}");
			playedOnce = false;
		}

		public override void OnGraphStop(Playable playable) {
			//Debug.Log($"[{nameof(OnGraphStop)}] videoPlayer null? {videoClip == null}");
			if (!Application.isPlaying) StopVideo();
		}

		public override void OnPlayableDestroy(Playable playable) {
			StopVideo();
		}

		public void PlayVideo() {
			//Debug.Log($"[{nameof(PlayVideo)}] videoPlayer null? {videoClip == null}");

			if (videoPlayer == null) return;

			videoPlayer.Play();
			preparing = false;

			if (!Application.isPlaying) PauseVideo();
		}

		public void PauseVideo() {
			//Debug.Log($"[{nameof(PauseVideo)}] videoPlayer null? {videoClip == null}");

			if (videoPlayer == null) return;

			videoPlayer.Pause();
			preparing = false;
		}

		public void StopVideo() {
			//Debug.Log($"[{nameof(StopVideo)}] videoPlayer null? {videoClip == null}");


			if (videoPlayer == null) return;

			playedOnce = false;
			videoPlayer.Stop();
			preparing = false;
		}

		private void SyncVideoToPlayable(Playable playable) {

			if (videoPlayer == null || videoPlayer.clip == null) return;
			//Debug.Log($"[{nameof(SyncVideoToPlayable)}] videoPlayer null? {videoClip == null} {playable.GetTime() * videoPlayer.playbackSpeed % videoPlayer.clip.length}");
			if (!videoPlayer.isPrepared) {
				//Debug.LogWarning("Sync hasn't prepared yet");
				PrepareVideo();
                return;
            }
            var time = playable.GetTime() * videoPlayer.playbackSpeed % videoPlayer.clip.length;
            videoPlayer.time = time;
            //videoPlayer.SmartSeek(time);
		}
    }
}