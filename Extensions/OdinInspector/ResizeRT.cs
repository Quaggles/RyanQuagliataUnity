using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace RyanQuagliataUnity.Extensions.OdinInspector {
	[ExecuteInEditMode]
	public class ResizeRT : 
#if ODIN_INSPECTOR && !ODIN_EDITOR_ONLY
		SerializedMonoBehaviour {
#else
		MonoBehaviour {
#endif
		[Required]
		public RenderTexture RenderTexture;

		public enum RunMode {
			Manual,
			Awake,
			Start,
			Update
		}

		public enum RectSource {
			Screen,
			Delegate
		}

		[EnumToggleButtons]
		public RunMode ResizeRunMode;

		private bool IsUpdate => ResizeRunMode == RunMode.Update;
		
		[ShowIf(nameof(IsUpdate))][MinValue(1)][Indent]
		public int FrameInterval = 1;
		
		[ShowIf(nameof(IsUpdate))][Indent]
		public bool ExecuteInEditMode = false;

		[EnumToggleButtons]
		public RectSource RenderTextureSizeSource;

		private bool IsDelegate => RenderTextureSizeSource == RectSource.Delegate;

		[ShowIf(nameof(IsDelegate))]
		[Indent]
		public Func<Vector2Int> GetNewSizeInt;
		
		[ShowIf(nameof(IsDelegate))]
		[Indent]
		public Func<Vector2> GetNewSize;

		// Start is called before the first frame updatea
		void Awake() {
			if (!Application.isPlaying) return;
			if (ResizeRunMode == RunMode.Awake) Resize();
		}

		// Start is called before the first frame update
		void Start() {
			if (!Application.isPlaying) return;
			if (ResizeRunMode == RunMode.Start) Resize();
		}

		// Update is called once per frame
		void Update() {
			if (ResizeRunMode == RunMode.Update) {
				if (!Application.isPlaying && !ExecuteInEditMode) return;
				if (FrameInterval > 1 && Time.frameCount % FrameInterval != 0) return;
				Resize();
			}
		}

		[Button]
		private void Resize() {
			var rtSize = new Vector2Int(RenderTexture.width, RenderTexture.height);
			var targetSize = GetTargetSize(RenderTextureSizeSource);
			if (rtSize != targetSize) {
				RenderTexture.DiscardContents();
				RenderTexture.Release();
				RenderTexture.width = targetSize.x;
				RenderTexture.height = targetSize.y;
				RenderTexture.Create();
			}
		}

		private Vector2Int GetTargetSize(RectSource rectSource) {
			switch (rectSource) {
				case RectSource.Screen:
					return new Vector2Int(Screen.width, Screen.height);
				case RectSource.Delegate:
					if (GetNewSizeInt != null) return GetNewSizeInt.Invoke();
					if (GetNewSize != null) return RoundToInt(GetNewSize.Invoke());
					return Vector2Int.one;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private Vector2Int RoundToInt(Vector2 vector2) => new Vector2Int(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
	}
}