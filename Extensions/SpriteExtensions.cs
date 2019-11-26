using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class SpriteExtensions {
		private static Sprite clearSprite;
		public static Sprite ClearSprite => clearSprite != null ? clearSprite : clearSprite = Create(Texture2DExtensions.CreateSolidTexture2D(1, 1, new Color32(1, 1, 1, 0)));
		
		private static Sprite whiteSprite;
		public static Sprite WhiteSprite => whiteSprite != null ? whiteSprite : whiteSprite = Create(Texture2DExtensions.CreateSolidTexture2D(1, 1, Color.white));

		
		private static Sprite blackSprite;
		public static Sprite BlackSprite => blackSprite != null ? blackSprite : blackSprite = Create(Texture2DExtensions.CreateSolidTexture2D(1, 1, Color.black));
		
		/// <summary>
		/// Sprite.Create but with less pointless arguments
		/// </summary>
		/// <param name="texture2D"></param>
		/// <param name="pixelsPerUnit"></param>
		/// <returns></returns>
		public static Sprite Create(Texture2D texture2D, float pixelsPerUnit = 100f) => texture2D.CreateSprite(pixelsPerUnit);
	}
}