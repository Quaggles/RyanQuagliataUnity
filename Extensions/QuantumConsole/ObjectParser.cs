using System;
using QFSW.QC;
using RyanQuagliataUnity.Extensions.OdinInspector;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
	public class ObjectParser : PolymorphicQcParser<Object> {
		public override Object Parse(string value, Type type) {
#if !ODIN_INSPECTOR
			throw new ParserInputException($"ODIN_INSPECTOR not available, cannot search StandaloneAssetDatabase");
#else
			var method = typeof(StandaloneAssetDatabaseV2).GetMethod(nameof(StandaloneAssetDatabaseV2.Get)).MakeGenericMethod(type);
			return method.Invoke(null, new object[]{value, SearchType.Matches, false}) as Object;
#endif
		}
	}
}