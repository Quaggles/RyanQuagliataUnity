﻿using System;
using System.Collections.Generic;
using RyanQuagliataUnity.Extensions;
using RyanQuagliataUnity.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace RyanQuagliataUnity.MaterialModifier {
	[ExecuteInEditMode, HideMonoScript]
	public abstract class MaterialModifier<T> : MonoBehaviour, IMaterialModifier {
		[HorizontalGroup("ModifierType")]
		public MaterialModifierType MaterialModifierType;

		[HorizontalGroup("ModifierType")]
		[HideLabel]
		[ShowIf("@MaterialModifierType == RyanQuagliataUnity.MaterialModifier.MaterialModifierType.Renderer")]
		public Renderer Renderer;

		[HorizontalGroup("ModifierType")]
		[HideLabel]
		[ShowIf("@MaterialModifierType == RyanQuagliataUnity.MaterialModifier.MaterialModifierType.UI")]
		public Graphic Graphic;

		[HorizontalGroup("ModifierType")]
		[HideLabel]
		[ShowIf("@MaterialModifierType == RyanQuagliataUnity.MaterialModifier.MaterialModifierType.Material")]
		public Material Material;

		private MaterialPropertyBlock materialPropertyBlock;

		private int propertyId;

		void UpdateShaderPropertyId() => propertyId = Shader.PropertyToID(propertyName);

		[ShowInInspector]
		[ValidateInput(nameof(ValidPropertyName))]
		[ValueDropdown(nameof(PropertyNames))]
		[OnValueChanged(nameof(UpdateShaderPropertyId))]
		[PropertyOrder(0)]
		public string PropertyName {
			get => propertyName;
			set {
				propertyName = value;
				UpdateShaderPropertyId();
			}
		}

		[SerializeField, HideInInspector]
		private string propertyName;

		private bool ValidPropertyName() => ValidPropertyName(PropertyName);

		private bool ValidPropertyName(string propertyName) {
			var mat = GetMaterial();
			if (mat == null) return true;
			if (!mat.shader.DoesPropertyExist(propertyName)) return false;
			var propType = mat.shader.GetPropertyType(propertyName);
			return IsValidProperty(propType);
		}

		protected abstract bool IsValidProperty(ShaderPropertyType shaderPropertyType);

		private IList<string> PropertyNames() {
			var mat = GetMaterial();
			if (mat == null) return null;
			var list = GetPropertyNames(mat.shader);
			Sort(list);
			return list;
		}

		public static void Sort<T>(IList<T> list) {
			if (list is List<T>) {
				((List<T>) list).Sort();
			} else {
				List<T> objList = new List<T>((IEnumerable<T>) list);
				objList.Sort();
				for (int index = 0; index < list.Count; ++index) list[index] = objList[index];
			}
		}

		protected abstract IList<string> GetPropertyNames(Shader shader);

		[HorizontalGroup("Value")]
		public bool ManualValue = true;

		[HorizontalGroup("Value")]
		[HideLabel]
		[OnValueChanged(nameof(UpdateMaterial))]
		[ShowIf(nameof(ManualValue))]
		public T Value;

		[HorizontalGroup("Value")]
		[HideLabel]
		[ShowInInspector, ReadOnly]
		[HideIf(nameof(ManualValue))]
		private T valueNonSerialized;

		private Material uiMaterial;

		void Awake() {
			if (Renderer == null) {
				Renderer = GetComponent<Renderer>();
				if (Renderer) MaterialModifierType = MaterialModifierType.Renderer;
			}

			if (Graphic == null) {
				Graphic = GetComponent<Graphic>();
				if (Graphic) MaterialModifierType = MaterialModifierType.UI;
			}

			UpdateShaderPropertyId();
		}

		void Start() {
			if (ManualValue) UpdateMaterial();
		}

		public Material GetMaterial() {
			try {
				return MaterialModifierType switch {
					MaterialModifierType.Renderer => Renderer.sharedMaterial,
					MaterialModifierType.UI => Graphic.materialForRendering,
					MaterialModifierType.Material => Material,
					_ => throw new ArgumentOutOfRangeException()
				};
			} catch {
				return null;
			}
		}

		private void OnDidApplyAnimationProperties() => UpdateMaterial();

		[Button, EnableIf(nameof(ValidPropertyName))]
		[ShowIf(nameof(ManualValue))]
		public void UpdateMaterial() => UpdateMaterial(Value);

		public void UpdateMaterial(T value) {
			switch (MaterialModifierType) {
				case MaterialModifierType.Renderer:
					if (!Renderer) throw new NullReferenceException(nameof(Renderer));
					if (materialPropertyBlock == null) materialPropertyBlock = new MaterialPropertyBlock();
					Renderer.GetPropertyBlock(materialPropertyBlock);
					UpdateMaterialPropertyBlock(materialPropertyBlock, propertyId, value);
					Renderer.SetPropertyBlock(materialPropertyBlock);
					break;
				case MaterialModifierType.UI:
					if (!Graphic) throw new NullReferenceException(nameof(Graphic));
					valueNonSerialized = value;
					Graphic.SetMaterialDirty();
					break;
				case MaterialModifierType.Material:
					UpdateMaterial(GetMaterial(), propertyId, value);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public abstract void UpdateMaterialPropertyBlock(MaterialPropertyBlock materialPropertyBlock, int propertyId, T value);
		public abstract void UpdateUiMaterial(Material uiMaterial, int propertyId, T value);
		public abstract void UpdateMaterial(Material material, int propertyId, T value);


		public void SetValue(T value) {
			Value = value;
			UpdateMaterial();
		}

		public void SetValueTemporary(T value) => UpdateMaterial(value);

		public Material GetModifiedMaterial(Material baseMaterial) {
			if (MaterialModifierType != MaterialModifierType.UI) {
				if (uiMaterial) {
					uiMaterial.DestroySmart();
					uiMaterial = null;
				}

				return baseMaterial;
			}

			if (uiMaterial == null || uiMaterial.shader != baseMaterial.shader) {
				uiMaterial = new Material(baseMaterial);
				uiMaterial.hideFlags = HideFlags.HideAndDontSave;
			} else {
				uiMaterial.CopyPropertiesFromMaterial(baseMaterial);
			}

			UpdateUiMaterial(uiMaterial, propertyId, valueNonSerialized);
			return uiMaterial;
		}
	}

	public enum MaterialModifierType {
		Renderer,
		UI,
		Material
	}
}