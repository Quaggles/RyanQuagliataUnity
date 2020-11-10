using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace RyanQuagliataUnity.Scripts {
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[Serializable]
	public struct AssemblyDefinition {
		[Serializable]
		public struct VersionDefine {
			public string name;
			public string expression;
			public string define;
		}
		
		public bool allowUnsafeCode;
		public bool autoReferenced;
		public string[] defineConstraints;
		public string[] excludePlatforms;
		public string[] includePlatforms;
		public string name;
		public bool noEngineReferences;
		public string[] optionalUnityReferences;
		public bool overrideReferences;
		public string[] precompiledReferences;
		public string[] references;
		public VersionDefine[] versionDefines;
		
#if UNITY_EDITOR
		public static implicit operator AssemblyDefinition(UnityEditorInternal.AssemblyDefinitionAsset that) =>
			JsonUtility.FromJson<AssemblyDefinition>(that.text);
#endif
		
		public override string ToString() => JsonUtility.ToJson(this, true);

		public string ToString(bool pretty) => JsonUtility.ToJson(this, pretty);
	}

	public static class AssemblyDefinitionExtensions {
#if UNITY_EDITOR
		public static AssemblyDefinition Convert(this UnityEditorInternal.AssemblyDefinitionAsset that) => that;
#endif
	}
}