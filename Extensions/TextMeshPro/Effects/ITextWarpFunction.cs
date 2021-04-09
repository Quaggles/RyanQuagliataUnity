using TMPro;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.TextMeshPro.Effects {
	public interface ITextWarpFunction {
		/// <summary>
		/// Computes the transformation matrix that maps the offsets of the vertices of each single character from
		/// the character's center to the final destinations of the vertices so that the text follows a curve
		/// </summary>
		/// <param name="charMidBaselinePosfloat">Position of the central point of the character</param>
		/// <param name="zeroToOnePos">Horizontal position of the character relative to the bounds of the box, in a range [0, 1]</param>
		/// <param name="textInfo">Information on the text that we are showing</param>
		/// <param name="charIdx">Index of the character we have to compute the transformation for</param>
		/// <returns>Transformation matrix to be applied to all vertices of the text</returns>
		public Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, float zeroToOnePos, TMP_TextInfo textInfo, int charIdx);
	}
}