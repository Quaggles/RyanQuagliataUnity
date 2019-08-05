using System.Collections.Generic;

namespace RyanQuagliataUnity.Extensions {
	public static class EnumeratorExtensions {
		/// <summary>
		/// Enumerates over an IEnumerator to create a list
		/// </summary>
		/// <param name="that"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<T> ToList<T>(this IEnumerator<T> that) {
			var list = new List<T>();
			while ( that.MoveNext() ) {
				list.Add(that.Current);
			}
			return list;
		}
	}
}