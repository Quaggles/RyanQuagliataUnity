using System;
using System.Reflection;

namespace RyanQuagliata.Extensions {
	public static class MemberInfoExtensions {
		public static object GetValue(this MemberInfo that, object forObject) {
			switch (that.MemberType) {
				case MemberTypes.Field:
					return ((FieldInfo) that).GetValue(forObject);
				case MemberTypes.Property:
					return ((PropertyInfo) that).GetValue(forObject);
				default:
					throw new NotImplementedException();
			}
		}

		public static void SetValue(this MemberInfo that, object forObject, object value) {
			switch (that.MemberType) {
				case MemberTypes.Field:
					((FieldInfo) that).SetValue(forObject, value);
					break;
				case MemberTypes.Property:
					((PropertyInfo) that).SetValue(forObject, value);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		public static Type GetUnderlyingType(this MemberInfo member) {
			switch (member.MemberType) {
				case MemberTypes.Event:
					return ((EventInfo) member).EventHandlerType;
				case MemberTypes.Field:
					return ((FieldInfo) member).FieldType;
				case MemberTypes.Method:
					return ((MethodInfo) member).ReturnType;
				case MemberTypes.Property:
					return ((PropertyInfo) member).PropertyType;
				default:
					throw new ArgumentException
					(
						"Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
					);
			}
		}
		
		public static bool IsOverriden(this MethodInfo that) => that.GetBaseDefinition().DeclaringType != that.DeclaringType;
	}
}