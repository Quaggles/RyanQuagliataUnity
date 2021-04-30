using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliataUnity.Configurable {
	public abstract class ConfigurableAsset<TConfig> : ScriptableObject {
		[InlineProperty]
		public TConfig Config;
	}
}