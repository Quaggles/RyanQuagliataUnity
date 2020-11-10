using QFSW.QC;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
    [CommandPrefix("Time")]
    public static class TimeCommands {
        [Command]
        public static float TimeSeconds => Time.time;

        [Command]
        public static float TimeScale {
            get => Time.timeScale;
            set => Time.timeScale = value;
        }

        [Command]
        public static float FixedDeltaTime {
            get => Time.fixedDeltaTime;
            set => Time.fixedDeltaTime = value;
        }

        [Command]
        public static float MaximumDeltaTime {
            get => Time.maximumDeltaTime;
            set => Time.maximumDeltaTime = value;
        }

        [Command]
        public static float MaximumParticleDeltaTime {
            get => Time.maximumParticleDeltaTime;
            set => Time.maximumParticleDeltaTime = value;
        }

        [Command]
        public static float CaptureDeltaTime {
            get => Time.captureDeltaTime;
            set => Time.captureDeltaTime = value;
        }

        [Command]
        public static int CaptureFramerate {
            get => Time.captureFramerate;
            set => Time.captureFramerate = value;
        }
    }
}