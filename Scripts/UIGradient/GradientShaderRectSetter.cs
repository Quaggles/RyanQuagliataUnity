using RyanQuagliataUnity.Extensions;
using RyanQuagliataUnity.MaterialModifier;
using UnityEngine;

namespace RyanQuagliataUnity.UIGradient {
	[ExecuteInEditMode]
	[RequireComponent(typeof(TransformHasChangedResetter))]
#if ODIN_INSPECTOR
	[Sirenix.OdinInspector.HideMonoScript]
#endif
	public class GradientShaderRectSetter : EnhancedMonoBehaviour {
#if ODIN_INSPECTOR
		[Sirenix.OdinInspector.Required]
#endif
		public MaterialModifierVector Position;

#if ODIN_INSPECTOR
		[Sirenix.OdinInspector.Required]
#endif
		public MaterialModifierVector Size;

		private Canvas canvas;

		void Start() => Set();

		//private void OnRectTransformDimensionsChange() => Set();

		void Update() {
			if (transform.hasChanged) Set();
		}

		void Set() {
			if (!canvas || !transform.HasParent(canvas.transform)) canvas = GetComponentInParent<Canvas>();
			// hacky slow method TODO: Find a faster way to calculate "CanvasSpace" position
			var cam = canvas?.worldCamera ?? Camera.current;
			if (Position && canvas && cam) {
				RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, cam.WorldToScreenPoint(RectTransform.position), cam,
					out var pos);
				Position.SetValue2Temporary(pos);
			}

			if (Size) Size.SetValue2Temporary(RectTransform.rect.size);
		}
	}
}