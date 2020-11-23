using UnityEngine;

namespace RyanQuagliataUnity.Extensions.OdinInspector {
	public interface IGuid {
		string Guid { get; set; }

		ScriptableObject ScriptableObject { get; }
	}
}