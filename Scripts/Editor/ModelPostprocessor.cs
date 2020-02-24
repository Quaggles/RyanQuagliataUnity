#if UNITY_VECTOR_GRAPHICS
using Unity.VectorGraphics.Editor;
#endif
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity.Editor {
	public class ModelPostprocessor : AssetPostprocessor {
		/// <summary>
		/// Disabled import materials for new models
		/// </summary>
		public void OnPreprocessModel() {
			ModelImporter modelImporter = (ModelImporter) assetImporter;
			string name = modelImporter.assetPath.ToLower();

			if (File.Exists(AssetDatabase.GetTextMetaFilePathFromAssetPath(name))) return;
		}

		/// <summary>
		/// Automatically creates prefab variants for newly imported models
		/// </summary>
		/// <param name="importedAssets"></param>
		/// <param name="deletedAssets"></param>
		/// <param name="movedAssets"></param>
		/// <param name="movedFromAssetPaths"></param>
		public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
			string[] movedFromAssetPaths) {
			// Only check imported assets
			foreach (var path in importedAssets) {
				// Load the asset from the path
				var prefab = AssetDatabase.LoadMainAssetAtPath(path);
				if (prefab == null) {
					Debug.LogError($"Failed to loading asset at path {path}");
					continue;
				}
				
				// Make sure this is a prefab
				if (PrefabUtility.GetPrefabInstanceStatus(prefab) != PrefabInstanceStatus.Connected) return;
				
				// Get the type of asset
				var assetType = PrefabUtility.GetPrefabAssetType(prefab);

				// If it's not a model, skip
				if (assetType != PrefabAssetType.Model) continue;

#if UNITY_VECTOR_GRAPHICS
				// Skip SVG files
				if (AssetImporter.GetAtPath(path) is SVGImporter) return;
#endif

				var prefabPath = Path.ChangeExtension(path, ".prefab");
				if (File.Exists(Path.GetFullPath(prefabPath))) continue;

				// Create a prefab variant
				var instanceRoot = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
				if (instanceRoot == null) {
					Debug.LogError("Failed to instantiate prefab");
					continue;
				}

				// Save prefab variant next to model
				PrefabUtility.SaveAsPrefabAsset(instanceRoot, prefabPath);

				if (Application.isPlaying)
					Object.Destroy(instanceRoot);
				else
					Object.DestroyImmediate(instanceRoot);
			}
		}
	}
}