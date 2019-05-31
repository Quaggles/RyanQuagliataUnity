using System;
using Sirenix.OdinInspector;

namespace RyanQuagliata.Looper {
	[System.Serializable]
	public class ActionLooper : Looper {		
		[InfoBox("Loop Action cannot be modified in the inspector, consider using \"UnityEventLooper\""), BoxGroup, ReadOnly]
		public Action<long> LoopAction;

		public ActionLooper(DelayType delayType) : base(delayType) { }
		public ActionLooper(DelayType delayType, string name) : base(delayType, name) { }
		
		protected override void InvokeLoopAction(long iterations) {
			LoopAction?.Invoke(iterations);
		}
	}
}
