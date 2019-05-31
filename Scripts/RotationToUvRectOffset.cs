using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Project {
	public class RotationToUvRectOffset : MonoBehaviour {
		public RawImage RawImage;

		[Range(0, 360)]
		public float Offset = 0;

        public bool UseLocalRotation = false;

		// Update is called once per frame
		void Update() {
			if (transform.hasChanged) {
                Apply();
                transform.hasChanged = false;

            }
		}
		
        [Button]
        void Apply() {
            var rotation = UseLocalRotation ? transform.localRotation : transform.rotation;
            RawImage.uvRect = new Rect((rotation.eulerAngles.y + Offset) % 360f / 360f, 0, 1, 1);
        }
	}
}