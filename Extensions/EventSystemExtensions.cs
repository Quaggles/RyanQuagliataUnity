using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RyanQuagliataUnity.Extensions {
	public static class EventSystemExtensions {
		public static void SetSelectedSelectable(this EventSystem that, Selectable selectable) => SetSelectedGameObjectForce(that, selectable.gameObject);

		public static void SetSelectedGameObjectForce(this EventSystem that, GameObject gameObject) {
			that.SetSelectedGameObject(null);
			that.SetSelectedGameObject(gameObject);
		}
	}
}