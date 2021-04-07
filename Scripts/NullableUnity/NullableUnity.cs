using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace RyanQuagliataUnity.NullableUnity {
	[Serializable]
#if ODIN_INSPECTOR
	[InlineProperty]
#endif
	public class NullableUnity<T> {
#if ODIN_INSPECTOR
		[HorizontalGroup, HideLabel]
#endif
		public bool HasValue;
#if ODIN_INSPECTOR
		[HorizontalGroup]
		[EnableIf(nameof(HasValue))]
#endif
		public T Value;
		protected NullableUnity() => HasValue = false;

		protected NullableUnity(T value) {
			Value = value;
			HasValue = true;
		}
		
		/* Implement these in inherited classes for easy implicit casting
		 
		 // Implement this in inherited class to allow implicit T => InheritedClass
		 public static implicit operator InheritedClass(T value) => new InheritedClass(value);
		 
		 // Implement this in inherited class to allow setting null using InheritedClass
		 public static implicit operator InheritedClass(NullableNull _) => new InheritedClass();

		 // Implement this in inherited class to allow implicit InheritedClass => T
		 public static implicit operator T(InheritedClass value) => value.Value;

		 // Implement this in inherited class to allow implicit InheritedClass => T?, only possible if T is a value type
		 public static implicit operator T?(InheritedClass value) => value.HasValue ? value.Value : new T?();
		 
		*/
	}
	
	/// <summary>
	/// Set NullableUnity field to this to set it to null
	/// </summary>
	public struct NullableNull { }
}