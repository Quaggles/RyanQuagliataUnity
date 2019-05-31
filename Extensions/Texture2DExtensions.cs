using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace RyanQuagliata.Extensions {
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
		
	}
}