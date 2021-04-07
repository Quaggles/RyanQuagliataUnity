using System;

namespace RyanQuagliataUnity.NullableUnity {
	[Serializable]
	public class StringNullable : NullableUnity<string> {
		public StringNullable() { }
		public StringNullable(string value) : base(value) { }

		// Implement this in inherited class to allow implicit string => StringNullable
		public static implicit operator StringNullable(string value) => new StringNullable(value);
		 
		// Implement this in inherited class to allow setting null using StringNullable
		public static implicit operator StringNullable(NullableNull _) => new StringNullable();

		// Implement this in inherited class to allow implicit StringNullable => string
		public static implicit operator string(StringNullable value) => value.Value;
		
	}
}