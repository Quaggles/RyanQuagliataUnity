using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace RyanQuagliataUnity.Extensions {
	public static class EnumerableExtensions {
		/// <summary>
		/// Does a where filter on an enumerable on all items in parallel
		/// </summary>
		/// <param name="that">Set of items we are operating on</param>
		/// <param name="predicate">Predicate that must be satisfied</param>
		/// <typeparam name="T">Set type</typeparam>
		/// <returns>Set of items that passed the predicate</returns>
		public static async Task<IEnumerable<T>> WhereParallelAsync<T>(this IEnumerable<T> that,
			Func<T, Task<bool>> predicate) {
			var results = new ConcurrentStack<T>();
			var tasks = that.Select(
				async x => {
					// Push the results to the stack as they are evaluated
					if (await predicate(x)) results.Push(x);
				});
			await Task.WhenAll(tasks);
			return results;
		}

		/// <summary>
		/// Does a where filter on an enumerable on all items sequentially
		/// </summary>
		/// <param name="that">Set of items we are operating on</param>
		/// <param name="predicate">Predicate that must be satisfied</param>
		/// <typeparam name="T">Set type</typeparam>
		public static async Task<IEnumerable<T>> WhereSequentialAsync<T>(this IEnumerable<T> that,
			Func<T, Task<bool>> predicate) {
			var results = new List<T>();
			foreach (var item in that)
				if (await predicate(item))
					results.Add(item);
			return results;
		}

		public static IEnumerable<T>
			DistinctBy<T, TIdentity>(this IEnumerable<T> source, Func<T, TIdentity> identitySelector) {
			return source.Distinct(By(identitySelector));
		}

		public static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector) {
			return new DelegateComparer<TSource, TIdentity>(identitySelector);
		}

		private class DelegateComparer<T, TIdentity> : IEqualityComparer<T> {
			private readonly Func<T, TIdentity> identitySelector;

			public DelegateComparer(Func<T, TIdentity> identitySelector) {
				this.identitySelector = identitySelector;
			}

			public bool Equals(T x, T y) {
				return Equals(identitySelector(x), identitySelector(y));
			}

			public int GetHashCode(T obj) {
				return identitySelector(obj).GetHashCode();
			}
		}

		/// <summary>
		/// Determines whether a collection is empty
		/// </summary>
		/// <param name="source"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>True if source is empty</returns>
		/// <exception cref="ArgumentNullException">source is null</exception>
		public static bool IsEmpty<T>([NotNull] this IEnumerable<T> source) {
			if (source == null) throw new ArgumentNullException(nameof(source));
			return !source.Any();
		}

		/// <summary>
		/// Performs a action on each element in an IEnumerable
		/// </summary>
		/// <param name="that"></param>
		/// <param name="action">Action to perform on each element</param>
		/// <typeparam name="T">Type of IEnumerable</typeparam>
		/// <returns>Original IEnumerable for method chaining</returns>
		public static IEnumerable<T> ForEach<T>([NotNull] this IEnumerable<T> that, Action<T> action = null) {
			foreach (var element in that) action?.Invoke(element);
			return that;
		}

		/// <summary>
		/// Determines whether a collection is empty
		/// </summary>
		/// <param name="source"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>True if source is null or empty</returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) {
			if (source == null) return true;
			return !source.Any();
		}

		private static Random random;
		public static Random Random => random ?? (random = new Random(Guid.NewGuid().GetHashCode()));

		/// <summary>
		/// Gets a random item from the enumerable
		/// </summary>
		/// <param name="that">Enumerable Collection</param>
		/// <typeparam name="T">Type of IEnumerable</typeparam>
		/// <returns>Random item from enumerable</returns>
		public static T GetRandomItem<T>(this IEnumerable<T> that) {
			if (!that.Any()) throw new EmptyEnumerableException();
			return that.ElementAt(GetRandomIndex(that));
		}

		public static int GetRandomIndex<T>(this IEnumerable<T> that) {
			if (!that.Any()) throw new EmptyEnumerableException();
			return Random.Next(that.Count());
		}

		/// <summary>
		/// https://stackoverflow.com/a/11930875
		/// </summary>
		/// <param name="that"></param>
		/// <param name="weightSelector"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static int GetRandomIndexWeighted<T>(this IEnumerable<T> that, Func<T, float> weightSelector) {
			float totalWeight = that.Sum(weightSelector);
			// The weight we are after...
			float itemWeightIndex = (float) Random.NextDouble() * totalWeight;
			float currentWeightIndex = 0;

			var array = that.Select(x => new {Value = x, Weight = weightSelector(x)}).ToArray();
			for (var index = 0; index < array.Length; index++) {
				var item = array[index];
				currentWeightIndex += item.Weight;

				// If we've hit or passed the weight we are after for this item then it's the one we want....
				if (currentWeightIndex >= itemWeightIndex) return index;
			}

			return -1;
		}

		/// <summary>
		/// https://stackoverflow.com/a/11930875
		/// </summary>
		/// <param name="that"></param>
		/// <param name="weightSelector"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetRandomItemWeighted<T>(this IEnumerable<T> that, Func<T, double> weightSelector) {
			var totalWeight = that.Sum(weightSelector);

			// The weight we are after...
			var itemWeightIndex = Random.NextDouble() * totalWeight;
			var currentWeightIndex = 0d;

			foreach (var item in from weightedItem in that select new {Value = weightedItem, Weight = weightSelector(weightedItem)}) {
				currentWeightIndex += item.Weight;

				// If we've hit or passed the weight we are after for this item then it's the one we want....
				if (currentWeightIndex >= itemWeightIndex) return item.Value;
			}

			return default;
		}

		/// <summary>
		/// Gets a random item from the enumerable that passes a filter
		/// </summary>
		/// <param name="that">Enumerable Collection</param>
		/// <param name="filter">Predicate filter for the return value</param>
		/// <param name="optionalFilters">If there are no results should the function return a random result even if it doesn't pass the filters</param>
		/// <typeparam name="T">Type of IEnumerable</typeparam>
		/// <returns>Random item from enumerable</returns>
		public static T GetRandomItemWithFilter<T>(this IEnumerable<T> that, Predicate<T> filter = null, bool optionalFilters = false) {
			if (!that.Any()) throw new EmptyEnumerableException();
			if (filter == null) filter = obj => true;

			// Create an enumerable of all valid items that pass the filter
			var validItems = that.Where(
				filter.Invoke
			);

			// If there are no results either throw an exception or handle the special case
			if (!validItems.Any()) {
				if (optionalFilters) return that.GetRandomItem();
				throw new NoFilteredResultsException();
			}

			// Select a random item out of that
			return validItems.ElementAt(Random.Next(validItems.Count()));
		}

		/// <summary>
		/// Gets a random item from the enumerable that passes a collection of filters
		/// </summary>
		/// <param name="that">Enumerable Collection</param>
		/// <param name="filters">Predicate filters for the return value</param>
		/// <param name="optionalFilters">If there are no results should the function return a random result even if it doesn't pass the filters</param>
		/// <typeparam name="T">Type of IEnumerable</typeparam>
		/// <returns>Random item from enumerable</returns>
		public static T GetRandomItemWithFilter<T>(this IReadOnlyCollection<T> that, IEnumerable<Predicate<T>> filters = null, bool optionalFilters = false) {
			if (!that.Any()) throw new EmptyEnumerableException();
			if (filters == null) filters = new Predicate<T>[] {obj => true};

			// Create an enumerable of all valid items that pass all filters
			var validItems = that.Where(
				item => { return filters.All(f => f.Invoke(item)); }
			);

			// If there are no results either throw an exception or handle the special case
			if (!validItems.Any()) {
				if (optionalFilters) return that.GetRandomItemWithFilter();
				throw new NoFilteredResultsException();
			}

			// Select a random item out of that
			return validItems.ElementAt(Random.Next(validItems.Count()));
		}

		public static IEnumerable<T> GetRandomItems<T>([NotNull] this IEnumerable<T> that, int count, IEnumerable<Predicate<T>> filters = null,
			bool optionalFilters = false) {
			if (that == null) throw new ArgumentNullException(nameof(that));
			if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
			var remainingList = that.ToList();
			var returnList = new List<T>();
			for (int i = 0; i < count; i++) {
				if (remainingList.IsEmpty()) return returnList;
				var item = remainingList.GetRandomItemWithFilter(filters, optionalFilters);
				remainingList.Remove(item);
				returnList.Add(item);
			}

			return returnList;
		}

		public class EmptyEnumerableException : Exception {
			public override string Message => "The collection has no items in it to randomly select from";
		}

		public class NoFilteredResultsException : Exception {
			public override string Message => "After running through the filters no valid results were found to choose from";
		}

		public static int MinIndex<T>([NotNull] this IEnumerable<T> that) {
			if (that == null) throw new ArgumentNullException(nameof(that));
			if (that.IsEmpty()) throw new EmptyEnumerableException();
			Comparer<T> comparer = Comparer<T>.Default;
			T minValue = that.First();
			int index = 0;
			int minIndex = 0;
			foreach (var x in that) {
				if (comparer.Compare(x, minValue) < 0) {
					minIndex = index;
					minValue = x;
				}

				index++;
			}

			return minIndex;
		}

		public static int MinIndex<T>(this IEnumerable<T> that, Func<T, float> selector) => that.Select(selector).MinIndex();

		/// <summary>
		/// If the collection is null or empty return defaultVal, otherwise return the average
		/// </summary>
		/// <param name="that"></param>
		/// <param name="selector"></param>
		/// <typeparam name="TSource"></typeparam>
		/// <returns></returns>
		public static float AverageOrDefault<TSource>(this IEnumerable<TSource> that, Func<TSource, float> selector, float defaultVal = 0) {
			if (that == null) return defaultVal;
			double total = 0.0;
			long iterator = 0;
			foreach (var i in that) {
				total += selector.Invoke(i);;
				checked {
					++iterator;
				}
			}

			if (iterator > 0L) return (float) total / (float) iterator;
			return defaultVal;
		}
	}
}