using System.Collections.Generic;
using RyanQuagliataUnity.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace RyanQuagliataUnity.MaterialModifier {
	public class MaterialModifierColour : MaterialModifier<Color> {
		protected override bool IsValidProperty(ShaderPropertyType shaderPropertyType) => shaderPropertyType == ShaderPropertyType.Color;
		protected override IList<string> GetPropertyNames(Shader shader) => shader.GetPropertyNamesByType(ShaderPropertyType.Color);

		public override void UpdateMaterialPropertyBlock(MaterialPropertyBlock materialPropertyBlock, int propertyId, Color value) => materialPropertyBlock.SetColor(propertyId, value);

		public override void UpdateUiMaterial(Material uiMaterial, int propertyId, Color value) => uiMaterial.SetColor(propertyId, value);
	}
}