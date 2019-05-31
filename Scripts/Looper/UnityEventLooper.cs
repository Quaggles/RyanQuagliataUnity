using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace RyanQuagliata.Looper {
	[System.Serializable]
	public class UnityEventLooper : Looper {
		[System.Serializable]
		public class LoopActionType : UnityEvent<long> { }
		
		[BoxGroup, InfoBox("Listeners added in code will not be visible here but they will work")]
		public LoopActionType LoopAction;

		public UnityEventLooper(DelayType delayType) : base(delayType) { }
		public UnityEventLooper(DelayType delayType, string name) : base(delayType, name) { }
		
		protected override void InvokeLoopAction(long iterations) {
			LoopAction?.Invoke(iterations);
		}
	}
}
