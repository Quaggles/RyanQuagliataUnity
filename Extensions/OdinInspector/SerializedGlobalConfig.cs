#if !ODIN_EDITOR_ONLY
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.Utilities;

namespace RyanQuagliataUnity.Extensions.OdinInspector {
	/// <summary>
	/// Global Config class serialized by Odin Inspector instead of Unity
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class SerializedGlobalConfig<T> : GlobalConfig<T>, ISerializationCallbackReceiver where T : GlobalConfig<T>, new() {
		public const string ASSET_FOLDER = "Assets/Resources";
		
		[SerializeField, HideInInspector]
		private SerializationData serializationData;

		void ISerializationCallbackReceiver.OnAfterDeserialize() {
			UnitySerializationUtility.DeserializeUnityObject(this, ref serializationData);
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize() {
			UnitySerializationUtility.SerializeUnityObject(this, ref serializationData);
		}
	}
}
#endif