#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace RyanQuagliata.PluginExtensions.OdinInspector.Drawers {
	public class MemberInfoDrawer<T> : OdinValueDrawer<T> where T : MemberInfo {
		protected override void DrawPropertyLayout(GUIContent label) {
			var memberInfo = Property.ValueEntry.WeakSmartValue as MemberInfo;

			UnityEditor.EditorGUILayout.TextArea($".{memberInfo?.Name}");

			CallNextDrawer(label);
		}
	}
}
#endif