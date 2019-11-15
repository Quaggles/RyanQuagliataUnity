#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using RyanQuagliataUnity.Extensions.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.TextMeshPro {
	public class TextToTextMeshProSwapper : ComponentSwapper<Text, TextMeshProUGUI> {
		[MenuItem("RyanQuagliata/Component Swapper/TextToTextMeshProSwapper")]
		public static void Create() => GetWindow<TextToTextMeshProSwapper>().Show();
		
		[BoxGroup("FontMapping", order:999, CenterLabel = true)]
		public List<TMP_FontAsset> Fonts = new List<TMP_FontAsset>();

		[BoxGroup("FontMapping")]
		public TMP_FontAsset DefaultFont;

		[Button]
		[BoxGroup("FontMapping")]
		void FillFonts() => Fonts.AddRange(FindAssetsByType<TMP_FontAsset>().Where(x => !Fonts.Contains(x)));

		public override void CopyComponents(Text source, TextMeshProUGUI destination) {
			destination.text = source.text;
			destination.fontSize = source.fontSize;
			destination.autoSizeTextContainer = source.resizeTextForBestFit;
			destination.fontSizeMin = source.resizeTextMinSize;
			destination.fontSizeMax = source.resizeTextMaxSize;
			destination.alignment = GetAlignmentMapping(source.alignment);
			destination.fontStyle = GetStyleMapping(source.fontStyle);
			destination.color = source.color;
			destination.raycastTarget = source.raycastTarget;
			destination.lineSpacing = source.lineSpacing;
			destination.richText = source.supportRichText;
			destination.overflowMode = GetWrappingMapping(source.verticalOverflow);
			destination.enableWordWrapping = source.horizontalOverflow == HorizontalWrapMode.Wrap;
			try {
				bool Predicate(TMP_FontAsset x) => GetFont(x) == source.font;
				Font GetFont(TMP_FontAsset x) {
					if (x.sourceFontFile != null) return x.sourceFontFile;
					foreach (var tmpFontAsset in x.fallbackFontAssetTable) {
						var font = GetFont(tmpFontAsset);
						if (font != null) return font;
					}
					return null;
				}

				destination.font = Fonts.First(Predicate);
			} catch (Exception ex) {
				Debug.LogWarning($"Couldn't find TMP_FontAsset for {source.font.name}: {ex}");
				if (DefaultFont) {
					destination.font = DefaultFont;
					Debug.LogWarning($"\tUsing default {DefaultFont}");
				}
			}
		}

		// Store size since TextMeshProUGUI.Awake changes the size for some reason
		private Vector2 sizeDelta;
		public override void PreCopy(Text that) {
			base.PreCopy(that);
			sizeDelta = that.rectTransform.sizeDelta;
		}

		public override void PostCopy(TextMeshProUGUI that) {
			base.PostCopy(that);
			that.rectTransform.sizeDelta = sizeDelta;
		}

		public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object {
			List<T> assets = new List<T>();
			string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
			for (int i = 0; i < guids.Length; i++) {
				string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
				T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
				if (asset != null) {
					assets.Add(asset);
				}
			}

			return assets;
		}

		private (TextAnchor, TextAlignmentOptions)[] AlignmentMapping = {
			(TextAnchor.UpperLeft, TextAlignmentOptions.TopLeft),
			(TextAnchor.UpperCenter, TextAlignmentOptions.Top),
			(TextAnchor.UpperRight, TextAlignmentOptions.TopRight),
			(TextAnchor.MiddleLeft, TextAlignmentOptions.Left),
			(TextAnchor.MiddleCenter, TextAlignmentOptions.Center),
			(TextAnchor.MiddleRight, TextAlignmentOptions.Right),
			(TextAnchor.LowerLeft, TextAlignmentOptions.BottomLeft),
			(TextAnchor.LowerCenter, TextAlignmentOptions.Bottom),
			(TextAnchor.LowerRight, TextAlignmentOptions.BottomRight),
		};

		TextAlignmentOptions GetAlignmentMapping(TextAnchor that) => AlignmentMapping.First(x => x.Item1 == that).Item2;

		private (FontStyle, FontStyles)[] StyleMapping = {
			(FontStyle.Bold, FontStyles.Bold),
			(FontStyle.Italic, FontStyles.Italic),
			(FontStyle.Normal, FontStyles.Normal),
			(FontStyle.BoldAndItalic, FontStyles.Bold | FontStyles.Italic),
		};

		FontStyles GetStyleMapping(FontStyle that) => StyleMapping.First(x => x.Item1 == that).Item2;
		
		private (VerticalWrapMode, TextOverflowModes)[] WrappingMapping = {
			(VerticalWrapMode.Truncate, TextOverflowModes.Truncate),
			(VerticalWrapMode.Overflow, TextOverflowModes.Overflow),
		};

		TextOverflowModes GetWrappingMapping(VerticalWrapMode that) => WrappingMapping.First(x => x.Item1 == that).Item2;
	}
}
#endif