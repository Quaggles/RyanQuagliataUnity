namespace RyanQuagliata.ObjectPool {
	public class GenericPoolable : PoolableObject {
		protected bool inUse = false;
		public override bool InUse {
			get {
				return inUse;
			}
			set {
				inUse = value;
				gameObject.SetActive(value);
			}
		}
	}
}
