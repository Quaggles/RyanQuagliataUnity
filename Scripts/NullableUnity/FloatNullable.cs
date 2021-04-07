using System;

namespace RyanQuagliataUnity.NullableUnity {
	[Serializable]
	public class FloatNullable : NullableUnity<float> {
		public FloatNullable() { }
		public FloatNullable(float value) : base(value) { }
		 
		// Implement this in inherited class to allow implicit float => FloatNullable
		public static implicit operator FloatNullable(float value) => new FloatNullable(value);
		 
		// Implement this in inherited class to allow setting null using FloatNullable
		public static implicit operator FloatNullable(NullableNull _) => new FloatNullable();

		// Implement this in inherited class to allow implicit FloatNullable => float
		public static implicit operator float(FloatNullable value) => value.Value;

		// Implement this in inherited class to allow implicit FloatNullable => float?, only possible if float is a value type
		public static implicit operator float?(FloatNullable value) => value.HasValue ? value.Value : new float?();
	}
}