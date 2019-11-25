
#if UNITY_EDITOR && ODIN_INSPECTOR
using UnityEditor;
using UnityEngine;
using System;
using Sirenix.OdinInspector.Editor;

namespace RyanQuagliataUnity.Extensions.OdinInspector.Editor {
	[OdinDrawer]
	public class UriDrawer : OdinValueDrawer<Uri> {
		protected override void DrawPropertyLayout(IPropertyValueEntry<Uri> entry, GUIContent label) {
			var uri = entry.SmartValue;


			if (label != null) {
				EditorGUILayout.PrefixLabel(label);
			}
			//entry.SmartValue = new Uri(EditorGUILayout.TextField(uri.ToString()));
			EditorGUILayout.ToggleLeft(nameof(uri.IsAbsoluteUri), uri.IsAbsoluteUri);
			EditorGUILayout.TextField(uri.ToString());
		}
	}
}
#endif