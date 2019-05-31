using UnityEngine;

namespace RyanQuagliata.Utilities {
	public class Text {
		/// <summary>
		/// Gets the length of a text string in pixels
		/// </summary>
		/// <param name="text"> Text UI object </param>
		/// <returns> Number of pixels wide the text object is </returns>
		public static int GetTextLengthPixels(UnityEngine.UI.Text text){
			return GetTextLengthPixels(text.font, text.text, text.fontSize);
		}

		/// <summary>
		/// Gets the length of a text string in pixels
		/// </summary>
		/// <param name="font"> Font style this text is using </param>
		/// <param name="text"> Text string to analyse </param>
		/// <param name="fontSize"> Font size </param>
		/// <returns></returns>
		public static int GetTextLengthPixels(Font font, string text, int fontSize){
			int totalLength = 0;
			CharacterInfo charInfo;
			font.RequestCharactersInTexture(text, fontSize);

			// Loop through every character in the string and add widths
			for (int i = 0; i < text.Length; i++) {
				font.GetCharacterInfo(text[i], out charInfo, fontSize);
				totalLength += charInfo.advance;
			}

			return totalLength;
		}
	}
}
