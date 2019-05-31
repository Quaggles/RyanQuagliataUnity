using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RyanQuagliata.NewObjectPool {
	[Serializable, HideReferenceObjectPicker]
	public abstract class GenericObjectPool<T> : IReadOnlyCollection<T> {
		public bool Expandable = true;

		[MinValue(0)]
		public int PreloadAmount = 0;
		
		[MinValue(0)]
		public int PreferredPoolAmount = 0;

		[MinValue(0)]
		public int PoolLimit = 0;

		public bool SetAvailableOnReturn = true;
		
		public readonly HashSet<T> Pool = new HashSet<T>(); // List of every object in the pool whether in use or not
		[ShowInInspector]
		public readonly Stack<T> Available = new Stack<T>(); // Stack of available objects for quick retrieval
		[ShowInInspector]
		public readonly HashSet<T> InUse = new HashSet<T>(); // List of objects in use for iteration over

		public event EventHandler<T> OnExpand; 
		public event EventHandler<T> OnRetrieve;
		public event EventHandler<T> OnReturn; 
		
		/// <summary>
		/// Preloads the pool with the required amount of objects
		/// </summary>
		public void Preload() {
			var poolCount = Pool.Count; // Store this count before we add new items
			for (int i = 0; i < PreloadAmount - poolCount; i++) ExpandPool();
		}

		protected abstract T Instantiate();
		protected abstract void Destroy(T item);

		protected virtual T ExpandPool() {
			var newItem = Instantiate();
			OnExpand?.Invoke(this, newItem);
			// If the new object wants to be able to return itself, give it a callback
			if (newItem is IPoolReturnable returnable) returnable.PoolReturn = () => Return(newItem);
			
			// Notify the object it has been returned to the pool
			try {
				if (newItem is IPoolNotifiable notifiable) notifiable.PoolInUse(false);
			} catch (Exception ex){
				if (newItem is Object unityItem) {
					Debug.LogError(ex, unityItem);
				} else {
					Debug.LogError(ex);
				}
			}
			
			Available.Push(newItem);
			Pool.Add(newItem);
			return newItem;
		}
		/// <summary>
		/// Retrieves an object from the pool for usage
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NoFreeItemsException"></exception>
		[Button]
		public virtual T Retrieve() {
			// If there are no available items and the pool can expand, expand it
			if (Expandable && Available.Count == 0) ExpandPool();

			if (Available.Count <= 0) throw new NoFreeItemsException();

			var item = Available.Pop();
			InUse.Add(item); // Record this as an item inuse
			
			// Notify the object it has become active
			try {
				if (item is IPoolNotifiable notifiable) notifiable.PoolInUse(true);
			} catch (Exception ex){
				if (item is Object unityItem) {
					Debug.LogError(ex, unityItem);
				} else {
					Debug.LogError(ex);
				}
			}

			OnRetrieve?.Invoke(this, item);
			return item;
		}
		
		/// <summary>
		/// Returns an item to the pool to be reused
		/// </summary>
		/// <param name="item">The item to be returned</param>
		/// <param name="destroy">Should this item be destroyed</param>
		public virtual void Return(T item, bool destroy = false) {
			// Notify the object it has been returned to the pool
			try {
				if (item is IPoolNotifiable notifiable) notifiable.PoolInUse(false);
			} catch (Exception ex){
				if (item is Object unityItem) {
					Debug.LogError(ex, unityItem);
				} else {
					Debug.LogError(ex);
				}
			}
			
			OnReturn?.Invoke(this, item);
			InUse.Remove(item);
			if (SetAvailableOnReturn) SetAvailable(item, destroy);
		}

		public virtual void SetAvailable(T item, bool destroy = false) {
			// When returning an object and the pool is larger than it wants to be destroy it
			if (destroy || PreferredPoolAmount > 0 && Pool.Count > PreferredPoolAmount) {
				Pool.Remove(item);
				Destroy(item);
			} else {
				Available.Push(item);
			}
		}

		/// <summary>
		/// Returns all items in use back to the pool
		/// </summary>
		[Button]
		public virtual void ReturnAll() {
			foreach (var item in Pool.ToList()) {
				if (InUse.Contains(item)) Return(item);
			}
		}
		
		/// <summary>
		/// Destroys all items and resets the pool
		/// </summary>
		[Button]
		public virtual void DestroyAll() {
			Available.Clear();
			InUse.Clear();
			foreach (var item in Pool) {
				Destroy(item);
			}
			Pool.Clear();
		}


		public class NoFreeItemsException : Exception {
			public override string Message =>
				"Ran out of unused items to return, increase the preload count or set 'Expandable' to true";
		}

		public IEnumerator<T> GetEnumerator() => Pool.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int Count => Pool.Count;
	}
}
