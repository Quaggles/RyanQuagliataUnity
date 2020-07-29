using RyanQuagliataUnity.Extensions.OdinInspector;
using Sirenix.Utilities;

namespace RyanQuagliataUnity.Editor {
#if ODIN_INSPECTOR
	[GlobalConfig(ASSET_FOLDER)]
	public class ModelPostProcessorConfig : SerializedGlobalConfig<ModelPostProcessorConfig> {
		public bool CreatePrefabVariantsForModels = true;
		public bool MeshSerializerEnabled = true;
	}
#endif
}