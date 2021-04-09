using System;
using TMPro;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.TextMeshPro.Effects {
	[Serializable]
	public class TextWarpCircle : ITextWarpFunction {
		/// <summary>
		/// The radius of the text circle arc
		/// </summary>
		[Tooltip("The radius of the text circle arc")]
		public float Radius = 10.0f;

		/// <summary>
		/// How much degrees the text arc should span
		/// </summary>
		[Tooltip("How much degrees the text arc should span")]
		public float ArcDegrees = 90.0f;

		/// <summary>
		/// The angular offset at which the arc should be centered, in degrees.
		/// -90 degrees means that the text is centered on the heighest point
		/// </summary>
		[Tooltip("The angular offset at which the arc should be centered, in degrees")]
		public float AngularOffset = -90;

		/// <summary>
		/// Computes the transformation matrix that maps the offsets of the vertices of each single character from
		/// the character's center to the final destinations of the vertices so that the text follows a curve
		/// </summary>
		/// <param name="charMidBaselinePosfloat">Position of the central point of the character</param>
		/// <param name="zeroToOnePos">Horizontal position of the character relative to the bounds of the box, in a range [0, 1]</param>
		/// <param name="textInfo">Information on the text that we are showing</param>
		/// <param name="charIdx">Index of the character we have to compute the transformation for</param>
		/// <returns>Transformation matrix to be applied to all vertices of the text</returns>
		public Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, float zeroToOnePos, TMP_TextInfo textInfo, int charIdx) {
			//compute the angle at which to show this character.
			//We want the string to be centered at the top point of the circle, so we first convert the position from a range [0, 1]
			//to a [-0.5, 0.5] one and then add m_angularOffset degrees, to make it centered on the desired point
			float angle = ((zeroToOnePos - 0.5f) * ArcDegrees + AngularOffset) * Mathf.Deg2Rad; //we need radians for sin and cos

			//compute the coordinates of the new position of the central point of the character. Use sin and cos since we are on a circle.
			//Notice that we have to do some extra calculations because we have to take in count that text may be on multiple lines
			float x0 = Mathf.Cos(angle);
			float y0 = Mathf.Sin(angle);
			float radiusForThisLine = Radius - textInfo.lineInfo[0].lineExtents.max.y * textInfo.characterInfo[charIdx].lineNumber;
			Vector2 newMideBaselinePos = new Vector2(x0 * radiusForThisLine, -y0 * radiusForThisLine); //actual new position of the character

			//compute the trasformation matrix: move the points to the just found position, then rotate the character to fit the angle of the curve 
			//(-90 is because the text is already vertical, it is as if it were already rotated 90 degrees)
			return Matrix4x4.TRS(new Vector3(newMideBaselinePos.x, newMideBaselinePos.y, 0),
				Quaternion.AngleAxis(-Mathf.Atan2(y0, x0) * Mathf.Rad2Deg - 90, Vector3.forward), Vector3.one);
		}
	}
}