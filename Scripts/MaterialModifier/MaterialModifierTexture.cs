using System.Collections.Generic;
using RyanQuagliataUnity.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace RyanQuagliataUnity.MaterialModifier {
    public class MaterialModifierTexture : MaterialModifier<Texture> {
        private const ShaderPropertyType SHADER_PROPERTY_TYPE = ShaderPropertyType.Texture;
        protected override bool IsValidProperty(ShaderPropertyType shaderPropertyType) => shaderPropertyType == SHADER_PROPERTY_TYPE;

        protected override IList<string> GetPropertyNames(Shader shader) => shader.GetPropertyNamesByType(SHADER_PROPERTY_TYPE);

        public override void UpdateMaterialPropertyBlock(MaterialPropertyBlock materialPropertyBlock, int propertyId, Texture value) => materialPropertyBlock.SetTexture(propertyId, value);

        public override void UpdateUiMaterial(Material uiMaterial, int propertyId, Texture value) => uiMaterial.SetTexture(propertyId, Value);
        public override void UpdateMaterial(Material material, int propertyId, Texture value) {
            material.SetTexture(propertyId, value);
        }
    }
}