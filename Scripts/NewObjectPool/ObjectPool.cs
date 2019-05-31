using System;

namespace RyanQuagliata.NewObjectPool {
	public class ObjectPool<T> : GenericObjectPool<T> where T : new() {
		protected override T Instantiate() => new T();

		protected override void Destroy(T item) {
			if (item is IDisposable disposable) disposable.Dispose();
		}
	}
}