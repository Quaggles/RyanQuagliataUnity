using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.OdinInspector {
	public class GuidSerializedScriptableObject : SerializedScriptableObject, IGuid {
		[HideInInspector, SerializeField]
		private string guid;

		[ShowInInspector, ReadOnly, PropertyOrder(Int32.MinValue)]
		public string Guid {
			get => guid;
			set => guid = value;
		}

		public ScriptableObject ScriptableObject => this;
	}
}