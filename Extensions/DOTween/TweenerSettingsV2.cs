using System;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using RyanQuagliataUnity.Utilities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events; 

namespace RyanQuagliataUnity.Extensions.DOTween {
	[Serializable]
	public class TweenerSettingsV2 : ITweenerConfig {
		[SuffixLabel("seconds"), MinValue(0), Tooltip("The length of time it will take for the tween to reach the end value")]
		public float Duration = 1;

		[SuffixLabel("seconds"), MinValue(0), Tooltip("The delay it will take for the tween to start playing for the first time")]
		public float Delay = 0;
	
		[Button("▲"), HorizontalGroup("Ease", 20)]
		private void PrevEase() => EaseFunction = EaseFunction.GetOffset(-1);

		[Button("▼"), HorizontalGroup("Ease", 20)]
		private void NextEase() => EaseFunction = EaseFunction.GetOffset(1);

		public bool IsEaseBidirectional(Ease ease) => ease.GetName().StartsWith("In Out");

		public Ease GetEaseVariant(Ease ease, bool inEase) {
			if (!IsEaseBidirectional(ease)) return ease;
			if (inEase) return ease.GetOffset(-2);
			else return ease.GetOffset(-1);
		}

		[Tooltip("Ease Function this tween will use to reach the end value, for a visualisation go to:\nwww.easings.net"), 
		 HorizontalGroup("Ease"), OnInspectorGUI("EaseFunctionPreview", append:true)]
		public Ease EaseFunction = Ease.Linear;
	
		/// <summary>
		/// Draws a preview of the Ease Function Curve
		/// </summary>
		private void EaseFunctionPreview () {
#if UNITY_EDITOR
			GUILayout.BeginVertical();
			var curve = CreateNewEasingCurve(EaseFunction, 100);
			if (curve != null) {
				UnityEditor.EditorGUILayout.CurveField(curve, GUILayout.Height(100));
			} else {
				GUILayout.Label("This Ease Function has no curve");
			}
			GUILayout.EndVertical();
#endif
		}
		
		private AnimationCurve CreateNewEasingCurve(Ease ease, int timesteps) {
			if (timesteps <= 0) return null;
		
			EaseFunction easeFunction = EaseManager.ToEaseFunction(ease);
			var curve = new AnimationCurve();
		
			// Adds all the required keys
			for (int i = 0; i < timesteps; i++) {
				float time = MathfExtensions.Remap(i, 0, timesteps, 0, Duration);
				var result = EaseManager.Evaluate(ease, easeFunction, time, Duration, EaseOvershoot, EasePeriod);
				curve.AddKey(time, result);
			}
		
#if UNITY_EDITOR
			// Set the tangent mode to Clamped Auto for all keys
			for (int i = 0; i < curve.length; i++) {
				var tangentMode = UnityEditor.AnimationUtility.TangentMode.Linear;
				UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, i, tangentMode);
				UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, i, tangentMode);
			}
#endif

