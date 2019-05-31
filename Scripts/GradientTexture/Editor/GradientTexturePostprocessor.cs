//using UnityEditor;
//using UnityEngine;
//
//namespace RyanQuagliata.Editor {
//	public class GradientTexturePostprocessor : AssetPostprocessor {
//		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
//			string[] movedFromAssetPaths) {
//			foreach (var importedAsset in importedAssets) {
//				if (!importedAsset.EndsWith(".asset")) continue;
//				var gradientTexture = AssetDatabase.LoadAssetAtPath<GradientTexture.GradientTexture>(importedAsset);
//				if (gradientTexture == null) continue;
//				if (!gradientTexture.HasTextureSubAsset()) gradientTexture.CreateTextureSubAsset();
//				Debug.Log($"Updated Texture {gradientTexture}");
//			}
//		}
//	}
//}
