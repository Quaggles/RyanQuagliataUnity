using System.Runtime.CompilerServices;
using Tayx.Graphy;
using UnityEngine;
using UnityEngine.UI;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace RyanQuagliataUnity.Extensions.Graphy {
	[RequireComponent(typeof(Image))]
	[ExecuteInEditMode]
	public class GraphGraphic : MonoBehaviour {
#if ODIN_INSPECTOR
		[Required]
#endif
		public Image Image;

		private G_GraphShader graph = new G_GraphShader();
#if ODIN_INSPECTOR
		[MinValue(0), MaxValue("@arrayMaxSize")]
#endif
		public int Resolution = 512;
#if ODIN_INSPECTOR
		[ShowInInspector, ReadOnly]
#endif
		private int arrayMaxSize;

		public bool DebugSinOutput = false;

#if ODIN_INSPECTOR
		[MinMaxSlider("@Range.x", "@Range.y", true)]
		[OnValueChanged(nameof(SetThresholds))]
#endif
		public Vector2 Thresholds = new Vector2(0.7f, 1);

#if ODIN_INSPECTOR
		[OnValueChanged(nameof(UpdateColours))]
#endif
		public Color GoodColour = Color.green;
		public Color CautionColor = Color.yellow;
		public Color CriticalColor = Color.red;

		public Vector2 Range = Vector2.up;

		public enum Threshold {
			Good,
			Caution,
			Critical
		}

#if ODIN_INSPECTOR
		[EnumToggleButtons, OnValueChanged(nameof(Initialize))]
#endif
		public GraphyManager.Mode Mode = GraphyManager.Mode.FULL;

		private Shader shaderFull;
		private Shader shaderMobile;

		public bool ShowAverage = true;
#if ODIN_INSPECTOR
		[EnableIf(nameof(ShowAverage)), Indent]
#endif
		public bool AutomaticAverage = true;

		private bool initialized = false;

		void Awake() {
			initialized = false;
		}

		void Start() => Initialize();

		void Initialize() {
			if (!Image) Image = GetComponent<Image>();
			shaderFull = Shader.Find("Graphy/Graph Standard");
			shaderMobile = Shader.Find("Graphy/Graph Mobile");
			if (!Image) return;
			graph = new G_GraphShader();
			graph.Image = Image;
			graph.Image.material = new Material(Mode == GraphyManager.Mode.FULL ? shaderFull : shaderMobile);
			arrayMaxSize = Mode == GraphyManager.Mode.FULL ? 512 : 128;
			graph.ArrayMaxSize = arrayMaxSize;
			Resolution = Mathf.Clamp(Resolution, 0, arrayMaxSize);
			VerifyArray();
			Clear();
			graph.InitializeShader();
			graph.GoodColor = GoodColour;
			graph.GoodThreshold = Thresholds.x;
			graph.CautionColor = CautionColor;
			graph.CautionThreshold = Thresholds.y;
			graph.CriticalColor = CriticalColor;
			UpdateColours();
			initialized = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void VerifyArray() {
			if (graph.Array == null || graph.Array.Length != Resolution) graph.Array = new float[Resolution];
		}

		public void Clear() {
			for (int i = 0; i < Resolution; i++) graph.Array[i] = 0;

			graph.UpdateArray();
			graph.UpdatePoints();
		}

		public void SubmitValue(float value) {
			if (!initialized) return;
			VerifyArray();
			RemapToRange(ref value);
			// Moves all old values up the array
			for (int i = 0; i < graph.Array.Length - 1; i++) graph.Array[i] = graph.Array[i + 1];

			// Puts the new value at the end
			graph.Array[graph.Array.Length - 1] = value;

			graph.UpdateArray();
			graph.UpdatePoints();
		}

		void RemapToRange(ref float value) => value = Mathf.InverseLerp(Range.x, Range.y, value);

		public void SubmitAverage(float value) {
			if (!initialized) return;
			RemapToRange(ref value);
			AutomaticAverage = false;
			VerifyArray();
			graph.Average = value;
			graph.UpdateAverage();
		}

		public void SetThreshold(Threshold threshold, float value) {
			if (!initialized) return;
			RemapToRange(ref value);
			switch (threshold) {
				case Threshold.Good:
					graph.GoodThreshold = value;
					break;
				case Threshold.Caution:
					graph.CautionThreshold = value;
					break;
			}

			graph.UpdateThresholds();
		}
		
		public void SetThresholds(Vector2 thresholds) {
			if (!initialized) return;
			RemapToRange(ref thresholds.x);
			RemapToRange(ref thresholds.y);
			graph.GoodThreshold = thresholds.x;
			graph.CautionThreshold = thresholds.y;
			graph.UpdateThresholds();
		}

		public void SetThresholdColour(Threshold threshold, Color value) {
			if (!initialized) return;
			switch (threshold) {
				case Threshold.Good:
					graph.GoodColor = value;
					break;
				case Threshold.Caution:
					graph.CautionColor = value;
					break;
				case Threshold.Critical:
					graph.CriticalColor = value;
					break;
			}

			UpdateColours();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void UpdateColours() => graph.UpdateColors();

		void Update() {
			if (!Application.isPlaying) return;
			if (!initialized) return;
			if (!ShowAverage) {
				if (graph.Average > 0) {
					graph.Average = -1;
					graph.UpdateAverage();
				}

				return;
			}

			if (!AutomaticAverage) return;
			VerifyArray();
			graph.Average = 0;
			for (var i = 0; i < graph.Array.Length; i++) graph.Average += graph.Array[i];
			graph.Average /= graph.Array.Length;
			graph.UpdateAverage();
		}

		void FixedUpdate() {
			if (!Application.isPlaying) return;
			if (!initialized) return;
			if (DebugSinOutput) {
				VerifyArray();
				SubmitValue(Mathf.InverseLerp(-1, 1, Mathf.Sin(Time.time)));
			}
		}
	}
}