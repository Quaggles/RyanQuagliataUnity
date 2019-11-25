#if UNITY_EDITOR && ODIN_INSPECTOR
using UnityEditor;
using UnityEngine;
using System.Net;
using Sirenix.OdinInspector.Editor;

namespace RyanQuagliataUnity.Extensions.OdinInspector.Editor {
	[OdinDrawer]
	public class NetworkCredentialDrawer : OdinValueDrawer<NetworkCredential> {
		protected override void DrawPropertyLayout(IPropertyValueEntry<NetworkCredential> entry, GUIContent label) {
			var networkCredential = entry.SmartValue;


			if (label != null) {
				EditorGUILayout.PrefixLabel(label);
			}
			entry.SmartValue.UserName = EditorGUILayout.TextField(networkCredential.UserName);
			entry.SmartValue.Password = EditorGUILayout.TextField(networkCredential.Password);
		}
	}
}
#endif