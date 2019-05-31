using System;
using UnityEngine;

namespace RyanQuagliata.ObjectPool {
	/// <summary>
	/// Abstract class for a poolable object to be using in the ObjectPooling system
	/// Date Created: 11/24/2016
	/// Author: Ryan Quagliata, RyanQuagliata@gmail.com
	/// </summary>
	public abstract class PoolableObject : EnhancedMonoBehaviour {
		/// <summary>
		/// Reference to the parent pool
		/// </summary>
		public IObjectPool ParentPool;

		/// <summary>
		/// Returns if the object is in use and cannot be retrieved by the pooling system
		/// </summary>
		public abstract bool InUse { get; set; }

		public event Action<PoolableObject> OnPoolPreloaded;

		/// <summary>
		/// Called when the object is preloaded
		/// </summary>
		public virtual void PoolPreloaded() => OnPoolPreloaded?.Invoke(this);

		public event Action<PoolableObject> OnPoolExpanded;

		/// <summary>
		/// Called when the object is created in the pool
		/// </summary>
		public virtual void PoolExpanded() => OnPoolExpanded?.Invoke(this);

		public event Action<PoolableObject> OnPoolRetrieved;

		/// <summary>
		/// Called when the object is retrieved from the pool
		/// </summary>
		public virtual void PoolRetrieved() => OnPoolRetrieved?.Invoke(this);

		public event Action<PoolableObject> OnPoolReturned;

		/// <summary>
		/// Called when the object is returned to the pool
		/// </summary>
		public virtual void PoolReturned() => OnPoolReturned?.Invoke(this);
		
		/// <summary>
		/// Returns this object to the pool for reuse
		/// </summary>
		public virtual void ReturnToPool() {
			ParentPool.Return(this);
		}
	}
}