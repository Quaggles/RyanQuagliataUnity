using System.Collections.Generic;

namespace RyanQuagliataUnity.Utilities {
	public abstract class EqualityComparerSingleton<T1, T2> : IEqualityComparer<T2> where T1 : new() {
		public static EqualityComparerSingleton<T1, T2> Instance => instance ??= new T1();
		private static EqualityComparerSingleton<T1, T2> instance;
		public abstract bool Equals(T2 x, T2 y);
		public abstract int GetHashCode(T2 obj);
	}
}