using System;

namespace RyanQuagliataUnity.Extensions {
	public static class Array2DExtensions {
		public static T[] GetRow<T>(this T[,] array, int row) {
			if (array == null) throw new ArgumentNullException(nameof(array));
			int rowCount = array.GetLength(1);
			if (row < 0 || row > rowCount) throw new IndexOutOfRangeException($"Row {row} is outside of the range of input array (0, {rowCount})");
		
			int columnCount = array.GetLength(0);
		
			T[] newArray = new T[columnCount];
			for (int col = 0; col < columnCount; col++) {
				newArray[col] = array[col, row];
			}
			return newArray;
		}
		public static T[] GetColumn<T>(this T[,] array, int col) {
			if (array == null) throw new ArgumentNullException(nameof(array));
			int columnCount = array.GetLength(0);
			if (col < 0 || col > columnCount) throw new IndexOutOfRangeException($"Column {col} is outside of the range of input array (0, {columnCount})");
		
			int rowCount = array.GetLength(1);
			T[] newArray = new T[rowCount];
		
			for (int row = 0; row < rowCount; row++) {
				newArray[row] = array[col, row];
			}
			return newArray;
		}

		public static void Resize2D<T>(ref T[,] original, int cols, int rows) {
			var newArray = new T[cols, rows];
			if (original == null) {
				original = newArray;
				return;
			}
			int minRows = Math.Min(cols, original.GetLength(0));
			int minCols = Math.Min(rows, original.GetLength(1));
			for (int i = 0; i < minRows; i++)
			for (int j = 0; j < minCols; j++)
				newArray[i, j] = original[i, j];
			original = newArray;
		}
	}
}