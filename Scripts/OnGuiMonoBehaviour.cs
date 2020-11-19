#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using RyanQuagliataUnity.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliataUnity {
	[ExecuteInEditMode]
	// ReSharper disable once InconsistentNaming
	public class OnGuiMonoBehaviour : MonoBehaviour {
		[ShowInInspector]
		[ListDrawerSettings(Expanded = true)]
		[InfoBox("MonoBehaviour used to execute OnGui calls that have been queued from other MonoBehaviour events")]
		// ReSharper disable once InconsistentNaming
		public List<Action> GizmoActions = new List<Action>();

		[ShowInInspector]
		[ListDrawerSettings(Expanded = true)]
		[InfoBox("MonoBehaviour used to execute OnGui calls that have been queued from other MonoBehaviour events")]
		// ReSharper disable once InconsistentNaming
		public List<Action> GuiActions = new List<Action>();

		private CancellationTokenSource cts;
		private IEnumerator coroutine;
		private void OnDrawGizmos() {
			foreach (var action in GizmoActions) action.Invoke();
		}

		private void OnGUI() {
			foreach (var action in GuiActions) action.Invoke();
		}

		/// <summary>
		/// Clear on frame end as OnDrawGizmos can be called multiple times per frame
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public IEnumerator ClearOnFrameEnd(CancellationToken cancellationToken) {
			var wait = new WaitForEndOfFrame();
			while (!cancellationToken.IsCancellationRequested) {
				yield return wait;
				GizmoActions.Clear();
				GuiActions.Clear();
			}
		}


		private void OnEnable() {
			EditorApplication.update += MoveNext;
			UnityEditor.Compilation.CompilationPipeline.compilationStarted += CompilationPipelineOnCompilationStarted;
			cts?.Cancel();
			cts = new CancellationTokenSource();
			coroutine = ClearOnFrameEnd(cts.Token);
			//StartCoroutine(ClearOnFrameEnd(cts.Token));
		}

		private void OnDisable() {
			EditorApplication.update -= MoveNext;
			UnityEditor.Compilation.CompilationPipeline.compilationStarted -= CompilationPipelineOnCompilationStarted;
			cts?.Cancel();
		}

		/// <summary>
		/// Calls the coroutine in the editor update cycle
		/// </summary>
		void MoveNext() => coroutine.MoveNext();

		private void CompilationPipelineOnCompilationStarted(object obj) => gameObject.DestroySmart();
	}
}
#endif