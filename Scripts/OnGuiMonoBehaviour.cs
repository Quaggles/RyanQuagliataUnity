#if UNITY_EDITOR
using System;
using System.Collections.Generic;
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
		public Queue<Action> OnGuiActions = new Queue<Action>();

		private void OnDrawGizmos() {
			while (OnGuiActions.Count > 0) {
				var i = OnGuiActions.Dequeue();
				i.Invoke();
			}
		}

		private void OnEnable() => UnityEditor.Compilation.CompilationPipeline.compilationStarted += CompilationPipelineOnCompilationStarted;

		private void CompilationPipelineOnCompilationStarted(object obj) => gameObject.DestroySmart();

		private void OnDisable() => UnityEditor.Compilation.CompilationPipeline.compilationStarted -= CompilationPipelineOnCompilationStarted;
	}
}
#endif