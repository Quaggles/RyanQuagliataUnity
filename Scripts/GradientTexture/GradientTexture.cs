using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using RyanQuagliata.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RyanQuagliata.GradientTexture {
	[CreateAssetMenu(menuName = "Ryan Quagliata/Gradient Texture", fileName = "Gradient", order = 1000)]
	public class GradientTexture : ScriptableObject {
		// The name used for the texture sub asset of the gradient
		private const string TEXTURE_NAME = "GradientTextureAsset"; 
		
		[OnValueChanged(nameof(GradientChanged))]
		public Gradient Gradient;
		
		[MinValue(2)][Delayed][OnValueChanged(nameof(ForceUpdate))]
		public int Width = 128;

		[MinValue(2)][Delayed][OnValueChanged(nameof(ForceUpdate))]
		public int Height = 2;

		private void ForceUpdate() => UpdateTexture();

		public enum Direction {
			Horizontal,
			Vertical
		}
		
		[EnumToggleButtons][OnValueChanged(nameof(ForceUpdate))]
		public Direction GradientDirection;


#if !UNITY_EDITOR
		public void UpdateTexture() { }
		private void OnGradientChanged() { }
		private void GradientChanged() { }
#else
		private bool gradientChanged = false;

		private void GradientChanged() => gradientChanged = true;

		[ReadOnly, ShowInInspector, InlineEditor(InlineEditorModes.LargePreview, Expanded = true), HideLabel]
		public Texture2D CachedTexture2D;
		
		public Texture2D PreviewTexture2D;

		/// <summary>
		/// Updates the gradient texture to reflect the new dimensions and gradient
		/// </summary>
		/// <exception cref="Exception"></exception>
		[InfoBox("Gradient has been modified since last export, requires update", InfoMessageType.Warning), Button("↻ Texture Update"), ShowIf(nameof(gradientChanged))]
		public void UpdateTexture() {
			CachedTexture2D = GetTextureSubAsset();
			if (CachedTexture2D == null) CachedTexture2D = CreateTextureSubAsset();
			if (CachedTexture2D == null) throw new Exception("Failed to retrieve internal gradient texture");
			
			// Create or update the preview texture
			if (PreviewTexture2D == null) PreviewTexture2D = new Texture2D(16, 16){name = "Preview"};
			UpdateTextureSubAsset(PreviewTexture2D, new Vector2Int(PreviewTexture2D.width, PreviewTexture2D.height));
			
			UpdateTextureSubAsset(CachedTexture2D, new Vector2Int(Width, Height));
			EditorUtility.SetDirty(CachedTexture2D);
			SceneView.RepaintAll();
		}

		/// <summary>
		/// Searches through the sub assets of the gradient asset to try and find the texture
		/// </summary>
		/// <returns>Texture2D of the gradient if found, null if not found</returns>
		private Texture2D GetTextureSubAsset() {
			var subs = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(this));
			foreach (var o in subs) {
				if (o is Texture2D texture2D && string.Equals(texture2D.name, TEXTURE_NAME)) return texture2D;
			}
			return null;
		}

		/// <summary>
		/// Creates a texture asset within the gradient asset
		/// </summary>
		/// <returns></returns>
		public Texture2D CreateTextureSubAsset() {
			var tex = new Texture2D(Width, Height) {name = TEXTURE_NAME};
			tex.Apply();
			AssetDatabase.AddObjectToAsset(tex, this);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			return tex;
		}

		/// <summary>
		/// Applies a gradient to a texture
		/// </summary>
		/// <param name="texture2D"></param>
		/// <param name="size"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public void UpdateTextureSubAsset([NotNull] Texture2D texture2D, Vector2Int size) {
			if (texture2D == null) throw new ArgumentNullException(nameof(texture2D));
			if (texture2D.width != size.x || texture2D.height != size.y) texture2D.Resize(size.x, size.y);
			
			// Colours array
			Color[] colours = new Color[GradientDirection == Direction.Horizontal ? size.x : size.y];
			
			// Fill colour array with gradient
			for (int i = 0; i < colours.Length; i++) colours[i] = Gradient.Evaluate(i.Remap(0, colours.Length, 0, 1));
			
			// Set for as many rows/columns as required
			if (GradientDirection == Direction.Horizontal) {
				for (int row = 0; row < size.y; row++) texture2D.SetPixels(0, row, size.x, 1, colours);
			} else {
				for (int col = 0; col < size.x; col++) texture2D.SetPixels(col, 0, 1, size.y, colours);
			}

			texture2D.Apply();
			gradientChanged = false;
		}

		public Texture2DExtensions.ExportFormat FileExportFormat;

		[Button]
		private void Save() {
			UpdateTexture();
			var relativeProjectPath = Path.ChangeExtension(AssetDatabase.GetAssetPath(this), FileExportFormat.ToString().ToLower());
			using (var fs = new FileStream(relativeProjectPath, FileMode.Create)) {
				using (var bw = new BinaryWriter(fs)) {
					if (CachedTexture2D == null) throw new Exception("No cached texture loaded, refresh");
					bw.Write(CachedTexture2D.EncodeToFormat(FileExportFormat));
				}
			}
			AssetDatabase.ImportAsset(relativeProjectPath, ImportAssetOptions.Default);
		}

		[Button]
		private void SaveAs() {
			UpdateTexture();
			var directory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
			var extension = FileExportFormat.ToString().ToLower();
			var absoluteOutputPath = EditorUtility.SaveFilePanel($"Export Gradient as {extension}", directory, name, extension);
			if (string.IsNullOrWhiteSpace(absoluteOutputPath)) return;
			using (var fs = new FileStream(absoluteOutputPath, FileMode.Create)) {
				using (var bw = new BinaryWriter(fs)) {
					if (CachedTexture2D == null) throw new Exception("No cached texture loaded, refresh");
					bw.Write(CachedTexture2D.EncodeToFormat(FileExportFormat));
				}
			}
			if (PathWithinProject(absoluteOutputPath)) AssetDatabase.ImportAsset(AbsolutePathToProjectRelativePath(absoluteOutputPath), ImportAssetOptions.Default);
		}

		/// <summary>
		/// Converts an absolute path to a path relative to the Assets folder
		/// </summary>
		/// <param name="path">Absolute path e.g. "C:\Users\quagliat\Documents\Git\Unity\QUTEducationPrecinctSphere\Assets\Textures\Gradient.asset"</param>
		/// <returns>Relative path e.g. "Textures\Gradient.asset"</returns>
		private string AbsolutePathToProjectRelativePath(string path) {
			var dataPath = Application.dataPath;
			var projectPathLength = dataPath.Substring(0, dataPath.Length - "Assets".Length).Length;
			return path.Substring(projectPathLength, path.Length - projectPathLength);
		}

		private bool PathWithinProject(string path) {
			return path.Contains(Application.dataPath);
		}

#endif
	}
}
