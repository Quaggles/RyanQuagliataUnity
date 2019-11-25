#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using System.Reflection;

namespace RyanQuagliataUnity.Extensions.OdinInspector.Editor {
	public class MemberInfoDrawer<T> : OdinValueDrawer<T> where T : MemberInfo {
		protected override void DrawPropertyLayout(GUIContent label) {
			var memberInfo = Property.ValueEntry.WeakSmartValue as MemberInfo;

			UnityEditor.EditorGUILayout.TextArea($".{memberInfo?.Name}");

			CallNextDrawer(label);
		}
	}
}
#endif