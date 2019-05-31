using System;
using System.Linq;
using JetBrains.Annotations;

namespace RyanQuagliata.Extensions {
	public static class StringExtensions {
		/// <summary>
		/// Creates a string where there are spaces before every capital letter (Except if the first letter is capital)
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static string ToSpaced(this string that) {
			return ToSpaced(that, (i, c) => i >= 0 && char.IsUpper(c));
		}

		/// <summary>
		/// Creates a string where there are spaces before any delimiting characters passed as an argument
		/// </summary>
		/// <param name="that"></param>
		/// <param name="wordDelimiters">Characters to be preceded by a space</param>
		/// <returns></returns>
		public static string ToSpaced(this string that, params char[] wordDelimiters) {
			return ToSpaced(that, wordDelimiters.Contains);
		}

		/// <summary>
		/// Creates a string where there are spaces before letters that match a condition
		/// </summary>
		/// <param name="that"></param>
		/// <param name="spaceBeforePredicate">Delegate to determine whether this characer be preceded by a space</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static string ToSpaced(this string that, [NotNull] Predicate<char> spaceBeforePredicate) {
			if (spaceBeforePredicate == null) throw new ArgumentNullException(nameof(spaceBeforePredicate));
			return ToSpaced(that, (i, c) => spaceBeforePredicate.Invoke(c));
		}

		/// <summary>
		/// Creates a string where there are spaces before letters that match a condition
		/// </summary>
		/// <param name="that"></param>
		/// <param name="spaceBeforePredicate">Delegate to determine whether this characer be preceded by a space</param>
		/// <returns></returns>
		public static string ToSpaced(this string that, [NotNull] Func<int, char, bool> spaceBeforePredicate) {
			if (spaceBeforePredicate == null) throw new ArgumentNullException(nameof(spaceBeforePredicate));
			string returnStr = "";
			for (var i = 0; i < that.Length; i++) {
				char c = that[i];
				if (i >= 0 && spaceBeforePredicate.Invoke(i, c)) returnStr += ' ';
				returnStr += c;
			}

			return returnStr;
		}

		/// <summary>
		/// Creates a string where characters after a space are capital
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static string ToTitleCase(this string that) {
			return ToTitleCase(that, ' ');
		}

		/// <summary>
		/// Creates a string where characters after a delimiter character are capital
		/// </summary>
		/// <param name="that"></param>
		/// <param name="wordDelimiters">Characters to denote the start of a word</param>
		/// <returns></returns>
		public static string ToTitleCase(this string that, params char[] wordDelimiters) {
			if (wordDelimiters == null) throw new ArgumentNullException(nameof(wordDelimiters));
			string returnStr = "";
			bool newWord = true;
			foreach (char c in that) {
				if (newWord) {
					returnStr += char.ToUpper(c);
					newWord = false;
				} else {
					returnStr += char.ToLower(c);
				}

				if (wordDelimiters.Contains(c)) newWord = true;
			}

			return returnStr;
		}

		public static bool Contains(this string that, string toCheck, StringComparison comp) {
			return that != null && toCheck != null && that.IndexOf(toCheck, comp) >= 0;
		}
	}
}