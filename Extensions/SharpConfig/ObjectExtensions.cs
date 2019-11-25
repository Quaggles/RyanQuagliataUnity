using UnityEngine;

namespace RyanQuagliataUnity.Extensions.SharpConfig {
	public static class ObjectExtensions {
		public static void InstantiateAndConfigure(this UnityEngine.Object that) {
			Object.Instantiate(that);
		}
	}
}