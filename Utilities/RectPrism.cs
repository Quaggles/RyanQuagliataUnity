using RyanQuagliata.Extensions;
using UnityEngine;

namespace RyanQuagliata.Utilities {
	[System.Serializable]
	public struct RectPrism {
		public bool Equals(RectPrism other) {
			return m_XMin.Equals(other.m_XMin) && m_YMin.Equals(other.m_YMin) && m_ZMin.Equals(other.m_ZMin) &&
			       m_Width.Equals(other.m_Width) && m_Height.Equals(other.m_Height) && m_Depth.Equals(other.m_Depth);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			return obj is RectPrism && Equals((RectPrism) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = m_XMin.GetHashCode();
				hashCode = (hashCode * 397) ^ m_YMin.GetHashCode();
				hashCode = (hashCode * 397) ^ m_ZMin.GetHashCode();
				hashCode = (hashCode * 397) ^ m_Width.GetHashCode();
				hashCode = (hashCode * 397) ^ m_Height.GetHashCode();
				hashCode = (hashCode * 397) ^ m_Depth.GetHashCode();
				return hashCode;
			}
		}

		public static implicit operator Rect(RectPrism rectPrism) => new Rect(rectPrism.xMin, rectPrism.yMin, rectPrism.Width, rectPrism.Height);

		public static implicit operator RectPrism(Rect rect) =>
			new RectPrism(rect.xMin, rect.yMin, 0, rect.width, rect.height, 0);

		private float m_XMin;
		private float m_YMin;
		private float m_ZMin;
		private float m_Width;
		private float m_Height;
		private float m_Depth;

		/// <summary>
		///   <para>Creates a new rectangle.</para>
		/// </summary>
		/// <param name="x">The X value the rect is measured from.</param>
		/// <param name="y">The Y value the rect is measured from.</param>
		/// <param name="z">The Z value the rect is measured from.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		/// <param name="depth">The depth of the rectangle.</param>
		public RectPrism(float x, float y, float z, float width, float height, float depth) {
			m_XMin = x;
			m_YMin = y;
			m_ZMin = z;
			m_Width = width;
			m_Height = height;
			m_Depth = depth;
		}

		/// <summary>
		///   <para>Creates a rectangle given a size and position.</para>
		/// </summary>
		/// <param name="position">The position of the minimum corner of the rect.</param>
		/// <param name="size">The width and height of the rect.</param>
		public RectPrism(Vector3 position, Vector3 size) {
			m_XMin = position.x;
			m_YMin = position.y;
			m_ZMin = position.z;
			m_Width = size.x;
			m_Height = size.y;
			m_Depth = size.z;
		}

		/// <summary>
		///   <para></para>
		/// </summary>
		/// <param name="source"></param>
		public RectPrism(RectPrism source) {
			m_XMin = source.m_XMin;
			m_YMin = source.m_YMin;
			m_ZMin = source.m_ZMin;
			m_Width = source.m_Width;
			m_Height = source.m_Height;
			m_Depth = source.m_Depth;
		}

