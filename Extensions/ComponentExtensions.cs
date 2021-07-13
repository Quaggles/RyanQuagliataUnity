using System;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class ComponentExtensions {
		/// <summary>
		/// Gets a component that exists on the target or adds a new one if it doesn't exist
		/// </summary>
		/// <param name="that"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetOrAddComponent<T>(this Component that) where T : Component => that.GetComponent<T>() ?? that.gameObject.AddComponent<T>();

		public static Component GetOrAddComponent(this Component that, Type type) => that.GetComponent(type) ?? that.gameObject.AddComponent(type);

		public static T GetOrAddComponent<T>(this GameObject that) where T : Component => that.GetComponent<T>() != null ? that.GetComponent<T>() : that.AddComponent<T>();

		public static T GetComponentIfNull<T>(this Component that, ref T value) where T : Component {
			if (value == null) value = that.GetComponent<T>();
			return value;
		}

		public static T GetComponent<T>(this Component that, ref T component) where T : Component => component ? component : component = that.GetComponent<T>();   
		public static T GetOrAddComponent<T>(this Component that, ref T component) where T : Component => component ? component : component = that.GetOrAddComponent<T>();   
	}
}
