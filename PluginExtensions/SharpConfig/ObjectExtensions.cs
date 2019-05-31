using UnityEngine;

namespace RyanQuagliata.PluginExtensions.SharpConfig {
	public static class ObjectExtensions {
		public static void InstantiateAndConfigure(this UnityEngine.Object that) {
			Object.Instantiate(that);
		}
	}
}