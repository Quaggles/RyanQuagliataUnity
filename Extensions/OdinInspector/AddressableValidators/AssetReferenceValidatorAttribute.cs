using System;
using System.Reflection;
using RyanQuagliata.Extensions;
using RyanQuagliataUnity.Extensions.OdinInspector.AddressableValidators;
using Sirenix.OdinInspector.Editor.Validation;
using Sirenix.Utilities;
using UnityEngine.AddressableAssets;

[assembly: RegisterValidator(typeof(AssetReferenceValidator))]

namespace RyanQuagliataUnity.Extensions.OdinInspector.AddressableValidators {
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

	public class AssetReferenceValidator : AttributeValidator<AssetReferenceValidatorAttribute> {
		public override RevalidationCriteria RevalidationCriteria => RevalidationCriteria.Always;

		public override bool CanValidateMember(MemberInfo member, Type memberValueType) {
			return memberValueType.IsAssignableFrom(typeof(AssetReference));
		}

		protected override void Validate(object parentInstance, object memberValue, MemberInfo member,
			ValidationResult result) {
			if (memberValue is AssetReference assetReference) {
				if (assetReference.editorAsset == null) {
					result.ResultType = ValidationResultType.Valid;
					return;
				}

				var editorAssetType = assetReference.editorAsset.GetType();
				if (IsValid(assetReference.editorAsset.GetType(), Attribute.EnforceType, Attribute.TypeStrictness)) {
					result.ResultType = ValidationResultType.Valid;
				} else {
					result.ResultType = ValidationResultType.Error;
					result.Message =
						$"{member.Name} {editorAssetType.ToString().SurroundApostrophe().Bold()} is not {Attribute.TypeStrictness.ToString().Bold().Italics()} enforced type {Attribute.EnforceType.ToString().SurroundApostrophe().Bold()}";
				}
			} else {
				result.ResultType = ValidationResultType.Error;
				result.Message =
					$"Cannot use {typeof(AssetReferenceValidatorAttribute).GetNiceName().SurroundApostrophe().Bold()} on member type {member.GetReturnType().ToString().SurroundApostrophe().Bold()} as it is not an {typeof(AssetReference).ToString().SurroundApostrophe().Bold()}";
			}
		}

		bool IsValid(Type editorAssetType, Type suppliedType, TypeStrictness typeStrictness) {
			switch (typeStrictness) {
				case TypeStrictness.AssignableFrom:
					return editorAssetType.IsAssignableFrom(suppliedType);
				case TypeStrictness.EquivalentTo:
					return editorAssetType.IsEquivalentTo(suppliedType);
				default:
					throw new ArgumentOutOfRangeException(nameof(typeStrictness), typeStrictness, null);
			}
		}
	}
}