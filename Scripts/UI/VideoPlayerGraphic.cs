using RyanQuagliataUnity.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace RyanQuagliataUnity.UI {
    public class VideoPlayerGraphic : RawImage {
        [Required]
        public VideoPlayer VideoPlayer;

        // Start is called before the first frame update
        protected override void Awake() {
            base.Awake();
            if (!Application.isPlaying && !VideoPlayer) VideoPlayer = GetComponent<VideoPlayer>();

            // Ensure the VideoPlayer is rendering in API only so we can grab the video texture
            if (VideoPlayer) VideoPlayer.renderMode = VideoRenderMode.APIOnly;
            
            // Set the Raw Image to transparent so we don't have a big white square in the UI 
            this.SetAlpha(0);
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