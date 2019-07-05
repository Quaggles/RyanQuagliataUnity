using System;
using System.Reflection;
using RyanQuagliata.Extensions;
using RyanQuagliataUnity.Attributes;
using RyanQuagliataUnity.Extensions.OdinInspector.AddressableValidators;
using Sirenix.OdinInspector.Editor.Validation;
using Sirenix.Utilities;
using UnityEngine.AddressableAssets;

[assembly: RegisterValidator(typeof(AssetReferenceRequiredAttributeValidator))]

namespace RyanQuagliataUnity.Extensions.OdinInspector.AddressableValidators {

	public class AssetReferenceRequiredAttributeValidator : AttributeValidator<AssetReferenceRequiredAttribute> {
		public override RevalidationCriteria RevalidationCriteria => RevalidationCriteria.Always;

		public override bool CanValidateMember(MemberInfo member, Type memberValueType) {
			return memberValueType.IsAssignableFrom(typeof(AssetReference));
		}

		protected override void Validate(object parentInstance, object memberValue, MemberInfo member,
			ValidationResult result) {
			if (memberValue is AssetReference assetReference) {
				if (assetReference.editorAsset == null) {
					result.ResultType = ValidationResultType.Error;
					result.Message = $"{member.Name} is required";
				}
			} else {
				result.ResultType = ValidationResultType.Error;
				result.Message =
					$"Cannot use {typeof(AssetReferenceRequiredAttribute).GetNiceName().Bold()} on member type {member.GetReturnType().ToString().Bold()} as is not a {typeof(AssetReference).ToString().Bold()}";
			}
		}
	}
}