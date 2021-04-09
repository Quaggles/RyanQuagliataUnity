// using TMPro;
// using UnityEngine;
//
// namespace RyanQuagliataUnity.Extensions.TextMeshPro.Effects {
//	[Serializable]
// 	public class TextWarpExp : ITextWarpFunction {
// 		/// <summary>
// 		/// The base of the exponential curve
// 		/// </summary>
// 		[Min(float.Epsilon)]
// 		[Tooltip("The base of the exponential curve")]
// 		public float ExpBase = 1.3f;
//
// 		/// <summary>
// 		/// Computes the transformation matrix that maps the offsets of the vertices of each single character from
// 		/// the character's center to the final destinations of the vertices so that the text follows a curve
// 		/// </summary>
// 		/// <param name="charMidBaselinePosfloat">Position of the central point of the character</param>
// 		/// <param name="zeroToOnePos">Horizontal position of the character relative to the bounds of the box, in a range [0, 1]</param>
// 		/// <param name="textInfo">Information on the text that we are showing</param>
// 		/// <param name="charIdx">Index of the character we have to compute the transformation for</param>
// 		/// <returns>Transformation matrix to be applied to all vertices of the text</returns>
// 		public Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, float zeroToOnePos, TMP_TextInfo textInfo, int charIdx) {
// 			//compute the coordinates of the new position of the central point of the character. Use the exp function
// 			//Notice that we have to do some extra calculations because we have to take in count that text may be on multiple lines
// 			float x0 = charMidBaselinePos.x;
// 			float y0 = Mathf.Pow(ExpBase, x0);
// 			Vector2 newMideBaselinePos =
// 				new Vector2(x0,
// 					y0 - textInfo.lineInfo[0].lineExtents.max.y * textInfo.characterInfo[charIdx].lineNumber); //actual new position of the character
//
// 			//compute the trasformation matrix: move the points to the just found position, then rotate the character to fit the angle of the curve 
// 			//(I do some calculations using derivative of the exp function for the orientation)
// 			return Matrix4x4.TRS(new Vector3(newMideBaselinePos.x, newMideBaselinePos.y, 0),
// 				Quaternion.AngleAxis(Mathf.Atan(Mathf.Log(ExpBase) * y0) * Mathf.Rad2Deg, Vector3.forward), Vector3.one);
// 		}
// 	}
// }