		/// <summary>
		///   <para>Shorthand for writing new Rect(0,0,0,0).</para>
		/// </summary>
		public static RectPrism Zero => new RectPrism(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

		/// <summary>
		///   <para>Creates a rectangle from min/max coordinate values.</para>
		/// </summary>
		/// <param name="xmin">The minimum X coordinate.</param>
		/// <param name="ymin">The minimum Y coordinate.</param>
		/// <param name="zmin">The minimum Z coordinate</param>
		/// <param name="xmax">The maximum X coordinate.</param>
		/// <param name="ymax">The maximum Z coordinate.</param>
		/// <param name="zmax"></param>
		/// <returns>
		///   <para>A rectangle matching the specified coordinates.</para>
		/// </returns>
		public static RectPrism MinMaxRect(float xmin, float ymin, float zmin, float xmax, float ymax, float zmax) {
			return new RectPrism(xmin, ymin, zmin, xmax - xmin, ymax - ymin, zmax - zmin);
		}

		/// <summary>
		/// <para>Creates a rectangle from min/max coordinate values.</para>
		/// </summary>
		/// <param name="minPoint">The minimum coordinate</param>
		/// <param name="maxPoint">The maximum coordinate</param>
		/// <returns></returns>
		public static RectPrism MinMaxRect(Vector3 minPoint, Vector3 maxPoint) {
			return MinMaxRect(minPoint.x, minPoint.y, minPoint.z, maxPoint.x, maxPoint.y, maxPoint.z);
		}

		/// <summary>
		///   <para>Set components of an existing Rect.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="depth"></param>
		public void Set(float x, float y, float z, float width, float height, float depth) {
			m_XMin = x;
			m_YMin = y;
			m_ZMin = z;
			m_Width = width;
			m_Height = height;
			m_Depth = depth;
		}

		/// <summary>
		///   <para>Set components of an existing Rect.</para>
		/// </summary>
		/// <param name="source">The rect to copy values from</param>
		public void Set(RectPrism source) {
			m_XMin = source.x;
			m_YMin = source.y;
			m_ZMin = source.z;
			m_Width = source.Width;
			m_Height = source.Height;
			m_Depth = source.Depth;
		}

		/// <summary>
		///   <para>The X coordinate of the rectangle.</para>
		/// </summary>
		public float x {
			get { return m_XMin; }
			set { m_XMin = value; }
		}

		/// <summary>
		///   <para>The Y coordinate of the rectangle.</para>
		/// </summary>
		public float y {
			get { return m_YMin; }
			set { m_YMin = value; }
		}

		/// <summary>
		///   <para>The Z coordinate of the rectangle.</para>
		/// </summary>
		public float z {
			get { return m_ZMin; }
			set { m_ZMin = value; }
		}

		/// <summary>
		///   <para>The X, Y and Z position of the rectangle.</para>
		/// </summary>
		#if ODIN_INSPECTOR
		[Sirenix.OdinInspector.ShowInInspector]
		#endif
		public Vector3 Position {
			get { return new Vector3(m_XMin, m_YMin, m_ZMin); }
			set {
				m_XMin = value.x;
				m_YMin = value.y;
				m_ZMin = value.z;
			}
		}

		/// <summary>
		///   <para>The position of the center of the rectangle.</para>
		/// </summary>
		public Vector3 Center {
			get { return new Vector3(x + m_Width / 2f, y + m_Height / 2f, z + m_Depth / 2f); }
			set {
				m_XMin = value.x - m_Width / 2f;
				m_YMin = value.y - m_Height / 2f;
				m_ZMin = value.z - m_Depth / 2f;
			}
		}

		/// <summary>
		///   <para>The position of the minimum corner of the rectangle.</para>
		/// </summary>
		public Vector3 Min {
			get { return new Vector3(xMin, yMin, zMax); }
			set {
				xMin = value.x;
				yMin = value.y;
				zMin = value.z;
			}
		}

		/// <summary>
		///   <para>The position of the maximum corner of the rectangle.</para>
		/// </summary>
		public Vector3 Max {
			get { return new Vector3(xMax, yMax, zMax); }
			set {
				xMax = value.x;
				yMax = value.y;
				zMax = value.z;
			}
		}

		/// <summary>
		///   <para>The width of the rectangle, measured from the X position.</para>
		/// </summary>
		public float Width {
			get { return m_Width; }
			set { m_Width = value; }
		}

		/// <summary>
		///   <para>The height of the rectangle, measured from the Y position.</para>
		/// </summary>
		public float Height {
			get { return m_Height; }
			set { m_Height = value; }
		}

		/// <summary>
		///   <para>The height of the rectangle, measured from the Y position.</para>
		/// </summary>
		public float Depth {
			get { return m_Depth; }
			set { m_Depth = value; }
		}

		/// <summary>
		///   <para>The width and height of the rectangle.</para>
		/// </summary>
#if ODIN_INSPECTOR
		[Sirenix.OdinInspector.ShowInInspector]
#endif
		public Vector3 Size {
			get { return new Vector3(m_Width, m_Height, m_Depth); }
			set {
				m_Width = value.x;
				m_Height = value.y;
				m_Depth = value.z;
			}
		}

		/// <summary>
		///   <para>The minimum X coordinate of the rectangle.</para>
		/// </summary>
		public float xMin {
			get { return m_XMin; }
			set {
				m_XMin = value;
				m_Width = xMax - m_XMin;
			}
		}

		/// <summary>
		///   <para>The minimum Y coordinate of the rectangle.</para>
		/// </summary>
		public float yMin {
			get { return m_YMin; }
			set {
				m_YMin = value;
				m_Height = yMax - m_YMin;
			}
		}

		/// <summary>
		///   <para>The minimum Z coordinate of the rectangle.</para>
		/// </summary>
		public float zMin {
			get { return m_ZMin; }
			set {
				m_ZMin = value;
				m_Depth = zMax - m_ZMin;
			}
		}

		/// <summary>
		///   <para>The maximum X coordinate of the rectangle.</para>
		/// </summary>
		public float xMax {
			get { return m_Width + m_XMin; }
			set { m_Width = value - m_XMin; }
		}

		/// <summary>
		///   <para>The maximum Y coordinate of the rectangle.</para>
		/// </summary>
		public float yMax {
			get { return m_Height + m_YMin; }
			set { m_Height = value - m_YMin; }
		}

		/// <summary>
		///   <para>The maximum Z coordinate of the rectangle.</para>
		/// </summary>
		public float zMax {
			get { return m_Depth + m_ZMin; }
			set { m_Depth = value - m_ZMin; }
		}

		/// <summary>
		///   <para>Returns true if the x and y components of point is a point inside this rectangle. If allowInverse is present and true, the width and height of the Rect are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="point">Point to test.</param>
		/// <returns>
		///   <para>True if the point lies within the specified rectangle.</para>
		/// </returns>
		public bool Contains(Vector2 point) {
			return (double) point.x >= (double) xMin && (double) point.x < (double) xMax && (double) point.y >= (double) yMin &&
			       (double) point.y < (double) yMax;
		}

		/// <summary>
		///   <para>Returns true if the x and y components of point is a point inside this rectangle. If allowInverse is present and true, the width and height of the Rect are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="point">Point to test.</param>
		/// <returns>
		///   <para>True if the point lies within the specified rectangle.</para>
		/// </returns>
		public bool Contains(Vector3 point) {
			bool a = (double) point.x >= (double) xMin && (double) point.x < (double) xMax;
			bool b = (double) point.y >= (double) yMin && (double) point.y < (double) yMax;
			bool c = (double) point.z >= (double) zMin && (double) point.z < (double) zMax;
			return a && b && c;
		}

		/// <summary>
		///   <para>Returns true if the x and y components of point is a point inside this rectangle. If allowInverse is present and true, the width and height of the Rect are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="point">Point to test.</param>
		/// <param name="allowInverse">Does the test allow the Rect's width and height to be negative?</param>
		/// <returns>
		///   <para>True if the point lies within the specified rectangle.</para>
		/// </returns>
		public bool Contains(Vector3 point, bool allowInverse) {
			if (!allowInverse)
				return Contains(point);
			bool a = (double) Width < 0.0 && (double) point.x <= (double) xMin && (double) point.x > (double) xMax ||
			         (double) Width >= 0.0 && (double) point.x >= (double) xMin && (double) point.x < (double) xMax;
			bool b = ((double) Height < 0.0 && (double) point.y <= (double) yMin && (double) point.y > (double) yMax ||
			          (double) Height >= 0.0 && (double) point.y >= (double) yMin && (double) point.y < (double) yMax);
			bool c = ((double) Depth < 0.0 && (double) point.z <= (double) zMin && (double) point.z > (double) zMax ||
			          (double) Depth >= 0.0 && (double) point.z >= (double) zMin && (double) point.z < (double) zMax);
			return a && b && c;
		}

		private static RectPrism OrderMinMax(RectPrism rect) {
			if ((double) rect.xMin > (double) rect.xMax) {
				float xMin = rect.xMin;
				rect.xMin = rect.xMax;
				rect.xMax = xMin;
			}

			if ((double) rect.yMin > (double) rect.yMax) {
				float yMin = rect.yMin;
				rect.yMin = rect.yMax;
				rect.yMax = yMin;
			}

			if ((double) rect.zMin > (double) rect.zMax) {
				float zMin = rect.zMin;
				rect.zMin = rect.zMax;
				rect.zMax = zMin;
			}

			return rect;
		}

		/// <summary>
		///   <para>Returns true if the other rectangle overlaps this one. If allowInverse is present and true, the widths and heights of the Rects are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="other">Other rectangle to test overlapping with.</param>
		public bool Overlaps(RectPrism other) {
			return (double) other.xMax > (double) xMin && (double) other.xMin < (double) xMax &&
			       (double) other.yMax > (double) yMin && (double) other.yMin < (double) yMax &&
			       (double) other.zMax > (double) zMin && (double) other.zMin < (double) zMax;
		}

		/// <summary>
		///   <para>Returns true if the other rectangle overlaps this one. If allowInverse is present and true, the widths and heights of the Rects are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="other">Other rectangle to test overlapping with.</param>
		/// <param name="allowInverse">Does the test allow the widths and heights of the Rects to be negative?</param>
		public bool Overlaps(RectPrism other, bool allowInverse) {
			RectPrism rect = this;
			if (allowInverse) {
				rect = RectPrism.OrderMinMax(rect);
				other = RectPrism.OrderMinMax(other);
			}

			return rect.Overlaps(other);
		}

		/// <summary>
		///   <para>Returns a point inside a rectangle, given normalized coordinates.</para>
		/// </summary>
		/// <param name="rectangle">Rectangle to get a point inside.</param>
		/// <param name="normalizedRectCoordinates">Normalized coordinates to get a point for.</param>
		public static Vector3 NormalizedToPoint(RectPrism rectangle, Vector3 normalizedRectCoordinates) {
			return new Vector3(
				UnityEngine.Mathf.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x),
				UnityEngine.Mathf.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y),
				UnityEngine.Mathf.Lerp(rectangle.z, rectangle.zMax, normalizedRectCoordinates.z)
			);
		}

