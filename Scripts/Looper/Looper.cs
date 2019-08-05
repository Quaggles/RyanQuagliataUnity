using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliataUnity.Looper {
	/// <summary>
	/// The type of delay that this loop will use between iterations
	/// </summary>
	public enum DelayType {
		/// <summary>
		/// Loop iterates every frame
		/// </summary>
		EveryUpdate,
		/// <summary>
		/// Loop iterates every fixed update cycle
		/// </summary>
		EveryFixedUpdate,
		/// <summary>
		/// Loop iterates ever N number of frames
		/// </summary>
		AfterFrameInterval,
		/// <summary>
		/// Loop iterates on a time delay
		/// </summary>
		AfterTimeDelay
	}
	
	[Serializable]
	public abstract class Looper {
		#region Static
		public static LooperHost Host => LooperHost.Instance;

		/// <summary>
		/// Starts a coroutine on the host monobehaviour
		/// </summary>
		/// <param name="looper">The looper object to start</param>
		/// <returns>The resulting coroutine</returns>
		public static Coroutine StartCoroutine(Looper looper){
			var saved = Host.StartCoroutine(looper.TickRoutine());
			Host.RunningLoops.Add(looper);
			return saved;
		}
		
		/// <summary>
		/// Stops a coroutine running on the host monobehaviour
		/// </summary>
		/// <param name="looper">The looper object to stop</param>
		/// <param name="coroutine">The coroutine reference to stop</param>
		public static void StopCoroutine(Looper looper, Coroutine coroutine){
			Host.RunningLoops.Remove(looper);
			Host.StopCoroutine(coroutine);
		}
#endregion
		
		[BoxGroup("Status and Controls"), Button, HideIf("Running"), HideInEditorMode]
		public void Run(){
			if (!Application.isPlaying)
				throw new InEditModeException();
			if (routine == null)
				routine = StartCoroutine(this);
		}

		[BoxGroup("Status and Controls"), Button, ShowIf("Running"), HideInEditorMode]
		public void Stop(){
			if (!Application.isPlaying)
				throw new InEditModeException();
			if (routine == null) return;
			StopCoroutine(this, routine);
			routine = null;
		}

		[BoxGroup("Status and Controls"), ShowInInspector]
		public bool Running => routine != null;
		
		/// <summary>
		/// Name for easy identification on LooperHost list
		/// </summary>
		[BoxGroup("Status and Controls")]
		public string Name;
		
		/// <summary>
		/// The type of delay that will be used between loop iterations
		/// </summary>
		[BoxGroup("Delay Settings")]
		public DelayType DelayType;
		
		private bool IsDelayTime => DelayType == DelayType.AfterTimeDelay;
		[BoxGroup("Delay Settings"), MinValue(0), ShowIf("IsDelayTime"), InfoBox("The time in seconds between loop iterations")]
		public float LoopDelayTime = 0;
		
		private bool IsDelayFrameInterval => DelayType == DelayType.AfterFrameInterval;
		[BoxGroup("Delay Settings"), MinValue(1), ShowIf("IsDelayFrameInterval"), InfoBox("The amount of frames that will pass between loop iterations")]
		public int LoopFrameInterval = 0;

		protected Looper(DelayType delayType) {
			DelayType = delayType;
		}

		protected Looper (DelayType delayType, string name) : this(delayType) {
			Name = name;
		}

		private Coroutine routine = null;
		
		public IEnumerator TickRoutine() {
			long iterations = 0;
			while (true){
				// Run loop action
				InvokeLoopAction(iterations);
				//LoopAction?.Invoke(iterations);
				iterations++;
				
				// Wait for the type of delay to complete
				switch (DelayType) {
					case DelayType.EveryUpdate:
						yield return null;
						break;
					case DelayType.EveryFixedUpdate:
						yield return new WaitForFixedUpdate();
						break;
					case DelayType.AfterFrameInterval:
						if (LoopFrameInterval <= 1)
							yield return null;
						else
							yield return new WaitUntil(() => Time.frameCount % LoopFrameInterval == 0);
						break;
					case DelayType.AfterTimeDelay:
						if (LoopDelayTime <= 0)
							yield return null;
						else
							yield return new WaitForSeconds(LoopDelayTime);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		protected abstract void InvokeLoopAction(long iterations);

		public class InEditModeException : Exception {
			public override string Message => "Looper cannot be used in Edit Mode";
		}
	}
}
