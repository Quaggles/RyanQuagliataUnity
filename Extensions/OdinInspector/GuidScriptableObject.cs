using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.OdinInspector {
    public abstract class GuidScriptableObject : ScriptableObject, IGuid {
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