#if ODIN_INSPECTOR && !ODIN_EDITOR_ONLY
using System.Collections.Generic;
using System.Linq;
using RyanQuagliata.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliata.GlobalShaderPropertySetter {
	[ExecuteInEditMode]
	public class GlobalShaderPropertySetterHost : SerializedMonoBehaviour {
		public bool SetInEditMode;
		
		public bool SetInPlayMode = true;

		[Space]
		[TableList]
		[InfoBox("Some properties share the same shader property name and will override eachother", InfoMessageType.Warning,
			nameof(ValidProperties))]
		public List<IGlobalShaderPropertySettable> Properties = new List<IGlobalShaderPropertySettable>();

		private bool ValidProperties() {
			var allPropertiesCount = Properties.Count(x => !string.IsNullOrWhiteSpace(x.GlobalShaderPropertyName));
			var distinctPropertiesCount = Properties.Where(x => !string.IsNullOrWhiteSpace(x.GlobalShaderPropertyName))
				.DistinctBy(x => x?.GlobalShaderPropertyName ?? "").Count();
			var result = Properties != null && allPropertiesCount != distinctPropertiesCount;
#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
#endif
			return result;
		}

		public void Start() {
			UpdatePropertyPreviews();
		}

		public void Update() {
			if (Application.isPlaying && SetInPlayMode) SetProperties();
			if (!Application.isPlaying && SetInEditMode) SetProperties();
		}

		[Button("Set Properties Manually")]
		public void SetProperties() {
			for (int i = 0; i < Properties.Count; i++) {
				Properties[i].Set();
			}
		}

		[Button]
		public void UpdatePropertyPreviews() {
			for (int i = 0; i < Properties.Count; i++) {
				Properties[i].UpdateValuePreview();
			}
		}
	}
}
#endif