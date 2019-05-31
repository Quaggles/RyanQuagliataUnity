#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RyanQuagliata.Extensions.Editor {
	public static class EditorUtilityExtensions {
		
		public static int CopySerializedPolymorphic(Object source, Object dest) => CopySerializedPolymorphic(new SerializedObject(source), new SerializedObject(dest));


		/// <summary>
		/// Evolved from http://answers.unity.com/answers/1538287/view.html
		/// </summary>
		/// <param name="source"></param>
		/// <param name="dest"></param>
		/// <returns>The number of properties copied</returns>
		public static int CopySerializedPolymorphic(SerializedObject source, SerializedObject dest) {
			int counter = 0;
			var propIterator = source.GetIterator();
			//jump into serialized object, this will skip script type so that we dont override the destination component's type
			if (propIterator.NextVisible(true)) {
				while (propIterator.NextVisible(true)) //iterate through all serializedProperties
				{
					//try obtaining the property in destination component
					var propElement = dest.FindProperty(propIterator.name);

					//validate that the properties are present in both components, and that they're the same type
					if (propElement != null && propElement.propertyType == propIterator.propertyType) {
						//copy value from source to destination component
						dest.CopyFromSerializedProperty(propIterator);
						counter++;
					}
				}
			}
			
			dest.ApplyModifiedProperties();
			return counter;
		}

		public static int MatchingPropertyCount(SerializedObject source, SerializedObject dest) {
			if (source == null || dest == null) return 0;
			int counter = 0;
			var propIterator = source.GetIterator();
			//jump into serialized object, this will skip script type so that we dont override the destination component's type
			if (propIterator.NextVisible(true)) {
				while (propIterator.NextVisible(true)) //iterate through all serializedProperties
				{
					//try obtaining the property in destination component
					var propElement = dest.FindProperty(propIterator.name);

					//validate that the properties are present in both components, and that they're the same type
					if (propElement != null && propElement.propertyType == propIterator.propertyType) {
						//copy value from source to destination component
						dest.CopyFromSerializedProperty(propIterator);
						counter++;
					}
				}
			}

			return counter;
		}
	}
}
#endif