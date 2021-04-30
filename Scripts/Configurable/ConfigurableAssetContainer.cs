using System;
using Sirenix.OdinInspector;

namespace RyanQuagliataUnity.Configurable {
	/// <summary>
	/// This container struct is required since [SerializeReference] cannot work directly on classes deriving from UnityEngine.Object
	/// </summary>
	[Serializable]
	public abstract class ConfigurableAssetContainer<TAsset, TConfig> where TAsset : ConfigurableAsset<TConfig> {
		[InlineEditor]
		public TAsset Asset;

		public TConfig GetConfig => Asset.Config;
	}
}