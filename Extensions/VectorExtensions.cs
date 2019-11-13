using RyanQuagliataUnity.Utilities;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class Vector {
		/// <summary>
		/// Converts a radian to a Vector 2
		/// </summary>
		/// <param name="radian"> Radian float </param>
		/// <param name="twelveOClockNorth"> Default radian circle defines 0 as {1,0}, this makes 0 radians {0, 1} (Twelve O'Clock) </param>
		/// <returns> Vector 2 representation of radian </returns>
		public static Vector2 RadianToVector2(float radian, bool twelveOClockNorth = false) {
			if (twelveOClockNorth)
				radian+=UnityEngine.Mathf.PI/2;
			return new Vector2(UnityEngine.Mathf.Cos(radian), UnityEngine.Mathf.Sin(radian));
		}

		/// <summary>
		/// Converts an angle to a Vector 2
		/// </summary>
		/// <param name="angle"> angle float </param>
		/// <param name="twelveOClockNorth"> Default circle defines 0 as {1,0}, this makes 0 radians {0, 1} (Twelve O'Clock) </param>
		/// <returns> Vector 2 representation of degree </returns>
		public static Vector2 DegreeToVector2(float angle, bool twelveOClockNorth = false) {
			return RadianToVector2(angle * UnityEngine.Mathf.Deg2Rad, twelveOClockNorth);
		}

		public static Vector2 Pow(Vector2 vector, float power){
			vector.x = UnityEngine.Mathf.Pow(vector.x, power);
			vector.y = UnityEngine.Mathf.Pow(vector.y, power);
			return vector;
		}

		public static Vector3 Pow(Vector3 vector, float power){
			vector.x = UnityEngine.Mathf.Pow(vector.x, power);
			vector.y = UnityEngine.Mathf.Pow(vector.y, power);
			vector.z = UnityEngine.Mathf.Pow(vector.z, power);
			return vector;
		}

		public static Vector3 Log(Vector3 vector, float power){
			vector.x = UnityEngine.Mathf.Log(vector.x, power);
			vector.y = UnityEngine.Mathf.Log(vector.y, power);
			vector.z = UnityEngine.Mathf.Log(vector.z, power);
			return vector;
		}

		public static Vector3 MaxMagnitude(Vector3 a, Vector3 b){
			if (a.magnitude > b.magnitude){
				return a;
			}
			return b;
		}

		public static Vector3 Max(Vector3 a, Vector3 b){
			Vector3 returnVector;
			returnVector.x = UnityEngine.Mathf.Max(a.x, b.x);
			returnVector.y = UnityEngine.Mathf.Max(a.y, b.y);
			returnVector.z = UnityEngine.Mathf.Max(a.z, b.z);
			return returnVector;
		}

		public static Vector3 Min(Vector3 a, Vector3 b){
			Vector3 returnVector;
			returnVector.x = UnityEngine.Mathf.Min(a.x, b.x);
			returnVector.y = UnityEngine.Mathf.Min(a.y, b.y);
			returnVector.z = UnityEngine.Mathf.Min(a.z, b.z);
			return returnVector;
		}

		public static Vector2 PushApartRange(Vector3 a, Vector3 b){
			Vector2 returnVector;
			returnVector.x = UnityEngine.Mathf.Min(a.x, b.x);
			returnVector.y = UnityEngine.Mathf.Max(a.y, b.y);
			return returnVector;
		}

		public static Vector3 MinMagnitude(Vector3 a, Vector3 b){
			if (a.magnitude < b.magnitude){
				return a;
			}
			return b;
		}

		public static float AverageVec2(Vector2 vec){
			float total = 0;
			total += vec.x;
			total += vec.y;
			return total/2;
		}

		public static float AverageVec3(Vector3 vec){
			float total = 0;
			total += vec.x;
			total += vec.y;
			total += vec.z;
			return total/3;
		}
		
		/// <summary>
		/// Component wise divides a vector
		/// </summary>
		/// <param name="that"></param>
		/// <param name="divide"></param>
		public static Vector2 Divide(this Vector2 that, Vector2 divide) {
			that.x /= divide.x;
			that.y /= divide.y;
			return that;
		}
		
		/// <summary>
		/// Component wise divides a vector
		/// </summary>
		/// <param name="that"></param>
		/// <param name="divide"></param>
		public static Vector3 Divide(this Vector3 that, Vector3 divide) {
			that.x /= divide.x;
			that.y /= divide.y;
			that.z /= divide.z;
			return that;
		}		


		/// <summary>
		/// Clamps the component values to the provided range
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static Vector2 ClampComponents(this Vector2 value, float min, float max) {
			value.x = UnityEngine.Mathf.Clamp(value.x, min, max);
			value.y = UnityEngine.Mathf.Clamp(value.y, min, max);
			return value;
		}		
		
		/// <summary>
		/// Clamps the component values to the provided range
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Vector2 Clamp01(this Vector2 value) {
			value.x = UnityEngine.Mathf.Clamp(value.x, 0, 1);
			value.y = UnityEngine.Mathf.Clamp(value.y, 0, 1);
			return value;
		}
		
		/// <summary>
		/// Clamps the component values to the provided range
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static Vector3 ClampComponents(this Vector3 value, float min, float max) {
			value.x = UnityEngine.Mathf.Clamp(value.x, min, max);
			value.y = UnityEngine.Mathf.Clamp(value.y, min, max);
			value.z = UnityEngine.Mathf.Clamp(value.z, min, max);
			return value;
		}
		
		/// <summary>
		/// Clamps the component values to the provided range
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static Vector2 ClampComponents(this Vector2 value, Vector2 min, Vector2 max) {
			value.x = UnityEngine.Mathf.Clamp(value.x, min.x, max.x);
			value.y = UnityEngine.Mathf.Clamp(value.y, min.y, max.y);
			return value;
		}
		
		/// <summary>
		/// Clamps the component values to the provided range
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static Vector3 ClampComponents(this Vector3 value, Vector3 min, Vector3 max) {
			value.x = UnityEngine.Mathf.Clamp(value.x, min.x, max.x);
			value.y = UnityEngine.Mathf.Clamp(value.y, min.y, max.y);
			value.z = UnityEngine.Mathf.Clamp(value.z, min.z, max.z);
			return value;
		}
		
		/// <summary>
		/// Clamps the component values to the provided range
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Vector3 Clamp01(this Vector3 value) {
			value.x = UnityEngine.Mathf.Clamp(value.x, 0, 1);
			value.y = UnityEngine.Mathf.Clamp(value.y, 0, 1);
			value.z = UnityEngine.Mathf.Clamp(value.z, 0, 1);
			return value;
		}

		public static Vector2Int Round(this Vector2 vec){
			Vector2Int returnVal = Vector2Int.zero;
			returnVal.x = UnityEngine.Mathf.RoundToInt(vec.x);
			returnVal.y = UnityEngine.Mathf.RoundToInt(vec.y);
			return returnVal;
		}

		public static Vector3Int Round(this Vector3 vec) {
			Vector3Int returnVal = Vector3Int.zero;
			returnVal.x = UnityEngine.Mathf.RoundToInt(vec.x);
			returnVal.y = UnityEngine.Mathf.RoundToInt(vec.y);
			returnVal.z = UnityEngine.Mathf.RoundToInt(vec.z);
			return returnVal;
		}

		public static float Average(this Vector2 that) {
			float total = 0;
			total += that.x;
			total += that.y;
			return total / 2f;
		}
		public static float Average(this Vector3 that) {
			float total = 0;
			total += that.x;
			total += that.y;
			total += that.z;
			return total / 3f;
		}
		
		public static Vector2 Remap(this Vector2 that, Vector2 iMin, Vector2 iMax, Vector2 oMin, Vector2 oMax) {
			that.x = that.x.Remap(iMin.x, iMax.x, oMin.x, oMax.x);
			that.y = that.y.Remap(iMin.y, iMax.y, oMin.y, oMax.y);
			return that;
		}
		
		public static Vector2 Remap(this Vector2 that, float iMin, float iMax, float oMin, float oMax) {
			that.x = that.x.Remap(iMin, iMax, oMin, oMax);
			that.y = that.y.Remap(iMin, iMax, oMin, oMax);
			return that;
		}
		
		public static Vector2 Remap(this Vector2 that, Rect i, Rect o) {
			return that.Remap(i.min, i.max, o.min, o.max);
		}
		
		public static Vector3 Remap(this Vector3 that, Vector3 iMin, Vector3 iMax, Vector3 oMin, Vector3 oMax) {
			that.x = that.x.Remap(iMin.x, iMax.x, oMin.x, oMax.x);
			that.y = that.y.Remap(iMin.y, iMax.y, oMin.y, oMax.y);
			that.z = that.z.Remap(iMin.z, iMax.z, oMin.z, oMax.z);
			return that;
		}		
		
		public static Vector3 Remap(this Vector3 that, float iMin, float iMax, float oMin, float oMax) {
			that.x = that.x.Remap(iMin, iMax, oMin, oMax);
			that.y = that.y.Remap(iMin, iMax, oMin, oMax);
			that.z = that.z.Remap(iMin, iMax, oMin, oMax);
			return that;
		}

		/// <summary>
		/// Returns true if any component of this Vector is NaN
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static bool IsNaN(this Vector2 that) {
			if (float.IsNaN(that.x)) return true;
			if (float.IsNaN(that.y)) return true;
			return false;
		}
		
		/// <summary>
		/// Returns a vector with all NaN components set to 0
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static Vector2 IsNaNThen0(this Vector2 that) => that.IsNaNThenSet(0);
		
		/// <summary>
		/// Returns a vector with all NaN components set to a value
		/// </summary>
		/// <param name="that"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Vector2 IsNaNThenSet(this Vector2 that, float value) {
			if (float.IsNaN(that.x)) that.x = value;
			if (float.IsNaN(that.y)) that.y = value;
			return that;
		}

		/// <summary>
		/// Returns true if any component of this Vector is NaN
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static bool IsNaN(this Vector3 that) {
			if (float.IsNaN(that.x)) return true;
			if (float.IsNaN(that.y)) return true;
			if (float.IsNaN(that.z)) return true;
			return false;
		}

		/// <summary>
		/// Returns a vector with all NaN components set to 0
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static Vector3 IsNaNThen0(this Vector3 that) => that.IsNaNThenSet(0);

		/// <summary>
		/// Returns a vector with all NaN components set to a value
		/// </summary>
		/// <param name="that"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Vector3 IsNaNThenSet(this Vector3 that, float value) {
			if (float.IsNaN(that.x)) that.x = value;
			if (float.IsNaN(that.y)) that.y = value;
			if (float.IsNaN(that.z)) that.z = value;
			return that;
		}

		public static float Min(this Vector2 that) => Mathf.Min(that.x, that.y);
		public static float Min(this Vector3 that) => MathfExtensions.Min(that.x, that.y, that.z);

		public static float Max(this Vector2 that) => Mathf.Max(that.x, that.y);
		public static float Max(this Vector3 that) => MathfExtensions.Max(that.x, that.y, that.z);

		/// <summary>
		/// Are the X and Y components within 0 - 1 
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static bool XYWithin01Range(this Vector3 that) {
			if (that.x < 0 || that.x > 1) return false;
			if (that.y < 0 || that.y > 1) return false;
			return true;
		}
		
		/// <summary>
		/// Are the X, Y and Z components within 0 - 1 
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static bool XYZWithin01Range(this Vector3 that) {
			if (that.x < 0 || that.x > 1) return false;
			if (that.y < 0 || that.y > 1) return false;
			if (that.z < 0 || that.z > 1) return false;
			return true;
		}
		
		public static Vector3 XYtoXZ(this Vector2 that) => new Vector3(that.x, 0, that.y);
	}
}