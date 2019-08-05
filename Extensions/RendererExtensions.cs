using System;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class RendererExtensions {
		/// <summary>
		/// If the application is playing it gets the instantiated material, otherwise gets the shared one
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static Material GetProperMaterial(this Renderer that) => Application.isPlaying ? that?.material : that?.sharedMaterial;

		/// <summary>
		/// Gets the 8 vertices of the bounding box in world space
		/// </summary>
		/// <param name="that"></param>
		/// <param name="corners">Optional array that will be filled instead of creating a new array</param>
		/// <exception cref="ArgumentOutOfRangeException">Provided corners array length is less than 8</exception>
		/// <returns></returns>
		public static Vector3[] GetCorners(this Renderer that, Vector3[] corners = null) {
			var positions = that.bounds.GetCorners(corners);
			for (int i = 0; i < 8; i++)
				positions[i] = that.bounds.center + positions[i];
			return positions;
		}
		
		/// <summary>
		/// Gets the center points of the 6 faces that make up the bounding box in world space
		/// </summary>
		/// <param name="that"></param>
		/// <param name="extents">Optional array that will be filled instead of creating a new array</param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException">Provided extents array length is less than 6</exception>
		public static Vector3[] GetExtents(this Renderer that, Vector3[] extents = null) {
			var positions = that.bounds.GetExtents(extents);
			for (int i = 0; i < 6; i++)
				positions[i] = that.bounds.center + positions[i];

			return positions;
		}
	}
}
