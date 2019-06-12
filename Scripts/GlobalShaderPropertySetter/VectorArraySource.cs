using System;
using UnityEngine;

namespace RyanQuagliata.GlobalShaderPropertySetter {
	public class VectorArraySource :
#if ODIN_INSPECTOR && !ODIN_EDITOR_ONLY
		Sirenix.OdinInspector.SerializedMonoBehaviour
#else
		MonoBehaviour
#endif
	{
		public Func<Vector4> GetVector4;
		public Func<Vector3> GetVector3;
		public Func<Vector2> GetVector2;
		public Func<float> GetVector1;

		public Vector4 Get() {
			if (GetVector4 != null) return GetVector4.Invoke();
			if (GetVector3 != null) return GetVector3.Invoke();
			if (GetVector2 != null) return GetVector2.Invoke();
			if (GetVector1 != null) return new Vector4(GetVector1.Invoke(), 0, 0, 0);
			return Vector4.zero;
		}
	}
}