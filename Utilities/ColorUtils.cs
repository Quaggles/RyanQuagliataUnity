using UnityEngine;

namespace RyanQuagliataUnity.Utilities {
	public static class ColorUtils {
		public static bool TryParse(string hexString, out UnityEngine.Color color) {
			try {
				color = Parse(hexString);
				return true;
			} catch {
				color = default;
				return false;
			}
		}
		
		public static Color Parse(string hexString) {
			if (hexString.StartsWith("#")) hexString = hexString.Substring(1);

			if (hexString.StartsWith("0x")) hexString = hexString.Substring(2);

			if (hexString.Length < 6) throw new System.Exception(string.Format("{0} is not a valid color string.", hexString));

			byte r = byte.Parse(hexString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hexString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hexString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			byte a = 255;
			if (hexString.Length >= 8) a = byte.Parse(hexString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

			return new Color32(r, g, b, a);
		}
	}
}