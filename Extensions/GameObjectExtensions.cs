using UnityEngine;

namespace RyanQuagliata.Extensions {
	public static class GameObjectExtensions {
		public static void SetLayer(this GameObject that, int layer) {
			that.layer = layer;
		}

		public static void SetLayerRecursive(this GameObject that, int layer) {
			that.SetLayer(layer);
			for (var i = 0; i < that.transform.childCount; i++) {
				that.transform.GetChild(i).gameObject.SetLayerRecursive(layer);
			}
		}
	}
}