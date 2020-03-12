using RyanQuagliataUnity.Extensions.OdinInspector;
using Sirenix.Utilities;

namespace RyanQuagliataUnity.Editor {
#if ODIN_INSPECTOR
	[GlobalConfig("_Project/Settings")]
	public class ModelPostProcessorConfig : SerializedGlobalConfig<ModelPostProcessorConfig> {
		public bool CreatePrefabVariantsForModels = true;
	}
#endif
}