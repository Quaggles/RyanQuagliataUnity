﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;

namespace RyanQuagliata.Extensions {
	public static class MeshExtensions {
		public static async Task RecalculateBoundsAsync(this Mesh that) => that.bounds = await RecalculateBoundsAsync(that.vertices);

		public static async Task<Bounds> RecalculateBoundsAsync(IReadOnlyList<Vector3> vertices) {
			// Get on a background thread as this operation is slow
			await UniTask.SwitchToThreadPool();

			var bounds = new Bounds();
			for (int i = 0; i < vertices.Count; i++) {
				bounds.Encapsulate(vertices[i]);
			}
			
			// Get back on the main thread so we can apply the bounds
			await UniTask.SwitchToMainThread();
			return bounds;
		}
	}
}