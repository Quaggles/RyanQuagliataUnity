using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity.ObjectPool {
	/// <summary>
	/// Interface to allow PoolableObjects to return themselves to a generic object pool
	/// </summary>
	public interface IObjectPool {
		void Return(PoolableObject poolable);
	}

	/// <summary>
	/// Generic Object Pooling System
	/// Date Created: 11/24/2016
	/// Author: Ryan Quagliata, RyanQuagliata@gmail.com
	/// </summary>
	public class ObjectPool<T> : IObjectPool, IReadOnlyCollection<T> where T : PoolableObject {
		public IEnumerator<T> GetEnumerator() => Pool.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int Count => Pool.Count;
		public int UsedCount => InUse.Count;
		public int AvailableCount => Available.Count;

		// Template for the object this pool handles, make sure it has a component inheriting from the abstract class poolable
		public readonly GameObject Template;


		// How many items should be created when initialization occurs
		public readonly int PreloadAmount;

		/// <summary>
		/// If the pool is larger than this and objects are returned they will be destroyed
		/// </summary>
		public int PreferredPoolSize = -1;

		// Set this var to make all pool objects a child of this, if this isn't set pool objects will be a child of this GameObject
		public readonly Transform PoolParent;


		// Should this pool make itself larger if a new object is requested and none are available
		public bool Expandable = true;

		public bool AllowStartOnExpandedObjects = false;

		public readonly HashSet<T> Pool; // List of every object in the pool whether in use or not
		public readonly Stack<T> Available; // Stack of available objects for quick retrieval
		public readonly HashSet<T> InUse; // List of objects in use for iteration over

		public event Action<T> OnExpand;
		public event Action<T> OnRetrieve;
		public event Action<T> OnReturn;


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="template">Prefab of object to populate pool</param>
		/// <param name="preloadAmount">How many objects to preload</param>
		/// <param name="expandable">Will this pool expand when required</param>
		/// <param name="poolParent">Optional parent transform for pool</param>
		/// <param name="onExpand">Action gets called when the pool is expanded, assign here so it works on preloads</param>
		/// <param name="allowStartOnExpandedObjects">Should Start be called on preloaded objects</param>
		public ObjectPool(GameObject template, int preloadAmount = 1, bool expandable = true, Transform poolParent = null, Action<T> onExpand = null, bool allowStartOnExpandedObjects = false) {
			Template = template;
			PreloadAmount = preloadAmount;
			PoolParent = poolParent;
			Expandable = expandable;
			AllowStartOnExpandedObjects = allowStartOnExpandedObjects;
			CheckValidTemplate();
			OnExpand = onExpand;

			Pool = new HashSet<T>();
			InUse = new HashSet<T>();
			Available = new Stack<T>(Mathf.Max(1, preloadAmount));
			Preload(Mathf.Max(0, preloadAmount));
		}

		/// <summary>
		/// Preloads the pool with the required amount of objects
		/// </summary>
		public void Preload(int amount) {
			for (int i = 0; i < amount; i++) ExpandPool();
		}

		/// <summary>
		/// Expands the pool by creating another object and adding it to the end
		/// </summary>
		/// <returns>Reference to the object that was created</returns>
		private void ExpandPool() {
			GameObject newGameobject = Object.Instantiate(Template);
			T newPoolable = newGameobject.GetComponent<T>();

			// Poolable Objects need a reference to the object pool irrelevant of type
			// this interface defines the return method
			newPoolable.ParentPool = this;

			newGameobject.transform.SetParent(PoolParent, false);

			if (AllowStartOnExpandedObjects)
				DelayedAction.AfterFrame(() => newPoolable.InUse = false);
			else
				newPoolable.InUse = false;

			Available.Push(newPoolable);
			Pool.Add(newPoolable);
			newPoolable.PoolExpanded();
			OnExpand?.Invoke(newPoolable);
		}

		/// <summary>
		/// Retrieves an unused object from the pool
		/// </summary>
		/// <returns>An available object from the pool</returns>
		public T Retrieve() {
			// If there are no available items and the pool can expand, expand it
			if (Expandable && Available.Count == 0) ExpandPool();

			if (Available.Count <= 0) throw new NoFreeItemsException();

			T found = Available.Pop();
			InUse.Add(found); // Record this as an item inuse
			found.InUse = true;
			found.PoolRetrieved();
			OnRetrieve?.Invoke(found);
			return found;
		}

		/// <summary>
		/// Returns an object to the pool
		/// </summary>
		/// <param name="poolable">The PoolableObject to return to the pool</param>
		public void Return(PoolableObject poolable) {
			poolable.PoolReturned();
			if (poolable.InUse == false) return;
			var item = (T) poolable;
			item.InUse = false;
			OnReturn?.Invoke(item);
			InUse.Remove(item); // This object is no longer inuse

			// When returning an object and the pool is larger than it wants to be destroy it
			if (PreferredPoolSize > 0 && Pool.Count > PreferredPoolSize) {
				Pool.Remove(item);
				Object.Destroy(item.gameObject);
				return;
			}

			// Object is available 
			Available.Push(item);
			item.transform.SetParent(PoolParent);
		}

		/// <summary>
		/// Checks if the provided template contains a component with the type of this object pool
		/// </summary>
		private void CheckValidTemplate() {
			if (Template == null)
				throw new NullReferenceException("Object Pool does not have a GameObject template assigned");
			if (Template.GetComponent<T>() == null)
				throw new InvalidTemplateException(typeof(T));
		}

		/// <summary>
		/// Returns all objects to the pool
		/// </summary>
		public void Clear() {
			foreach (var item in Pool.ToList()) {
				if (item.InUse)
					Return(item);
			}
		}

		public class InvalidTemplateException : Exception {
			private readonly Type expectedType;

			public InvalidTemplateException(Type expectedType) {
				this.expectedType = expectedType;
			}

			public override string Message =>
				$"Object Pool does not have a template with a script of type \"{expectedType}\" added";
		}

		public class NoFreeItemsException : Exception {
			public override string Message =>
				"Ran out of unused items to return, increase the preload count or set 'Expandable' to true";
		}
	}
}