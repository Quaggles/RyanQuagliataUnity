using System.Collections.Generic;
using RyanQuagliataUnity.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace RyanQuagliataUnity.MaterialModifier {
    public class MaterialModifierVector : MaterialModifier<Vector4> {
        private const ShaderPropertyType SHADER_PROPERTY_TYPE = ShaderPropertyType.Vector;
        protected override bool IsValidProperty(ShaderPropertyType shaderPropertyType) => shaderPropertyType == SHADER_PROPERTY_TYPE;

        protected override IList<string> GetPropertyNames(Shader shader) => shader.GetPropertyNamesByType(SHADER_PROPERTY_TYPE);

        public override void UpdateMaterialPropertyBlock(MaterialPropertyBlock materialPropertyBlock, int propertyId, Vector4 value) => materialPropertyBlock.SetVector(propertyId, value);

        public override void UpdateUiMaterial(Material uiMaterial, int propertyId, Vector4 value) => uiMaterial.SetVector(propertyId, value);

        public void SetValue2(Vector2 vector) => SetValue(vector);
        
        public void SetValue3(Vector3 vector) => SetValue(vector);
        
        public void SetValue2Temporary(Vector2 vector) => SetValueTemporary(vector);
        
        public void SetValue3Temporary(Vector3 vector) => SetValueTemporary(vector);
    }
}