using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity {
	// ReSharper disable once InconsistentNaming
	public static class OnGuiAnywhere {
#if UNITY_EDITOR
		private static OnGuiMonoBehaviour onGuiHost;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize() {
			GetHandlesHost();
		}

		static OnGuiMonoBehaviour GetHandlesHost() {
			if (onGuiHost) return onGuiHost;
			onGuiHost = new GameObject().AddComponent<OnGuiMonoBehaviour>();
			onGuiHost.gameObject.name = $"[{nameof(OnGuiMonoBehaviour)}]";
			onGuiHost.gameObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
			if (Application.isPlaying) Object.DontDestroyOnLoad(onGuiHost);

			return onGuiHost;
		}
#endif

		public static void Draw(Action action) {
#if UNITY_EDITOR
			GetHandlesHost().GizmoActions.Add(action);
#endif
		}
		
		public static void DrawGui(Action action) {
#if UNITY_EDITOR
			GetHandlesHost().GuiActions.Add(action);
#endif
		}

		public static class Handles {
			public static void Label(Vector3 position, string label) {
#if UNITY_EDITOR
				void Action() => UnityEditor.Handles.Label(position, label);
				Draw(Action);
#endif
			}
		}

		public static class Gizmos {
			public static void DrawLine(Vector3 from, Vector3 to) {
#if UNITY_EDITOR
				void Action() => UnityEngine.Gizmos.DrawLine(from, to);
				Draw(Action);
#endif
			}
		}
	}
}