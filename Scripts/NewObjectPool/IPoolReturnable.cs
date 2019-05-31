using System;

namespace RyanQuagliata.NewObjectPool {
	public interface IPoolReturnable {
		Action PoolReturn { set; }
	}
}