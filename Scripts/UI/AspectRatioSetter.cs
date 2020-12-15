using RyanQuagliataUnity.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace RyanQuagliataUnity.UI {
	[ExecuteInEditMode]
	[RequireComponent(typeof(AspectRatioFitter))]
	public class AspectRatioSetter : MonoBehaviour {
		[Required]
		public Graphic Graphic;
		
		[Required]
		public AspectRatioFitter AspectRatioFitter;

		public bool UpdateInEditMode = false;

		public bool UpdateEveryFrame = false;

		void Start() {
			if (!Application.isPlaying) {
				this.GetComponentIfNull(ref Graphic);
				this.GetComponentIfNull(ref AspectRatioFitter);
			}
		}

		void Update() {
			if (!Application.isPlaying && !UpdateInEditMode) return; 
			if (UpdateEveryFrame) UpdateRatio();
		}

		[Button]
		public void UpdateRatio() {
			if (!Graphic.mainTexture)
				AspectRatioFitter.aspectRatio = 1;
			else
				AspectRatioFitter.aspectRatio = Graphic.mainTexture.GetAspectRatio();
		}
	}
}