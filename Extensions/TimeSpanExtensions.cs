using System;
using System.Threading;

namespace RyanQuagliataUnity.Extensions {
	public static class TimeSpanExtensions {
		/// <summary>
		/// Does this timespan represent an infinite duration
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static bool IsInfinite(this TimeSpan that) {
			return that == Timeout.InfiniteTimeSpan;
		}
	}
}
