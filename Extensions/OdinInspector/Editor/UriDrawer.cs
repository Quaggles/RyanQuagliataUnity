#if UNITY_EDITOR && ODIN_INSPECTOR

using System;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace RyanQuagliata.PluginExtensions.OdinInspector.Drawers {
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