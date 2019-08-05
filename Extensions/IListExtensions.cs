using System;
using System.Collections.Generic;

namespace RyanQuagliataUnity.Extensions {
	public static class ListExtensions {
		/// <summary>
		/// Fisher-Yates Shuffle
		/// <Source cref="https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle"/>
		/// </summary>
		/// <param name="list"></param>
		/// <param name="rnd"></param>
		/// <typeparam name="T"></typeparam>
		public static IList<T> Shuffle<T>(this IList<T> list, Random rnd) {
			for (var i = 0; i < list.Count - 1; i++) list.Swap(i, rnd.Next(i, list.Count));
			return list;
		}

		public static void Swap<T>(this IList<T> list, int i, int j) {
			var temp = list[i];
			list[i] = list[j];
			list[j] = temp;
		}
	}
}