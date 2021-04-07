using System;

namespace RyanQuagliataUnity.NullableUnity {
	[Serializable]
	public class BoolNullable : NullableUnity<bool> {
		public BoolNullable() { }
		public BoolNullable(bool value) : base(value) { }
		
		// Implement this in inherited class to allow implicit bool => BoolNullable
		public static implicit operator BoolNullable(bool value) => new BoolNullable(value);
		 
		// Implement this in inherited class to allow setting null using BoolNullable
		public static implicit operator BoolNullable(NullableNull _) => new BoolNullable();

		// Implement this in inherited class to allow implicit BoolNullable => bool
		public static implicit operator bool(BoolNullable value) => value.Value;

		// Implement this in inherited class to allow implicit BoolNullable => bool?, only possible if bool is a value type
		public static implicit operator bool?(BoolNullable value) => value.HasValue ? value.Value : new bool?();
	}
}