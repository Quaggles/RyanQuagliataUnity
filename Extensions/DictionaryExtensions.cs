using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace RyanQuagliata.Extensions {
	public static class DictionaryExtensions {
		/// <summary>
		/// Attempts to get a value from a dictionary, if it doesn't exist it creates a new value using the default constructor and adds that
		/// </summary>
		/// <param name="that"></param>
		/// <param name="key"></param>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <returns></returns>
		public static TValue GetOrNew<TKey, TValue>(this IDictionary<TKey, TValue> that, [NotNull] TKey key) where TValue : new() {
			return GetOrAdd(that, key, () => new TValue());
		}

		/// <summary>
		/// Attempts to get a value from a dictionary, if it doesn't exist it creates the default value and adds that
		/// </summary>
		/// <param name="that"></param>
		/// <param name="key"></param>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <returns></returns>
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> that, [NotNull] TKey key) {
			return GetOrAdd(that, key, () => default(TValue));
		}

		/// <summary>
		/// Attempts to get a value from a dictionary, if it doesn't exist it creates a new one using the factory function
		/// then adds that to the dictionaru
		/// </summary>
		/// <param name="that"></param>
		/// <param name="key"></param>
		/// <param name="factory"></param>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <returns></returns>
		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> that, [NotNull] TKey key,
			[NotNull] Func<TValue> factory) {
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (factory == null) throw new ArgumentNullException(nameof(factory));
			TValue value;
			if (that.TryGetValue(key, out value)) {
				return value;
			}

			value = factory.Invoke();
			that.Add(key, value);
			return value;
		}
	}
}