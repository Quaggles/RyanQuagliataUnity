using System.Collections.Generic;
using RyanQuagliataUnity.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace RyanQuagliataUnity.MaterialModifier {
	public class MaterialModifierBool : MaterialModifier<bool> {
		protected override bool IsValidProperty(ShaderPropertyType shaderPropertyType) => shaderPropertyType == ShaderPropertyType.Float;
		protected override IList<string> GetPropertyNames(Shader shader) => shader.GetPropertyNamesByType(ShaderPropertyType.Float);

		public override void UpdateMaterialPropertyBlock(MaterialPropertyBlock materialPropertyBlock, int propertyId, bool value) => materialPropertyBlock.SetFloat(propertyId, value ? 1 : 0);

		public override void UpdateUiMaterial(Material uiMaterial, int propertyId, bool value) => uiMaterial.SetFloat(propertyId, value ? 1 : 0);
		public override void UpdateMaterial(Material material, int propertyId, bool value) => material.SetFloat(propertyId, value ? 1 : 0);
	}
}