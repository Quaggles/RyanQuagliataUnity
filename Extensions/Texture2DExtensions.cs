using System;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class Texture2DExtensions {
		public enum ExportFormat {
			Png,
			Jpg,
			Exr
		}

		public static byte[] EncodeToFormat(this Texture2D texture2D, ExportFormat exportFormat) {
			switch (exportFormat) {
				case ExportFormat.Png:
					return texture2D.EncodeToPNG();
				case ExportFormat.Jpg:
					return texture2D.EncodeToJPG();
				case ExportFormat.Exr:
					return texture2D.EncodeToEXR();
				default:
					throw new ArgumentOutOfRangeException(nameof(exportFormat), exportFormat, null);
			}
		}

		/// <summary>
		/// Once the total pixel count exceeds this it's probably better to use SetPixels and pay the garbage allocation cost of the array 
		/// </summary>
		private const int SET_PIXELS_PIXEL_COUNT_THRESHOLD = 4*4;

		public static Texture2D CreateSolidTexture2D(int width, int height, Color color) {
			var tex = new Texture2D(width, height);
			return width * height < SET_PIXELS_PIXEL_COUNT_THRESHOLD ? FillTexture2DSequential(tex, color) : FillTexture2DBlock(tex, color);
		}

		public static Texture2D CreateSolidTexture2D(int width, int height, Color32 color) {
			var tex = new Texture2D(width, height);
			return width * height < SET_PIXELS_PIXEL_COUNT_THRESHOLD ? FillTexture2DSequential(tex, color) : FillTexture2DBlock(tex, color);
		}

		/// <summary>
		/// Sprite.Create but with less pointless arguments
		/// </summary>
		/// <param name="that"></param>
		/// <param name="pixelsPerUnit"></param>
		/// <returns></returns>
		public static Sprite CreateSprite(this Texture2D that, float pixelsPerUnit = 100f) => Sprite.Create(that, new Rect(Vector2.zero, new Vector2(that.width, that.height)), Vector2.one * 0.5f, pixelsPerUnit);

		/// <summary>
		/// Fills a texture with a colour one pixel at a time, no garbage created but takes longer
		/// </summary>
		/// <param name="that"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Texture2D FillTexture2DSequential(this Texture2D that, Color color) {
			for (int x = 0; x < that.width; x++)
				for (int y = 0; y < that.height; y++)
					that.SetPixel(x, y, color);

			that.Apply();
			return that;
		}

		/// <summary>
		/// Fills a texture with a colour in a single block array, creates garbage but will be faster with larger resolutions
		/// </summary>
		/// <param name="that"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Texture2D FillTexture2DBlock(this Texture2D that, Color color) {
			var colours = new Color[that.width * that.height];
			for (var i = 0; i < colours.Length; i++) {
				colours[i] = color;
			}

			that.SetPixels(colours);
			that.Apply();

			return that;
		}
		
		/// <summary>
		/// Fills a texture with a colour in a single block array, creates garbage but will be faster with larger resolutions
		/// </summary>
		/// <param name="that"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Texture2D FillTexture2DBlock(this Texture2D that, Color32 color) {
			var colours = new Color32[that.width * that.height];
			for (var i = 0; i < colours.Length; i++) {
				colours[i] = color;
			}

			that.SetPixels32(colours);
			that.Apply();

			return that;
		}
	}
}