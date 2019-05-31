using System;

namespace RyanQuagliata.Extensions {
	public static class FloatExtensions {
		/// <summary>
		/// Remaps a value from a previous min and max range to a new range
		/// </summary>
		/// <param name="iValue">Value to remap</param>
		/// <param name="iMin">Min value of input range</param>
		/// <param name="iMax">Max value of input range</param>
		/// <param name="oMin">Min value of output range</param>
		/// <param name="oMax">Max value of output range</param>
		/// <returns>Remapped value</returns>
		public static float Remap(this float iValue, float iMin, float iMax, float oMin, float oMax) => Utilities.MathfExtensions.Remap(iValue, iMin, iMax, oMin, oMax);

		/// <summary>
		/// Remaps a value from a previous min and max range to a new range
		/// </summary>
		/// <param name="iValue">Value to remap</param>
		/// <param name="iMin">Min value of input range</param>
		/// <param name="iMax">Max value of input range</param>
		/// <param name="oMin">Min value of output range</param>
		/// <param name="oMax">Max value of output range</param>
		/// <returns>Remapped value</returns>
		public static float Remap(this int iValue, float iMin, float iMax, float oMin, float oMax) => Utilities.MathfExtensions.Remap(iValue, iMin, iMax, oMin, oMax);

		/// <summary>
		/// Returns whether the float is approximately equal to the given value
		/// </summary>
		/// <param name="that"></param>
		/// <param name="value"></param>
		/// <param name="tolerance">How far apart the values can be and still be considered equal, default is Epsilon</param>
		/// <returns></returns>
		public static bool ApproximatelyEqual(this float that, float value, float tolerance = float.Epsilon) => Utilities.MathfExtensions.FastApproximately(that, value, tolerance);

		/// <summary>
		/// Returns whether floats are approximately equal with a tolerance relative to the smallest value,
		/// for example with a 10% tolerance a value of 1 and 1.1 would be approximately the same but 1.2 would not
		/// </summary>
		/// <param name="that"></param>
		/// <param name="value"></param>
		/// <param name="relativeTolerance">How far apart the larger value can be from the lesser relatively</param>
		/// <returns></returns>
		public static bool ApproximatelyEqualRelative(this float that, float value, float relativeTolerance) {
			if (relativeTolerance < 0) throw new ArgumentOutOfRangeException(nameof(relativeTolerance), "Must be greater than 0");
			float tolerance = UnityEngine.Mathf.Min(that, value) * relativeTolerance;
			return Utilities.MathfExtensions.FastApproximately(that, value, tolerance);
		}
		
		/// <summary>
		/// Returns whether doubles are approximately equal with a tolerance relative to the smallest value,
		/// for example with a 10% tolerance a value of 1 and 1.1 would be approximately the same but 1.2 would not
		/// </summary>
		/// <param name="that"></param>
		/// <param name="value"></param>
		/// <param name="relativeTolerance">How far apart the larger value can be from the lesser relatively</param>
		/// <returns></returns>
		public static bool ApproximatelyEqualRelative(this double that, double value, double relativeTolerance) {
			if (relativeTolerance < 0) throw new ArgumentOutOfRangeException(nameof(relativeTolerance), "Must be greater than 0");
			double tolerance = Math.Min(that, value) * relativeTolerance;
			return Utilities.MathfExtensions.FastApproximately(that, value, tolerance);
		}
	}
}
