using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace RyanQuagliataUnity.Extensions {
	public static class CollectionExtensions {
		/// <summary>
		/// Attempts to cast the ICollection`1 into an into a List`1 to take advantage of the native 'AddRange' speed,
		/// if this isn't successful it inserts the range using a foreach loop
		/// </summary>
		/// <param name="that">Collection to add to</param>
		/// <param name="collection">Range of values that will be added to the collection</param>
		/// <typeparam name="T"></typeparam>
		public static void AddRange<T>([NotNull] this ICollection<T> that, [NotNull] IEnumerable<T> collection) {
			if (that == null) throw new ArgumentNullException(nameof(that));
			if (collection == null) throw new ArgumentNullException(nameof(collection));

			if (that is List<T> list)
				list.AddRange(collection);
			else
				collection.ForEach(that.Add);
		}
		
		/// <summary>
		/// Adds a range of values to a collection but only the ones that don't already exist in it
		/// </summary>
		/// <param name="that"></param>
		/// <param name="collection"></param>
		/// <typeparam name="T"></typeparam>
		/// <exception cref="ArgumentNullException"></exception>
		public static void AddUniqueRange<T>([NotNull] this ICollection<T> that, [NotNull] IEnumerable<T> collection) {
			if (that == null) throw new ArgumentNullException(nameof(that));
			if (collection == null) throw new ArgumentNullException(nameof(collection));

			foreach (var item in collection) {
				if (that.Contains(item)) continue;
				that.Add(item);
			}
		}
	}
}