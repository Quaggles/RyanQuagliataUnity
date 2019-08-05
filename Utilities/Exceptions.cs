using System;
using UnityEngine;

namespace RyanQuagliataUnity.Utilities {
	public static class PlayerAssertions {
		public static void AssertEditModeOnly(string methodName) {
			if (Application.isPlaying) throw new EditModeOnlyException(methodName);
		}
		
		public static void AssertPlayModeOnly(string methodName) {
			if (!Application.isPlaying) throw new PlayModeOnlyException(methodName);
		}
	}
	
	public class EditorOnlyException : Exception {
		public string MethodName;
		public EditorOnlyException(string methodName) {
			MethodName = methodName;
		}
		
		public override string Message => $"The method '{MethodName}' can only be used while in the editor";
	}
	
	public class EditModeOnlyException : Exception {
		public string MethodName;
		public EditModeOnlyException(string methodName) {
			MethodName = methodName;
		}
		
		public override string Message => $"The method '{MethodName}' can only be used in edit mode";
	}
	
	public class PlayModeOnlyException : Exception {
		public string MethodName;
		public PlayModeOnlyException(string methodName) {
			MethodName = methodName;
		}
		
		public override string Message => $"The method '{MethodName}' can only be used in play mode";
	}
}
