using System;
using UnityEngine;

namespace RyanQuagliata.Extensions {
	public static class BoundsExtensions {
		/// <summary>
		/// Gets the 8 vertices of the bounding box in local space
		/// </summary>
		/// <param name="that"></param>
		/// <param name="corners">Optional array that will be filled instead of creating a new array</param>
		/// <exception cref="ArgumentOutOfRangeException">Provided corners array length is less than 8</exception>
		/// <returns></returns>
		public static Vector3[] GetCorners(this Bounds that, Vector3[] corners = null) {
			corners = corners ?? new Vector3[8];
			if (corners.Length < 8) throw new ArgumentOutOfRangeException(nameof(corners), "Length of corners array must be at least 8");
			corners[0] = that.min;
			corners[1] = that.max;
			corners[2] = new Vector3(corners[0].x, corners[0].y, corners[1].z);
			corners[3] = new Vector3(corners[0].x, corners[1].y, corners[0].z);
			corners[4] = new Vector3(corners[1].x, corners[0].y, corners[0].z);
			corners[5] = new Vector3(corners[0].x, corners[1].y, corners[1].z);
			corners[6] = new Vector3(corners[1].x, corners[0].y, corners[1].z);
			corners[7] = new Vector3(corners[1].x, corners[1].y, corners[0].z);
			return corners;
		}
		
		/// <summary>
		/// Gets the center points of the 6 faces that make up the bounding box in local space
		/// </summary>
		/// <param name="that"></param>
		/// <param name="extents">Optional array that will be filled instead of creating a new array</param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException">Provided extents array length is less than 6</exception>
		public static Vector3[] GetExtents(this Bounds that, Vector3[] extents = null){
			extents = extents ?? new Vector3[6];
			if (extents.Length < 6) throw new ArgumentOutOfRangeException(nameof(extents), "Length of extents array must be at least 6");
			extents[0] = new Vector3(that.extents.x, 0, 0);
			extents[1] = new Vector3(-that.extents.x, 0, 0);
			extents[2] = new Vector3(0, that.extents.y, 0);
			extents[3] = new Vector3(0, -that.extents.y, 0);
			extents[4] = new Vector3(0, 0, that.extents.z);
			extents[5] = new Vector3(0, 0, -that.extents.z);
			return extents;
		}
	}
}
