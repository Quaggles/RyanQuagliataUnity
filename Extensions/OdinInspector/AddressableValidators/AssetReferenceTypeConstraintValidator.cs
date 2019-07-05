using System;
using System.Reflection;
using RyanQuagliata.Extensions;
using RyanQuagliataUnity.Attributes;
using RyanQuagliataUnity.Extensions.OdinInspector.AddressableValidators;
using Sirenix.OdinInspector.Editor.Validation;
using Sirenix.Utilities;
using UnityEngine.AddressableAssets;

[assembly: RegisterValidator(typeof(AssetReferenceTypeConstraintValidator))]

namespace RyanQuagliataUnity.Extensions.OdinInspector.AddressableValidators {
	public class AssetReferenceTypeConstraintValidator : AttributeValidator<AssetReferenceTypeConstraintAttribute> {
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
				if (IsValid(assetReference.editorAsset.GetType(), Attribute.Type, Attribute.TypeStrictness)) {
					result.ResultType = ValidationResultType.Valid;
				} else {
					result.ResultType = ValidationResultType.Error;
					result.Message =
						$"{member.Name} {editorAssetType.ToString().SurroundApostrophe().Bold()} is not {Attribute.TypeStrictness.ToString().Bold().Italics()} enforced type {Attribute.Type.ToString().SurroundApostrophe().Bold()}";
				}
			} else {
				result.ResultType = ValidationResultType.Error;
				result.Message =
					$"Cannot use {typeof(AssetReferenceTypeConstraintAttribute).GetNiceName().SurroundApostrophe().Bold()} on member type {member.GetReturnType().ToString().SurroundApostrophe().Bold()} as it is not an {typeof(AssetReference).ToString().SurroundApostrophe().Bold()}";
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