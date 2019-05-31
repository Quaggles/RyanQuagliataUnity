using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RyanQuagliata.NewObjectPool {
	[Serializable]
	public class ComponentObjectPool<T> : UnityObjectPool<T> where T : Component {
		public Transform Parent;
		
		/// <summary>
		/// When destroying a component the entire gameobject needs to be destroyed
		/// </summary>
		/// <param name="item"></param>
		protected override void Destroy(T item) {
			if (Application.isPlaying)
				Object.Destroy(item.gameObject);
			else
				Object.DestroyImmediate(item.gameObject);
		}

		protected override T Instantiate() {
			var item = base.Instantiate();
			if (Parent != null) item.transform.SetParent(Parent, false);
			return item;
		}

		public override void Return(T item, bool destroy = false) {
			base.Return(item, destroy);
			if (item != null && item.transform.parent != Parent) {
				item.transform.SetParent(Parent, false);	
			}
		}
	}
}