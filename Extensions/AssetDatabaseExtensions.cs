using UnityEngine;

namespace RyanQuagliata.Extensions {
	public class AssetDatabaseExtensions {
		public static T[] GetScriptableObjects<T>() where T : ScriptableObject {
#if UNITY_EDITOR
			string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
			T[] assetGuids = new T[guids.Length];
			for (int i = 0; i < guids.Length; i++) {
				string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
				assetGuids[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
			}

			return assetGuids;
#else
			return new T[0];
#endif
		}
	}
}