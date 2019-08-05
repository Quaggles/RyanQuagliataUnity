using UnityEngine;

namespace RyanQuagliataUnity.Utilities {

	public class Trigonometry {
		///<summary>
		/// Determine whether a point P is inside the triangle ABC. Note, this function
		/// assumes that P is coplanar with the triangle.
		///</summary>
		///<returns>True if the point is inside, false if it is not.</returns>
		public static bool PointInTriangle(Vector3 P, Vector3 A, Vector3 B, Vector3 C) {
			// Prepare our barycentric variables
			Vector3 u = B - A;
			Vector3 v = C - A;
			Vector3 w = P - A;

			Vector3 vCrossW = Vector3.Cross(v, w);
			Vector3 vCrossU = Vector3.Cross(v, u);

			// Test sign of r
			if (Vector3.Dot(vCrossW, vCrossU) < 0)
				return false;

			Vector3 uCrossW = Vector3.Cross(u, w);
			Vector3 uCrossV = Vector3.Cross(u, v);

			// Test sign of t
			if (Vector3.Dot(uCrossW, uCrossV) < 0)
				return false;

			// At this point, we know that r and t and both > 0.
			// Therefore, as long as their sum is <= 1, each must be less <= 1
			float denom = uCrossV.magnitude;
			float r = vCrossW.magnitude / denom;
			float t = uCrossW.magnitude / denom;


			return (r + t <= 1);
		}
	}
}