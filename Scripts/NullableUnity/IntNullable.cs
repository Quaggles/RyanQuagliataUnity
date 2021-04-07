using System;

namespace RyanQuagliataUnity.NullableUnity {
	[Serializable]
	public class IntNullable : NullableUnity<int> {
		public IntNullable() { }
		public IntNullable(int value) : base(value) { }
		 
		// Implement this in inherited class to allow implicit int => IntNullable
		public static implicit operator IntNullable(int value) => new IntNullable(value);
		 
		// Implement this in inherited class to allow setting null using IntNullable
		public static implicit operator IntNullable(NullableNull _) => new IntNullable();

		// Implement this in inherited class to allow implicit IntNullable => int
		public static implicit operator int(IntNullable value) => value.Value;

		// Implement this in inherited class to allow implicit IntNullable => int?, only possible if int is a value type
		public static implicit operator int?(IntNullable value) => value.HasValue ? value.Value : new int?();
	}
}