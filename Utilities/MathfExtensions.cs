using System;
using UnityEngine;

namespace RyanQuagliataUnity.Utilities {
	/// <summary>
	/// Adds remap function
	/// </summary>
	public static class MathfExtensions {
		/// <summary>
		/// Remaps a value from a previous min and max range to a new range
		/// </summary>
		/// <param name="iValue">Value to remap</param>
		/// <param name="iMin">Min value of input range</param>
		/// <param name="iMax">Max value of input range</param>
		/// <param name="oMin">Min value of output range</param>
		/// <param name="oMax">Max value of output range</param>
		/// <returns>Remapped value</returns>
		public static float Remap(float iValue, float iMin, float iMax, float oMin, float oMax) {
			var val = oMin + (iValue - iMin) * (oMax - oMin) / (iMax - iMin);
			if (float.IsNaN(val)) return 0;
			return val;
		}
		
		/// <summary>
		/// Remaps a value from a previous min and max range to a new range
		/// </summary>
		/// <param name="iValue">Value to remap</param>
		/// <param name="iMin">Min value of input range</param>
		/// <param name="iMax">Max value of input range</param>
		/// <param name="oMin">Min value of output range</param>
		/// <param name="oMax">Max value of output range</param>
		/// <returns>Remapped value</returns>
		public static double Remap(double iValue, double iMin, double iMax, double oMin, double oMax) {
			var val = oMin + (iValue - iMin) * (oMax - oMin) / (iMax - iMin);
			if (double.IsNaN(val)) return 0;
			return val;
		}

		/// <summary>
		/// Overload of Min function for 3 values to avoid overhead and allocation of using params
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		public static float Min(float a, float b, float c) => Mathf.Min(a, Mathf.Min(b, c));
		
		/// <summary>
		/// Overload of Max function for 3 values to avoid overhead and allocation of using params
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		public static float Max(float a, float b, float c) => Mathf.Max(a, Mathf.Max(b, c));

		/// <summary>
		/// Finds which number from an array of numbers is nearest to the supplied number
		/// </summary>
		/// <param name="val">Number to evaluate</param>
		/// <param name="options">Array of floats to check for the nearest</param>
		/// <returns>Nearest float in 'options' array to 'val'</returns>
		public static float Nearest(float val, params float[] options){
			float nearestDistance = float.PositiveInfinity;
			float nearestAngle = float.NaN;
			for (int i = 0; i < options.Length; i++){
				float difference = Math.Abs(val - options[i]);
				if (difference < nearestDistance){
					nearestDistance = difference;
					nearestAngle = options[i];
				}
			}

			return nearestAngle;
		}

		/// <summary>
		/// Normalizes an angle so it is witin a range of -360 to 360 (allowNegative true) or 0 to 360 (allowNegative false)
		/// </summary>
		/// <param name="angle">Angle to normalize</param>
		/// <param name="allowNegative">Allow a range of -360 to 360 for normalization</param>
		/// <returns></returns>
		public static float NormalizeAngle(float angle, bool allowNegative = false){
			if (allowNegative){
				while (angle < -360f){
					angle += 360f;
				}
			} else {
				while (angle < 0f){
					angle += 360f;
				}
			}
			while (angle > 360f){
				angle -= 360f;
			}
			return angle;
		}

		public static bool FastApproximately(float a, float b, float threshold) {
			return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
		}
		
		public static bool FastApproximately(double a, double b, double threshold) {
			return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
		}
		
		/// <summary>
		///   <para>Calculates the linear parameter t that produces the interpolant value within the range [a, b] without clamping.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static float InverseLerpUnclamped(float a, float b, float value) {
			if ((double) a != (double) b)
				return (float) (((double) value - (double) a) / ((double) b - (double) a));
			return 0.0f;
		}
		
		/// <summary>
		///   <para>Linearly interpolates between a and b by t without clamping</para>
		/// </summary>
		/// <param name="a">The start value.</param>
		/// <param name="b">The end value.</param>
		/// <param name="t">The interpolation value between the two floats.</param>
		/// <returns>
		///   <para>The interpolated float result between the two float values.</para>
		/// </returns>
		public static float LerpUnclamped(float a, float b, float t)
		{
			return a + (b - a) * t;
		}
	}
}