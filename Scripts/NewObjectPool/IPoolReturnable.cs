using System;

namespace RyanQuagliataUnity.NewObjectPool {
	public interface IPoolReturnable {
		Action PoolReturn { set; }
	}
}