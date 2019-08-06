using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class LayerMaskExtensions {
		/// <summary>
		/// Finds if the LayerMask contains a layer
		/// </summary>
		/// <param name="that"></param>
		/// <param name="layer"></param>
		/// <returns></returns>
		public static bool ContainsLayer(this LayerMask that, int layer) => that == (that | (1 << layer));
	}
}