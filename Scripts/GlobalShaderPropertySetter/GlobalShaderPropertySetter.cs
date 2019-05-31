using System;
using System.Collections.Generic;
using System.Linq;
using RyanQuagliata.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliata.GlobalShaderPropertySetter {
    [Serializable]
    public abstract class GlobalShaderPropertySetter<T> : IGlobalShaderPropertySettable {
        protected GlobalShaderPropertySetter() {
            Enabled = true;
        }

        [SerializeField, HideInInspector] private bool enabled;

        [TableColumnWidth(55), PropertyOrder(-2), ShowInInspector]
        public bool Enabled {
            get { return enabled; }
            set { enabled = value; }
        }

        [PropertyOrder(-1), ShowInInspector]
        public string SourceTypeName => typeof(T).Name;

        [Required, OnValueChanged(nameof(UpdateValuePreview))]
        public Func<T> ValueSource;

        public void UpdateValuePreview() => UpdateValuePreview(ValueSource);

        private void UpdateValuePreview(Func<T> value) =>
            valuePreview = value == null ? default(T) : ValueSource.Invoke();

#pragma warning disable 0414
        [LabelText("")] [ReadOnly, ShowInInspector, NonSerialized]
        private T valuePreview;
#pragma warning restore 0414

        [SerializeField, HideInInspector] private string globalShaderPropertyName;

        [ValidateInput(nameof(StringNotEmpty), "Cannot be empty"), ShowInInspector]
        public string GlobalShaderPropertyName {
            get { return globalShaderPropertyName; }
            set { globalShaderPropertyName = value; }
        }

        private bool StringNotEmpty(string that) {
            return !string.IsNullOrWhiteSpace(that);
        }

        public void Set() {
            if (!Enabled) return;
            var value = ValueSource == null ? default(T) : ValueSource.Invoke();
            valuePreview = value;
            Set(value);
        }

        protected abstract void Set(T value);
    }

    [Serializable]
    public class SetBuffer : GlobalShaderPropertySetter<ComputeBuffer> {
        protected override void Set(ComputeBuffer value) {
            Shader.SetGlobalBuffer(GlobalShaderPropertyName, value);
        }
    }

    [Serializable]
    public class SetColor : GlobalShaderPropertySetter<Color> {
        protected override void Set(Color value) {
            Shader.SetGlobalColor(GlobalShaderPropertyName, value);
        }
    }

    [Serializable]
    public class SetFloat : GlobalShaderPropertySetter<float> {
        protected override void Set(float value) {
            Shader.SetGlobalFloat(GlobalShaderPropertyName, value);
        }
    }

    [Serializable]
    public class SetFloatArray : GlobalShaderPropertySetter<IEnumerable<float>> {
        protected override void Set(IEnumerable<float> value) {
            Shader.SetGlobalFloatArray(GlobalShaderPropertyName, value.ToArray());
        }
    }

    [Serializable]
    public class SetInt : GlobalShaderPropertySetter<int> {
        protected override void Set(int value) {
            Shader.SetGlobalInt(GlobalShaderPropertyName, value);
        }
    }

    [Serializable]
    public class SetMatrix : GlobalShaderPropertySetter<Matrix4x4> {
        protected override void Set(Matrix4x4 value) {
            Shader.SetGlobalMatrix(GlobalShaderPropertyName, value);
        }
    }

    [Serializable]
    public class SetMatrixArray : GlobalShaderPropertySetter<IEnumerable<Matrix4x4>> {
        protected override void Set(IEnumerable<Matrix4x4> value) {
            Shader.SetGlobalMatrixArray(GlobalShaderPropertyName, value.ToArray());
        }
    }

    [Serializable]
    public class SetTexture : GlobalShaderPropertySetter<Texture> {
        protected override void Set(Texture value) {
            Shader.SetGlobalTexture(GlobalShaderPropertyName, value);
        }
    }

    [Serializable]
    public class SetVector2 : GlobalShaderPropertySetter<Vector2> {
        protected override void Set(Vector2 value) {
            Shader.SetGlobalVector(GlobalShaderPropertyName, value);
        }
    }

    [Serializable]
    public class SetVector3 : GlobalShaderPropertySetter<Vector3> {
        protected override void Set(Vector3 value) {
            Shader.SetGlobalVector(GlobalShaderPropertyName, value);
        }
    }

    [Serializable]
    public class SetVector4 : GlobalShaderPropertySetter<Vector4> {
        protected override void Set(Vector4 value) {
            Shader.SetGlobalVector(GlobalShaderPropertyName, value);
        }
    }

    [Serializable]
    public class SetQuaternionAsVector4 : GlobalShaderPropertySetter<Quaternion> {
        protected override void Set(Quaternion value) {
            Shader.SetGlobalVector(GlobalShaderPropertyName, new Vector4(value.x, value.y, value.z, value.w));
        }
    }

    [Serializable]
    public class SetVector4Array : GlobalShaderPropertySetter<Vector4[]> {
        public string GlobalShaderPropertyArrayLengthName;

        protected override void Set(Vector4[] value) {
	        if (value.Length == 0) return;
            Shader.SetGlobalVectorArray(GlobalShaderPropertyName, value);
            Shader.SetGlobalInt(GlobalShaderPropertyArrayLengthName, value.Length);
        }
    }
	
    [Serializable]
    public class SetVector4List : GlobalShaderPropertySetter<List<Vector4>> {
        public string GlobalShaderPropertyArrayLengthName;

        protected override void Set(List<Vector4> value) {
	        if (value.Count == 0) return;
            Shader.SetGlobalVectorArray(GlobalShaderPropertyName, value);
            Shader.SetGlobalInt(GlobalShaderPropertyArrayLengthName, value.Count);
        }
    }
}