			return curve;
		}
	
		[Indent, MinValue(0), ShowIf("ShowOvershoot"), LabelText("$OvershootLabel"),
		 PropertyTooltip("$OvershootTooltip"), SuffixLabel("$OvershootSuffix"), CustomContextMenu("Reset", "ResetOvershoot"),
		 InfoBox("Flash count will be rounded", "IsFlashNotWhole"),
		 InfoBox("Flash will end on the ending value", "IsFlashEven"),
		 InfoBox("Flash will end on the original value", "IsFlashOdd")]
		public float EaseOvershoot = 1.70158f;
	
		/// <summary>
		/// Returns true if the current EaseFunction uses the overshoot/amplitude value
		/// </summary>
		private bool ShowOvershoot {
			get {
				switch (EaseFunction) {
					case Ease.InElastic:
					case Ease.OutElastic:
					case Ease.InOutElastic:
					case Ease.InBack:
					case Ease.OutBack:
					case Ease.InOutBack:
					case Ease.Flash:
					case Ease.InFlash:
					case Ease.OutFlash:
					case Ease.InOutFlash:
						return true;
					default: return false;
				}
			}
		}
	
		/// <summary>
		/// Gets the correct label for the OvershootO field based on the current EaseFunction
		/// </summary>
		private string OvershootLabel {
			get {
				switch (EaseFunction) {
					case Ease.InElastic:
					case Ease.OutElastic:
					case Ease.InOutElastic:
					case Ease.InOutBack:
						return "Overshoot";
					case Ease.InBack:
					case Ease.OutBack:
						return "Amplitude";
					case Ease.Flash:
					case Ease.InFlash:
					case Ease.OutFlash:
					case Ease.InOutFlash:
						return "Flash Count";
					default: return "";
				}
			}
		}
	
		/// <summary>
		/// Gets the correct tooltip for the Overshoot field based on the current EaseFunction
		/// </summary>
		private string OvershootTooltip {
			get {
				switch (EaseFunction) {
					case Ease.InElastic:
					case Ease.OutElastic:
					case Ease.InOutElastic:
						return "A multiplier for how far the value will travel past the end value before being pulled back by the elastic function, " +
						       "if tweening from 0 => 2 a multiplier of 2 would overshoot the value to 4";
					case Ease.InOutBack:
					case Ease.InBack:
					case Ease.OutBack:
						return "A multiplier far the value will overshoot the end value before slowly easing back";
					case Ease.Flash:
					case Ease.InFlash:
					case Ease.OutFlash:
					case Ease.InOutFlash:
						return "An intenger that sets the total number of flashes that will happen.\n" +
						       "Using an even number will complete the tween on the starting value, while an odd one will complete it on the end value";
					default: return "Disabled because the selected easing function does not use this value";
				}
			}
		}
	
		/// <summary>
		/// Gets the correct suffix string for the ease overshoot based on Ease Function
		/// </summary>
		private string OvershootSuffix {
			get {
				switch (EaseFunction) {
					case Ease.Flash:
					case Ease.InFlash:
					case Ease.OutFlash:
					case Ease.InOutFlash:
						return "x Flashes";
					default:
						return "Multiplier";
				}
			}
		}


		private bool IsFlashEase {
			get {
				switch (EaseFunction) {
					case Ease.Flash:
					case Ease.InFlash:
					case Ease.OutFlash:
					case Ease.InOutFlash:
						return true;
					default:
						return false;
				}
			}
		}
	
		private bool IsFlashNotWhole => IsFlashEase && EaseOvershoot % 1 != 0;
		private bool IsFlashEven => IsFlashEase && Mathf.RoundToInt(EaseOvershoot) % 2 == 0;
		private bool IsFlashOdd => IsFlashEase && !IsFlashEven;

		private void ResetOvershoot() => EaseOvershoot = 1.70158f;

		[Indent, ShowIf("ShowPeriod"), LabelText("$PeriodLabel"), PropertyTooltip("$PeriodTooltip")]
		public float EasePeriod = 0;
	
		/// <summary>
		/// Returns true if the current EaseFunction uses the period value
		/// </summary>
		private bool ShowPeriod {
			get {
				switch (EaseFunction) {
					case Ease.InElastic:
					case Ease.OutElastic:
					case Ease.InOutElastic:
					case Ease.Flash:
					case Ease.InFlash:
					case Ease.OutFlash:
					case Ease.InOutFlash:
						return true;
					default: return false;
				}
			}
		}
	
		/// <summary>
		/// Gets the correct label for the Period field based on the current Ease Function
		/// </summary>
		private string PeriodLabel {
			get {
				switch (EaseFunction) {
					case Ease.InElastic:
					case Ease.OutElastic:
					case Ease.InOutElastic:
						return "Period";
					case Ease.Flash:
					case Ease.InFlash:
					case Ease.OutFlash:
					case Ease.InOutFlash:
						return
							"Flash Power";
					default: return "";
				}
			}
		}
	
		/// <summary>
		/// Gets the correct tooltip for the Period field based on the current Ease Function
		/// </summary>
		private string PeriodTooltip {
			get {
				switch (EaseFunction) {
					case Ease.InElastic:
					case Ease.OutElastic:
					case Ease.InOutElastic:
						return "Changes how quickly the elastic effect occurs, a value of 0 will default to 0.3, " +
						       "negative values result in the absolute value being used";
					case Ease.Flash:
					case Ease.InFlash:
					case Ease.OutFlash:
					case Ease.InOutFlash:
						return
							"Indicates the power in time of the ease\n\n" +
							"__——‾‾\t-1 strengthens the ease over time\n\n" +
							"—————\t0 is balanced\n\n" +
							"‾‾——__\t1 fully weakens the ease over time";
					default: return "Disabled because the selected easing function does not use this value";
				}
			}
		}

	
		[FoldoutGroup("Advanced Settings", false), SuffixLabel("iterations", true), MinValue(-1),
		 HorizontalGroup("Advanced Settings/Loops"), LabelWidth(40),
		 Tooltip("Number of iterations this tween will loop for, -1 will result in infinite loops"),
		 CustomContextMenu("Infinite", "SetInfiniteLoopCount")]
		public int Loops;

		private void SetInfiniteLoopCount() => Loops = -1;

		private bool HasLoops => Loops != 0;
	
		[FoldoutGroup("Advanced Settings"), EnableIf("HasLoops"), HorizontalGroup("Advanced Settings/Loops"),
		 LabelWidth(70), Tooltip("Loop behaviour type when it has completed one iteration")]
		public LoopType LoopType;
	
		[FoldoutGroup("Advanced Settings"), HorizontalGroup("Advanced Settings/Stop Motion", .4f),
		 LabelText("Do Stop Motion"), LabelWidth(95),
		 InfoBox("Warning: Using this will disable any overshoot/amplitude and period settings", InfoMessageType.Warning, "StopMotionEase")]
		public bool StopMotionEase = false;
	
		[FoldoutGroup("Advanced Settings"), HorizontalGroup("Advanced Settings/Stop Motion"), LabelText("Framerate"),
		 EnableIf("StopMotionEase"), MinValue(1), SuffixLabel("fps"), LabelWidth(65)]
		public int StopMotionEaseFrameRate = 60;
	
		[FoldoutGroup("Advanced Settings"), Tooltip("At what point in the Unity update cycle will this Tweens setter be called")]
		public UpdateType UpdateType;
	
		[FoldoutGroup("Advanced Settings"), Tooltip("If true the tween will not be reusable after is has been completed")]
		public bool AutoKill = true;
	
		[InfoBox("These will be overridden if the <i>OnX()</i> function is called in code e.g.:\n" +
		         "    <i>OnStart(<b>action</b>);</i>\n\n" +
		         "If you want to add additional callbacks without overriding these use:\n" +
		         "    <i>onStart += <b>action</b>;</i>")]
		[FoldoutGroup("Advanced Settings/Events", false), SerializeField] public UnityEvent OnStart = new UnityEvent();
		[FoldoutGroup("Advanced Settings/Events", false), SerializeField] public UnityEvent OnKill = new UnityEvent();
		[FoldoutGroup("Advanced Settings/Events", false), SerializeField] public UnityEvent OnPlay = new UnityEvent();
		[FoldoutGroup("Advanced Settings/Events", false), SerializeField] public UnityEvent OnPause = new UnityEvent();
		[FoldoutGroup("Advanced Settings/Events", false), SerializeField] public UnityEvent OnRewind = new UnityEvent();
		[FoldoutGroup("Advanced Settings/Events", false), SerializeField] public UnityEvent OnStepComplete = new UnityEvent();
		[FoldoutGroup("Advanced Settings/Events", false), SerializeField] public UnityEvent OnComplete = new UnityEvent();

		public TweenerSettingsV2 GetConfig => this;
	}
}