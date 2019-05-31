using System;
using System.Linq;
using System.Reflection;
using RyanQuagliata.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace RyanQuagliata {
	public abstract class EnhancedMonoBehaviour : MonoBehaviour {
		public enum GizmoDrawModeType {
			Disabled,
			Selected,
			Always
		}

		private Transform transformCached;
		private RectTransform rectTransformCached;

// If in the editor automatically grab it if it doesn't exist (For edit mode), to save performance don't do this in builds
//#if UNITY_EDITOR
		public Transform Transform => transformCached ? transformCached : (transformCached = transform);
		public RectTransform RectTransform =>
			rectTransformCached != null
				? rectTransformCached
				: (rectTransformCached = transform as RectTransform);
//#else
//		public Transform Transform => transformCached;
//		public RectTransform RectTransform => rectTransformCached;
//#endif

		// Detect if the gizmo controls should be shown
#if UNITY_EDITOR
		private bool UserGizmosOverridden => GetType().GetMethod(nameof(DrawGizmosUserSelected), BindingFlags.NonPublic | BindingFlags.Instance).IsOverriden();
#else
		private bool UserGizmosOverridden => false;
#endif
		
		[ShowIf(nameof(UserGizmosOverridden)), EnumToggleButtons, OnValueChanged("ForceSceneUpdate")]
		public GizmoDrawModeType GizmoDrawMode = GizmoDrawModeType.Selected;

		public bool GizmosBeingDrawn {
			get {
#if UNITY_EDITOR
				switch (GizmoDrawMode) {
					case GizmoDrawModeType.Disabled:
						return false;
					case GizmoDrawModeType.Selected:
						return Selection.gameObjects?.Contains(this.gameObject) ?? false;
					case GizmoDrawModeType.Always:
						return true;
					default:
						throw new ArgumentOutOfRangeException();
				}
#else
				return false;
#endif
			}
		}

		protected virtual void Awake() {
			transformCached = transform;
			rectTransformCached = transform as RectTransform;
		}

		// Disabled in builds to save performance from Unity Event Calls
#if UNITY_EDITOR
		protected void OnDrawGizmos() {
			if (GizmoDrawMode == GizmoDrawModeType.Always) DrawGizmosUserSelected();
		}

		protected void OnDrawGizmosSelected() {
			if (GizmoDrawMode == GizmoDrawModeType.Selected) DrawGizmosUserSelected();
		}

		protected virtual void DrawGizmosUserSelected() { }

		private void ForceSceneUpdate() {
			SceneView.RepaintAll();
		}
#endif
	}
}