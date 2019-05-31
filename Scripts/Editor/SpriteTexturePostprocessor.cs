using System.IO;
using UnityEditor;

namespace RyanQuagliata.Scripts.Editor {
	/// <summary>
	/// Imports textures located in a folder containing the string "sprite" as sprites and enables mipmaps
	/// </summary>
	public class SpriteTexturePostprocessor : AssetPostprocessor {
		void OnPreprocessTexture() {
			string path = assetPath.ToLower();
			if (File.Exists(AssetDatabase.GetTextMetaFilePathFromAssetPath(path)))
				return;
			if (path.Contains("sprite")) {
				TextureImporter textureImporter  = (TextureImporter)assetImporter;
				textureImporter.textureType = TextureImporterType.Sprite;
				textureImporter.mipmapEnabled = true;
			}
		}
	}
}
