using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace RyanQuagliata.Extensions {
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

			var list = that as List<T>;
			if (list != null)
				list.AddRange(collection);
			else
				collection.ForEach(that.Add);
		}
	}
}