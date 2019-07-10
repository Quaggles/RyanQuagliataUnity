using System;

namespace RyanQuagliataUnity.Attributes {
	public enum TypeStrictness {
		AssignableFrom,
		EquivalentTo
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class AssetReferenceTypeConstraintAttribute : Attribute {
		public Type[] Types;

		public TypeStrictness TypeStrictness;

		/// <summary>
		/// Enforces that an AssetReference must be of a certain type or is required
		/// </summary>
		/// <param name="type">The type that the Asset Reference must adhere to</param>
		/// <param name="typeStrictness">How strictly does the type need to match</param>
		public AssetReferenceTypeConstraintAttribute(params Type[] types) {
			Types = types;
		}
	}
}