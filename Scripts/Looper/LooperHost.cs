using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliataUnity.Looper {
	public class LooperHost : MonoBehaviour {
		private static LooperHost instance;
		
		/// <summary>
		/// Instance of the singleton host object
		/// </summary>
		public static LooperHost Instance {
			get {
				if (instance != null) return instance;
				instance = new GameObject().AddComponent<LooperHost>();
				instance.name = "[LooperHost]";
				DontDestroyOnLoad(instance);
				return instance;
			}
		}
		
		[ShowInInspector]
		public int RunningCoroutinesCount => RunningLoops.Count;

		[ShowInInspector]
		public List<Looper> RunningLoops = new List<Looper>();
	}
}
