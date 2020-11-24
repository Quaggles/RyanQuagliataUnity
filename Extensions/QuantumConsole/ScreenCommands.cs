using System.Linq;
using QFSW.QC;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
    [CommandPrefix("Screen")]
    public static class ScreenCommands {
        [Command]
        public static int TargetFrameRate {
            get => Application.targetFrameRate;
            set => Application.targetFrameRate = value;
        }

        [Command]
        public static bool FullScreen {
            get => Screen.fullScreen;
            set => Screen.fullScreen = value;
        }
        
        

        [Command]
        public static bool FullScreenDelayed {
            get => Screen.fullScreen;
            set => DelayedAction.AfterFrame(() => Screen.fullScreen = value);
        }

        [Command]
        [CommandDescription("ExclusiveFullScreen: 0\n" +
                            "FullScreenWindow: 1\n" +
                            "MaximizedWindow: 2\n" +
                            "Windowed: 3")]
        public static FullScreenMode FullScreenMode {
            get => Screen.fullScreenMode;
            set => Screen.fullScreenMode = value;
        }

        [Command]
        public static Resolution GetResolution => Screen.currentResolution;

        [Command]
        public static float ResolutionScale {
            get {
                var native = GetNativeResolution;
                return (Mathf.InverseLerp(0, native.width, Screen.currentResolution.width) +
                        Mathf.InverseLerp(0, native.height, Screen.currentResolution.height)) / 2f;
            }
            set {
                var native = GetNativeResolution;
                SetResolution(Mathf.FloorToInt(native.width * value), Mathf.FloorToInt(native.height * value));
            }
        }
        
        [Command]
        public static Vector2 ResolutionScale2 {
            get {
                var native = GetNativeResolution;
                return new Vector2(Mathf.InverseLerp(0, native.width, Screen.currentResolution.width), Mathf.InverseLerp(0, native.height, Screen.currentResolution.height));
            }
            set {
                var native = GetNativeResolution;
                SetResolution(Mathf.FloorToInt(native.width * value.x), Mathf.FloorToInt(native.height * value.y));
            }
        }

        [Command]
        public static Resolution[] GetResolutions => Screen.resolutions;

        [Command]
        public static void SetResolution(int width, int height, bool fullscreen, int refreshRate) => Screen.SetResolution(width, height, fullscreen, refreshRate);
        
        [Command]
        public static void SetResolution(int width, int height) => Screen.SetResolution(width, height, Screen.fullScreen, Screen.currentResolution.refreshRate);

        [Command]
        public static void SetResolution(int index) => SetResolution(Screen.resolutions[index]);
        
        [Command]
        public static void SetNativeResolution() => SetResolution(GetNativeResolution);

        [Command]
        public static Resolution GetNativeResolution => Screen.resolutions.Last();

        private static void SetResolution(Resolution resolution) => Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);

        [Command]
        public static int RefreshRate {
            get => Screen.currentResolution.refreshRate;
            set => Screen.SetResolution(Screen.currentResolution.height, Screen.currentResolution.width, Screen.fullScreen, value);
        }
        
        [Command]
        public static void SetPosition(int posX, int posY) => DisplayExtensions.SetPosition(posX, posY, 0, 0);
    }
}