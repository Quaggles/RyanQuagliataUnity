using System;

namespace RyanQuagliataUnity.Attributes {
	public enum TypeStrictness {
		AssignableFrom,
		EquivalentTo
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class AssetReferenceValidatorAttribute : Attribute {
		public Type EnforceType;

		public TypeStrictness TypeStrictness;

		/// <summary>
		/// Enforces that an AssetReference must be of a certain type or is required
		/// </summary>
		/// <param name="enforceType">The type that the Asset Reference must adhere to</param>
		/// <param name="typeStrictness">How strictly does the type need to match</param>
		public AssetReferenceValidatorAttribute(Type enforceType,
			TypeStrictness typeStrictness = TypeStrictness.AssignableFrom) {
			EnforceType = enforceType;
			TypeStrictness = typeStrictness;
		}
	}
}