using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity.NewObjectPool {
	[Serializable]
	public class UnityObjectPool<T> : GenericObjectPool<T> where T : Object {
		[Required]
		[PropertyOrder(-1)]
		[AssetsOnly]
		public T Template;
		
		protected override T Instantiate() {
			if (!TemplateValid) throw new NullReferenceException($"Template property on {GetType()} is null, cannot populate pool");
			return Object.Instantiate(Template);
		}

		protected override void Destroy(T item) {
			if (Application.isPlaying)
				Object.Destroy(item);
			else
				Object.DestroyImmediate(item);
		}
		
		[Button]
		[EnableIf(nameof(TemplateValid))]
		public override T Retrieve() {
			return base.Retrieve();
		}

		[Button]
		[EnableIf(nameof(TemplateValid))]
		public override void ReturnAll() {
			base.ReturnAll();
		}
		
		[Button]
		[EnableIf(nameof(TemplateValid))]
		public override void DestroyAll() {
			base.DestroyAll();
		}

		public bool TemplateValid => Template != null;
	}
}