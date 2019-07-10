using System;
using System.Reflection;
using RyanQuagliata.Extensions;
using RyanQuagliataUnity.Attributes;
using RyanQuagliataUnity.Extensions.OdinInspector.AddressableValidators;
using Sirenix.OdinInspector.Editor.Validation;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;

[assembly: RegisterValidator(typeof(AssetReferenceRequiredValidator))]

namespace RyanQuagliataUnity.Extensions.OdinInspector.AddressableValidators {
	public class AssetReferenceRequiredValidator : AttributeValidator<AssetReferenceRequiredAttribute> {
		public override RevalidationCriteria RevalidationCriteria => RevalidationCriteria.Always;

		public override bool CanValidateMember(MemberInfo member, Type memberValueType) {
			return memberValueType.IsAssignableFrom(typeof(AssetReference));
		}

		protected override void Validate(object parentInstance, object memberValue, MemberInfo member,
			ValidationResult result) {
			if (memberValue is AssetReference assetReference) {
				string guid = assetReference.RuntimeKey.ToString();
				var aaSettings = AddressableAssetSettingsDefaultObject.Settings;
				if (assetReference.editorAsset == null) {
					result.ResultType = ValidationResultType.Error;
					result.Message = $"{member.Name} is required";
				} else if (aaSettings != null && aaSettings.FindAssetEntry(guid) == null) {
					result.ResultType = ValidationResultType.Error;
					result.Message = $"{member.Name} asset {AssetDatabase.GetAssetPath(assetReference.editorAsset).SurroundApostrophe().Bold()} is not marked as addressable";
				}
			} else {
				result.ResultType = ValidationResultType.Error;
				result.Message =
					$"Cannot use {typeof(AssetReferenceRequiredAttribute).GetNiceName().Bold()} on member type {member.GetReturnType().ToString().Bold()} as is not a {typeof(AssetReference).ToString().Bold()}";
			}
		}
	}
}