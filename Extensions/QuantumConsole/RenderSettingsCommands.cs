using QFSW.QC;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
    [CommandPrefix("RenderSettings")]
    public class RenderSettingsCommands {
        [Command]
        public static bool Fog {
            get => RenderSettings.fog;
            set => RenderSettings.fog = value;
        }
        
        [Command]
        public static float FogDensity {
            get => RenderSettings.fogDensity;
            set => RenderSettings.fogDensity = value;
        }
        
        [Command]
        public static float FogStartDistance {
            get => RenderSettings.fogStartDistance;
            set => RenderSettings.fogStartDistance = value;
        }
        
        [Command]
        public static float FogEndDistance {
            get => RenderSettings.fogEndDistance;
            set => RenderSettings.fogEndDistance = value;
        }
        
        [Command]
        public static FogMode FogMode {
            get => RenderSettings.fogMode;
            set => RenderSettings.fogMode = value;
        }
    }
}