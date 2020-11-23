using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.OdinInspector {
	public class GuidSerializedScriptableObject : SerializedScriptableObject, IGuid {
		[HideInInspector, SerializeField]
		private string guid;

		[ShowInInspector]
		public string Guid {
			get => guid;
			set => guid = value;
		}

		public ScriptableObject ScriptableObject => this;
	}
}