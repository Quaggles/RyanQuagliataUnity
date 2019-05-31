using UnityEngine;

namespace RyanQuagliata.Extensions {
	/// <summary>
	/// SkinnedMeshRendererExtetensions
	/// </summary>
	public static class SkinnedMeshRendererExtensions {
		/// <summary>
		/// Searches through the blend shape properties and returns the index of the blendshape with the supplied name,
		/// returns -1 if the name couldn't be found
		/// </summary>
		/// <param name="head"></param>
		/// <param name="name">Name of the blend shape to search for</param>
		/// <returns></returns>
		public static int GetIndexBlendShapeName(this SkinnedMeshRenderer head, string name) {
			var mesh = head.sharedMesh;
			for (int i = 0; i < GetBlendShapeCount(head); i++) {
				if (mesh.GetBlendShapeName(i).Equals(name))
					return i;
			}
			return -1;
		}

		/// <summary>
		/// How many blend shapes are there?
		/// </summary>
		/// <returns>int: number of blend shapes</returns>
		public static int GetBlendShapeCount(this SkinnedMeshRenderer head) {
			return head.sharedMesh.blendShapeCount;
		}
	}
}