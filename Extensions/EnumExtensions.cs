using System;
using System.Linq;

namespace RyanQuagliataUnity.Extensions {
	public static class EnumExtensions {
		/// <summary>
		/// Finds and returns the next enum value after the supplied one
		/// </summary>
		/// <param name="that"></param>
		/// <typeparam name="TEnum">The type of enum that is supplied</typeparam>
		/// <returns>The next enumerated type after the current supplied one</returns>
		/// <exception cref="ArgumentException">Thrown when the supplied type isn't an enum</exception>
		public static TEnum GetNextEnum<TEnum>(this TEnum that)
			where TEnum : struct, IConvertible, IComparable, IFormattable {
			if (!typeof(TEnum).IsEnum) throw new ArgumentException("T must be an enumerated type");

			return (from TEnum val in Enum.GetValues(typeof(TEnum))
				where val.CompareTo(that) > 0
				orderby val
				select val).DefaultIfEmpty().First();
		}
		
		/// <summary>
		/// Finds and returns the previous enum value before the supplied one
		/// </summary>
		/// <param name="that"></param>
		/// <typeparam name="TEnum">The type of enum that is supplied</typeparam>
		/// <returns>The previous enumerated type before the current supplied one</returns>
		/// <exception cref="ArgumentException">Thrown when the supplied type isn't an enum</exception>
		public static TEnum GetPrevEnum<TEnum>(this TEnum that)
			where TEnum : struct, IConvertible, IComparable, IFormattable {
			if (!typeof(TEnum).IsEnum) throw new ArgumentException("T must be an enumerated type");

			return (from TEnum val in Enum.GetValues(typeof(TEnum))
				where val.CompareTo(that) < 0
				orderby val
				select val).DefaultIfEmpty().Last();
		}

		/// <summary>
		/// Finds the total amount of enumerated types by their names, types with the different names but the same value will be counted
		/// </summary>
		/// <typeparam name="TEnum">The type of enum to measure</typeparam>
		/// <returns>Finds the total amount of enumerated types by their names</returns>
		/// <exception cref="ArgumentException">Thrown when the supplied type isn't an enum</exception>
		public static int GetCountByNames<TEnum>() where TEnum : struct, IConvertible, IComparable, IFormattable{
			if (!typeof(TEnum).IsEnum) throw new ArgumentException("T must be an enumerated type");
			return Enum.GetNames(typeof(TEnum)).Length;
		}

		/// <summary>
		/// Finds the total amount of enumerated types by their distinct values
		/// </summary>
		/// <typeparam name="TEnum">The type of enum to measure</typeparam>
		/// <returns>Total amount of enumerated types by their distinct values</returns>
		/// <exception cref="ArgumentException">Thrown when the supplied type isn't an enum</exception>
		public static int GetCountByValues<TEnum>() where TEnum : struct, IConvertible, IComparable, IFormattable{
			if (!typeof(TEnum).IsEnum) throw new ArgumentException("T must be an enumerated type");
			return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Distinct().Count();
		}
	}
}