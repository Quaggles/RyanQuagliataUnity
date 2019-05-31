using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if ODIN_INSPECTOR
#endif


namespace RyanQuagliata {
	public class DelayedActionHost : MonoBehaviour {
#if ODIN_INSPECTOR
		[InfoBox("The DelayedActionHost runs all delayed action coroutines")]
#endif
		private static DelayedActionHost instance;

		/// <summary>
		/// Instance of the singleton host object
		/// </summary>
		public static DelayedActionHost Instance {
			get {
				if (instance != null) return instance;
				instance = new GameObject().AddComponent<DelayedActionHost>();
				instance.name = "[DelayedActionHost]";
				DontDestroyOnLoad(instance);
				return instance;
			}
		}
	}

	public static class DelayedAction {
		/// <summary>
		/// Dictionary for cached WaitForSeconds objects, reduceds garbage on repeated Wait's
		/// </summary>
		private static readonly Dictionary<float, WaitForSeconds> WaitForSecondsCache =
			new Dictionary<float, WaitForSeconds>();

		/// <summary>
		/// Does an action after a specified number of frames
		/// </summary>
		/// <param name="action">The action to perform</param>
		/// <param name="frameDelay">The amount of frames to wait</param>
		/// <returns>Coroutine object</returns>
		public static Coroutine AfterFrameCount(Action action, int frameDelay) {
			return DelayedActionHost.Instance.StartCoroutine(AfterFrameCountRoutine(action, frameDelay));
		}

		/// <summary>
		/// Does an action after a specified number of frames
		/// </summary>
		/// <param name="action">The action to perform</param>
		/// <param name="frameDelay">The amount of frames to wait</param>
		/// <returns>Yield Instruction</returns>
		/// <exception cref="ArgumentException">Called if frameDelay is less than 0</exception>
		private static IEnumerator AfterFrameCountRoutine(Action action, int frameDelay) {
			if (frameDelay < 0) throw new ArgumentException("frameDelay must be greater than 0");
			int count = 0;
			while (count < frameDelay) {
				yield return null;
				count++;
			}
			action.Invoke();
		}

		/// <summary>
		/// Does an action when the next frame occurs
		/// </summary>
		/// <param name="action">The action to perform</param>
		/// <returns>Coroutine object</returns>
		public static Coroutine AfterFrame(Action action) {
			return DelayedActionHost.Instance.StartCoroutine(AfterFrameRoutine(action));
		}

		/// <summary>
		/// Does an action when the next frame occurs
		/// </summary>
		/// <param name="action">The action to perform</param>
		/// <returns>Yield Instruction</returns>
		private static IEnumerator AfterFrameRoutine(Action action) {
			yield return null;
			action.Invoke();
		}

		/// <summary>
		/// Does an action after a specified amount of seconds
		/// </summary>
		/// <param name="action">The action to perform</param>
		/// <param name="delay">The delay to wait</param>
		/// <param name="doCache">Should we cache the WaitForSeconds Yield Instruction, only do this if you intend to reuse this exact wait delay</param>
		/// <returns>Coroutine object</returns>
		public static Coroutine AfterSeconds(Action action, float delay, bool doCache = true) {
			return DelayedActionHost.Instance.StartCoroutine(AfterSecondsRoutine(action, delay, doCache));
		}

		/// <summary>
		/// Does an action after a specified amount of seconds
		/// </summary>
		/// <param name="action">The action to perform</param>
		/// <param name="delay">The delay to wait</param>
		/// <param name="doCache">Should we cache the WaitForSeconds Yield Instruction, only do this if you intend to reuse this exact wait delay</param>
		/// <returns>Yield Instruction</returns>
		private static IEnumerator AfterSecondsRoutine(Action action, float delay, bool doCache = true) {
			WaitForSeconds waitAction = null;
			if (!WaitForSecondsCache.TryGetValue(delay, out waitAction)) {
				waitAction = new WaitForSeconds(delay);
				if (doCache)
					WaitForSecondsCache.Add(delay, waitAction);
			}
			yield return waitAction;
			action.Invoke();
		}
	}
}