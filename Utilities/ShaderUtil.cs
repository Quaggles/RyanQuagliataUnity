using System;
using System.Collections.Generic;
using UnityEngine;

namespace RyanQuagliata.Utilities {
	public static class ShaderUtil {
		public static float GetPropertyDefaultValue(this Shader that, string propertyName) =>
			GetPropertyDefaultValue(that, GetPropertyIdX(that, propertyName));

		public static float GetPropertyDefaultValue(this Shader that, int propertyIdX) {
#if !UNITY_EDITOR
			throw new EditorOnlyException(nameof(GetPropertyDefaultValue));
#else
			var propertyType = UnityEditor.ShaderUtil.GetPropertyType(that, propertyIdX);
			if (propertyType == UnityEditor.ShaderUtil.ShaderPropertyType.Float || propertyType == UnityEditor.ShaderUtil.ShaderPropertyType.Range) 
				return UnityEditor.ShaderUtil.GetRangeLimits(that, propertyIdX, 0);
			throw new InvalidPropertyTypeException($"{propertyIdX} was of type {propertyType} instead of the required type {UnityEditor.ShaderUtil.ShaderPropertyType.Float} or {UnityEditor.ShaderUtil.ShaderPropertyType.Range}");
#endif
		}

		public static Vector2 GetPropertyRange(this Shader that, string propertyName) =>
			GetPropertyRange(that, GetPropertyIdX(that, propertyName));

		public static Vector2 GetPropertyRange(this Shader that, int propertyIdX) {
#if !UNITY_EDITOR
			throw new EditorOnlyException(nameof(GetPropertyDefaultValue));
#else
			// If the property isn't a range return (0, 0)
			var propertyType = UnityEditor.ShaderUtil.GetPropertyType(that, propertyIdX);
			if (propertyType != UnityEditor.ShaderUtil.ShaderPropertyType.Range) 
				throw new InvalidPropertyTypeException($"{propertyIdX} was of type {propertyType} instead of the required type {UnityEditor.ShaderUtil.ShaderPropertyType.Range}");

			Vector2 range = Vector2.zero;
			range.x = UnityEditor.ShaderUtil.GetRangeLimits(that, propertyIdX, 1);
			range.y = UnityEditor.ShaderUtil.GetRangeLimits(that, propertyIdX, 2);
			return range;
#endif
		}
	
		public class InvalidPropertyTypeException : Exception {
			public InvalidPropertyTypeException(string message) : base(message) {}
		}

		public static bool DoesPropertyExist(this Shader that, string propertyName) {
#if !UNITY_EDITOR
			return true;
#else
			for (int i = 0; i < UnityEditor.ShaderUtil.GetPropertyCount(that); i++) {
				var name = UnityEditor.ShaderUtil.GetPropertyName(that, i);
				if (name == propertyName) return true;
			}

			return false;
#endif			
		}

		/// <summary>
		/// Gets the local property index
		/// </summary>
		/// <param name="that"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static int GetPropertyIdX(this Shader that, string propertyName) {
#if !UNITY_EDITOR
			throw new EditorOnlyException(nameof(GetPropertyDefaultValue));
#else
			for (int i = 0; i < UnityEditor.ShaderUtil.GetPropertyCount(that); i++) {
				if (UnityEditor.ShaderUtil.GetPropertyName(that, i) == propertyName) return i;
			}

			return -1;
#endif
		}

		public static IList<string> GetShaderPropertyNames(this Shader that, Predicate<int> filter = null) {
			var list = new List<string>();
#if UNITY_EDITOR
			for (int i = 0; i < UnityEditor.ShaderUtil.GetPropertyCount(that); i++) {
				if (!filter?.Invoke(i) ?? false) continue;
				list.Add(UnityEditor.ShaderUtil.GetPropertyName(that, i));
			}
#endif
			return list;
		}
	}
}