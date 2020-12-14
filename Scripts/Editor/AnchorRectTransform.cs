using UnityEditor;
using UnityEngine;

namespace RyanQuagliataUnity.Editor {
	public static class AnchorRectTransform {
		/// <summary>
		/// Sets the anchors of the RectTransform so it will keep the same relative size when the parent is changed
		/// </summary>
		[MenuItem("CONTEXT/RectTransform/Anchor Rect Transform")]
		public static void Anchor() {
			var gameObject = Selection.activeGameObject;
			RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
			if (!gameObject || !rectTransform) return;
			var parentRectTransform = gameObject.transform.parent.GetComponent<RectTransform>();
			if (!parentRectTransform) return;

			var offsetMin = rectTransform.offsetMin;
			var offsetMax = rectTransform.offsetMax;
			var _anchorMin = rectTransform.anchorMin;
			var _anchorMax = rectTransform.anchorMax;

			var parent_width = parentRectTransform.rect.width;
			var parent_height = parentRectTransform.rect.height;

			var anchorMin = new Vector2(_anchorMin.x + (offsetMin.x / parent_width),
				_anchorMin.y + (offsetMin.y / parent_height));
			var anchorMax = new Vector2(_anchorMax.x + (offsetMax.x / parent_width),
				_anchorMax.y + (offsetMax.y / parent_height));

			rectTransform.anchorMin = anchorMin;
			rectTransform.anchorMax = anchorMax;

			rectTransform.offsetMin = new Vector2(0, 0);
			rectTransform.offsetMax = new Vector2(0, 0);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
		}
	}
}