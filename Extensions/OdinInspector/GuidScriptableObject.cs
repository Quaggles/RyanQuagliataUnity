using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.OdinInspector {
    public abstract class GuidScriptableObject : ScriptableObject {
        [ReadOnly]
        public string Guid;
    }
}