using System.Collections.Generic;

namespace RyanQuagliataUnity.Utilities {
	public abstract class EqualityComparerSingleton<T1, T2> : IEqualityComparer<T2> where T1 : class, new() {
		public static T1 Instance => instance ?? (instance = new T1());

		private static T1 instance;
		public abstract bool Equals(T2 x, T2 y);
		public abstract int GetHashCode(T2 obj);
	}
}