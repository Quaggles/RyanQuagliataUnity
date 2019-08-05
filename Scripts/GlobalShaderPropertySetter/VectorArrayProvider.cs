using System.Collections.Generic;
using UnityEngine;

namespace RyanQuagliataUnity.GlobalShaderPropertySetter {
	public class VectorArrayProvider : MonoBehaviour {
		public List<VectorArraySource> VectorSources = new List<VectorArraySource>();
		
		private Vector4[] cachedArray;
		public Vector4[] GetArray() {
			// If the sizes don't match make a new array
			if (cachedArray == null || cachedArray.Length != VectorSources.Count) cachedArray = new Vector4[VectorSources.Count];
			
			// Set values
			for (var i = 0; i < VectorSources.Count; i++) cachedArray[i] = VectorSources[i].Get();

			return cachedArray;
		}

		private readonly List<Vector4> cachedList = new List<Vector4>();
		public List<Vector4> GetList() {
			int diff = cachedList.Count - VectorSources.Count;
			
			// Reduce excess size
			if (diff > 0) cachedList.RemoveRange(cachedList.Count - diff, diff);

			// Update existing
			for (var i = 0; i < VectorSources.Count; i++) {
				var vectorSource = VectorSources[i];
				// Expand list if needed
				if (i >= VectorSources.Count) {
					cachedList.Add(vectorSource.Get());
				} else {
					cachedList[i] = vectorSource.Get();
				}
			}

			return cachedList;
		}
	}
}