		/// <summary>
		///   <para>Returns the normalized coordinates cooresponding the the point.</para>
		/// </summary>
		/// <param name="rectangle">Rectangle to get normalized coordinates inside.</param>
		/// <param name="point">A point inside the rectangle to get normalized coordinates for.</param>
		/// <param name="clamped">Whether the resulting vector will be clamped between {0,0,0} and {1,1,1}</param>
		public static Vector3 PointToNormalized(RectPrism rectangle, Vector3 point, bool clamped = true) {
			if (!clamped) {
				return new Vector3(
					MathfExtensions.InverseLerpUnclamped(rectangle.x, rectangle.xMax, point.x),
					MathfExtensions.InverseLerpUnclamped(rectangle.y, rectangle.yMax, point.y),
					MathfExtensions.InverseLerpUnclamped(rectangle.z, rectangle.zMax, point.z)
				);
			} else {
				return new Vector3(
					UnityEngine.Mathf.InverseLerp(rectangle.x, rectangle.xMax, point.x),
					UnityEngine.Mathf.InverseLerp(rectangle.y, rectangle.yMax, point.y),
					UnityEngine.Mathf.InverseLerp(rectangle.z, rectangle.zMax, point.z)
				);
			}
		}

		public static bool operator !=(RectPrism lhs, RectPrism rhs) {
			return !(lhs == rhs);
		}

		public static bool operator ==(RectPrism lhs, RectPrism rhs) {
			return (double) lhs.x == (double) rhs.x && (double) lhs.y == (double) rhs.y && (double) lhs.z == (double) rhs.z &&
			       (double) lhs.Width == (double) rhs.Width && (double) lhs.Height == (double) rhs.Height &&
			       (double) lhs.Depth == (double) rhs.Depth;
		}

		/// <summary>
		///   <para>Returns a nicely formatted string for this Rect.</para>
		/// </summary>
		public override string ToString() =>
			$"(x:{x:F2}, y:{y:F2}, z:{z:F2}, width:{Width:F2}, height:{Height:F2}, depth:{Depth:F2})";

		/// <summary>
		///   <para>Returns a nicely formatted string for this Rect.</para>
		/// </summary>
		/// <param name="format"></param>
		public string ToString(string format) =>
			$"(x:{x.ToString(format)}, y:{y.ToString(format)}, z:{z.ToString(format)}, width:{Width.ToString(format)}, height:{Height.ToString(format)}, depth:{Depth.ToString(format)})";
		
		/// <summary>
		/// Returns a vector of the position normalized within this Rect space
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public Vector3 NormalizePosition(Vector3 position) => position.Remap(Min, Max, new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
	}
}