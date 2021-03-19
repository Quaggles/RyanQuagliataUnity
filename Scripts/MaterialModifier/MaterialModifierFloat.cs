using System.Collections.Generic;
using RyanQuagliataUnity.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace RyanQuagliataUnity.MaterialModifier {
    public class MaterialModifierFloat : MaterialModifier<float> {
        protected override bool IsValidProperty(ShaderPropertyType shaderPropertyType) => shaderPropertyType == ShaderPropertyType.Float || shaderPropertyType == ShaderPropertyType.Range;
        protected override IList<string> GetPropertyNames(Shader shader) {
            var list = new List<string>();
            list.AddRange(shader.GetPropertyNamesByType(ShaderPropertyType.Float));
            list.AddRange(shader.GetPropertyNamesByType(ShaderPropertyType.Range));
            return list;
        }

        public override void UpdateMaterialPropertyBlock(MaterialPropertyBlock materialPropertyBlock, int propertyId, float value) => materialPropertyBlock.SetFloat(propertyId, value);

        public override void UpdateUiMaterial(Material uiMaterial, int propertyId, float value) => uiMaterial.SetFloat(propertyId, value);
    }
}