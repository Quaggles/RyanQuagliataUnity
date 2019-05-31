using UnityEditor;
using UnityEngine;

namespace RyanQuagliata.Scripts.Editor {
	public static class TransformTools {
		[System.Serializable]
		public struct TransformStruct {
			public Vector3 position;
			public Quaternion rotation;
			public Vector3 localScale;
		}

		#region Reset Transform Hotkeys
		[MenuItem("GameObject/Transform Tools/Reset Transform Local %&q")]
		private static void ResetTransformLocal() {
			if (Selection.activeTransform == null) return;
			Undo.RegisterCompleteObjectUndo(Selection.activeTransform, $"reset local transform");
			ResetPositionLocal();
			ResetRotationLocal();
			ResetScaleLocal();
		}
	
		[MenuItem("GameObject/Transform Tools/Reset Position Local %&w")]
		private static void ResetPositionLocal() {
			if (Selection.activeTransform == null) return;
			Undo.RegisterCompleteObjectUndo(Selection.activeTransform, $"reset local position");
			Selection.activeTransform.localPosition = Vector3.zero;
		}
	
		[MenuItem("GameObject/Transform Tools/Reset Rotation Local %&e")]
		private static void ResetRotationLocal() {
			if (Selection.activeTransform == null) return;
			Undo.RegisterCompleteObjectUndo(Selection.activeTransform, $"reset local rotation");
			Selection.activeTransform.localRotation = Quaternion.identity;
		}
	
		[MenuItem("GameObject/Transform Tools/Reset Scale Local %&r")]
		private static void ResetScaleLocal() {
			if (Selection.activeTransform == null) return;
			Undo.RegisterCompleteObjectUndo(Selection.activeTransform, $"reset local scale");
			Selection.activeTransform.localScale = Vector3.one;
		}
	
		[MenuItem("GameObject/Transform Tools/Reset Transform World %#&q")]
		private static void ResetTransformWorld() {
			if (Selection.activeTransform == null) return;
			Undo.RegisterCompleteObjectUndo(Selection.activeTransform, $"reset world transform");
			ResetPositionWorld();
			ResetRotationWorld();
			ResetScaleWorld();
		}
	
		[MenuItem("GameObject/Transform Tools/Reset Position Local %#&w")]
		private static void ResetPositionWorld() {
			if (Selection.activeTransform == null) return;
			Undo.RegisterCompleteObjectUndo(Selection.activeTransform, $"reset world position");
			Selection.activeTransform.position = Vector3.zero;
		}
	
		[MenuItem("GameObject/Transform Tools/Reset Rotation Local %#&e")]
		private static void ResetRotationWorld() {
			if (Selection.activeTransform == null) return;
			Undo.RegisterCompleteObjectUndo(Selection.activeTransform, $"reset world rotation");
			Selection.activeTransform.rotation = Quaternion.identity;
		}
	
		[MenuItem("GameObject/Transform Tools/Reset Scale Local %#&r")]
		private static void ResetScaleWorld() {
			if (Selection.activeTransform == null) return;
			Undo.RegisterCompleteObjectUndo(Selection.activeTransform, $"reset world scale");
			Selection.activeTransform.localScale = Vector3.one;
		}
		#endregion

		#region Copy/Paste Transform Hotkeys
		[MenuItem("GameObject/Transform Tools/Copy Transform Local %&c")]
		private static void CopyTransformActionLocal() {
			CopyTransformAction();
		}

		[MenuItem("GameObject/Transform Tools/Copy Transform World %#&c")]
		private static void CopyTransformActionWorld() {
			CopyTransformAction(true);
		}

		[MenuItem("GameObject/Transform Tools/Paste Transform Local %&v")]
		private static void PasteTransformLocal() {
			PasteTransformAction();
		}

		[MenuItem("GameObject/Transform Tools/Paste Transform World %#&v")]
		private static void PasteTransformWorld() {
			PasteTransformAction(true);
		}
		#endregion
	
		#region Copy/Paste Transform Functions

		private static void CopyTransformAction(bool worldSpace = false){
			if (Selection.activeTransform == null){
				EditorGUIUtility.systemCopyBuffer = "";
				return;
			}

			// Store the selected transform in struct
			TransformStruct storedTransform;
			storedTransform = worldSpace ? CopyTransformWorld(Selection.activeTransform) : CopyTransformLocal(Selection.activeTransform);

			// Serialize and save to clipboard
			EditorGUIUtility.systemCopyBuffer = JsonUtility.ToJson(storedTransform);
		}

		private static void PasteTransformAction(bool worldSpace = false){
			if (Selection.activeTransform == null) return;
			Undo.RegisterCompleteObjectUndo(Selection.activeGameObject, $"pasted transform values");
			TransformStruct storedTransform = new TransformStruct();

			// Attempt to parse JSON
			try {
				storedTransform = JsonUtility.FromJson<TransformStruct>(EditorGUIUtility.systemCopyBuffer);
				if (worldSpace) {
					PasteTransformWorld(Selection.activeTransform, storedTransform);
				} else {
					PasteTransformLocal(Selection.activeTransform, storedTransform);
				}
			} catch {
			}
		}

		private static TransformStruct CopyTransformWorld(Transform transform){
			TransformStruct stored = new TransformStruct();
			stored.position = transform.position;
			stored.rotation = transform.rotation;
			stored.localScale = transform.localScale;
			return stored;
		}

		private static TransformStruct CopyTransformLocal(Transform transform){
			TransformStruct stored = new TransformStruct();
			stored.position = transform.localPosition;
			stored.rotation = transform.localRotation;
			stored.localScale = transform.localScale;
			return stored;
		}

		private static void PasteTransformWorld(Transform transform, TransformStruct stored){
			transform.position = stored.position;
			transform.rotation = stored.rotation;
			transform.localScale = stored.localScale;
		}

		private static void PasteTransformLocal(Transform transform, TransformStruct stored){
			transform.localPosition = stored.position;
			transform.localRotation = stored.rotation;
			transform.localScale = stored.localScale;
		}
		#endregion
	}
}