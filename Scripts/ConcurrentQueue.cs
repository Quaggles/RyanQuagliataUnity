using System.Collections;
using System.Collections.Generic;

namespace RyanQuagliataUnity
{
	[System.Serializable]
	public class ConcurrentQueue<T> : IEnumerable<T> {
		private readonly object syncLock = new object();

		public Queue<T> Queue { get; protected set; }

		public int Count
		{
			get
			{
				lock(syncLock) 
				{
					return Queue.Count;
				}
			}
		}

		public ConcurrentQueue()
		{
			this.Queue = new Queue<T>();
		}

		public T Peek()
		{
			lock(syncLock)
			{
				return Queue.Peek();
			}
		}	

		public void Enqueue(T obj)
		{
			lock(syncLock)
			{
				Queue.Enqueue(obj);
			}
		}

		public T Dequeue()
		{
			lock(syncLock)
			{
				return Queue.Dequeue();
			}
		}

		public void Clear()
		{
			lock(syncLock)
			{
				Queue.Clear();
			}
		}

		public T[] CopyToArray()
		{
			lock(syncLock)
			{
				if(Queue.Count == 0)
				{
					return new T[0];
				}

				T[] values = new T[Queue.Count];
				Queue.CopyTo(values, 0);	
				return values;
			}
		}
		
		

		public static ConcurrentQueue<T> InitFromArray(IEnumerable<T> initValues)
		{
			var queue = new ConcurrentQueue<T>();

			if(initValues == null)	
			{
				return queue;
			}

			foreach(T val in initValues)
			{
				queue.Enqueue(val);
			}

			return queue;
		}

		public IEnumerator<T> GetEnumerator() {
			lock(syncLock) {
				return Queue.GetEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}