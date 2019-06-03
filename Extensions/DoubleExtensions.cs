using System;

namespace RyanQuagliata.Extensions {
	public static class DoubleExtensions {
		/// <summary>
		/// Remaps a value from a previous min and max range to a new range
		/// </summary>
		/// <param name="iValue">Value to remap</param>
		/// <param name="iMin">Min value of input range</param>
		/// <param name="iMax">Max value of input range</param>
		/// <param name="oMin">Min value of output range</param>
		/// <param name="oMax">Max value of output range</param>
		/// <returns>Remapped value</returns>
		public static double Remap(this double iValue, double iMin, double iMax, double oMin, double oMax) => Utilities.MathfExtensions.Remap(iValue, iMin, iMax, oMin, oMax);
	}
}
