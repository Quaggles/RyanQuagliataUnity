using UnityEditor;
using UnityEngine;

namespace RyanQuagliataUnity.MaterialPropertyDrawer {
	public abstract class VectorDrawer : UnityEditor.MaterialPropertyDrawer {
		public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor) {
			// Setup
			var value = prop.vectorValue;

			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMixedValue;

			// Show the toggle control
			var cacheLabel = EditorGUIUtility.labelWidth;
			var cacheField = EditorGUIUtility.fieldWidth;
			float total = position.width;
			EditorGUIUtility.labelWidth = Mathf.Ceil(0.45f * total);
			EditorGUIUtility.fieldWidth = Mathf.Ceil(0.55f * total);
			value = DrawField(position, label, value);
			EditorGUIUtility.labelWidth = cacheLabel;
			EditorGUIUtility.fieldWidth = cacheField;

			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck()) {
				// Set the new value if it has changed
				prop.vectorValue = value;
			}
		}


		public abstract Vector4 DrawField(Rect position, string label, Vector4 value);
	}

	public class Vector2Drawer : VectorDrawer {
		public override Vector4 DrawField(Rect position, string label, Vector4 value) => EditorGUI.Vector2Field(position, label, value);
	}

	public class Vector3Drawer : VectorDrawer {
		public override Vector4 DrawField(Rect position, string label, Vector4 value) => EditorGUI.Vector3Field(position, label, value);
	}
}