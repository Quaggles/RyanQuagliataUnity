using RyanQuagliataUnity.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace RyanQuagliataUnity.UI {
    public class VideoPlayerGraphic : RawImage {
        [Required]
        public VideoPlayer VideoPlayer;

        public bool SetNativeSizeOnAwake = false;

        // Start is called before the first frame update
        protected override void Awake() {
            base.Awake();
            if (!Application.isPlaying && !VideoPlayer) VideoPlayer = GetComponent<VideoPlayer>();

            if (Application.isPlaying && SetNativeSizeOnAwake) SetNativeSize();

            // Ensure the VideoPlayer is rendering in API only so we can grab the video texture
            if (VideoPlayer) VideoPlayer.renderMode = VideoRenderMode.APIOnly;
            
            // Set the Raw Image to transparent so we don't have a big white square in the UI 
            this.SetAlpha(0);
        }

        public override void SetNativeSize() {
            base.SetNativeSize();
            if (!VideoPlayer || !VideoPlayer.clip) return;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, VideoPlayer.clip.width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, VideoPlayer.clip.height);
        }

        private void PrepareCompleted(VideoPlayer source) {
            texture = source.texture;
            color = texture == null ? Color.clear : Color.white;
        }

        protected override void OnEnable() {
            base.OnEnable();
            if (VideoPlayer) VideoPlayer.prepareCompleted += PrepareCompleted;
        }

        protected override void OnDisable() {
            base.OnDisable();
            if (VideoPlayer) VideoPlayer.prepareCompleted -= PrepareCompleted;
        }

        void Update() {
            if (VideoPlayer) this.SetAlpha(VideoPlayer.targetCameraAlpha);
        }
    }
}