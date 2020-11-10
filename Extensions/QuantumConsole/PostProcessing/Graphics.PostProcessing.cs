using System;
using System.Collections.Generic;
using System.Linq;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


namespace RyanQuagliataUnity.Extensions.QuantumConsole.PostProcessing {
    [CommandPrefix("Graphics.Post")]
    public static class GraphicsPostProcessing {
        [Command]
        public static void Profiles() {
            foreach (var postProcessProfile in LsProfiles) {
                Debug.Log(postProcessProfile.name);

                foreach (var postProcessEffectSettings in postProcessProfile.settings) {
                    Debug.Log(postProcessEffectSettings.name);
                }
            }
        }

        [Command]
        public static void SetActive<T>(bool enabled) where T : PostProcessEffectSettings {
            foreach (var postProcessProfile in LsProfiles) {
                var profile = postProcessProfile.GetSetting<T>();
                if (profile) profile.active = enabled;
            }
            foreach (var postProcessLayer in LsLayers) {
                postProcessLayer.enabled = false;
                postProcessLayer.enabled = true;
            }
        }
        
        [Command] public static IEnumerable<PostProcessProfile> LsProfiles => ObjectExtensions.FindSceneObjectsOfType<PostProcessVolume>(true).Select(x => x.profile);
        [Command] public static IEnumerable<PostProcessEffectSettings> LsModules => QFSW.QC.Utilities.CollectionExtensions.DistinctBy(LsProfiles.SelectMany(x => x.settings), x => x.name);
        [Command] public static IEnumerable<PostProcessLayer> LsLayers => ObjectExtensions.FindSceneObjectsOfType<PostProcessLayer>(true);

        private static PostProcessProfile FirstPostProfile(string name) => LsProfiles.FirstOrDefault(x => MatchPostProfile(x, name));
        private static PostProcessProfile FirstOrDefaultPostProfile(string name) => LsProfiles.FirstOrDefault(x => MatchPostProfile(x, name));
        private static bool MatchPostProfile(this PostProcessProfile that, string name) => QFSW.QC.Utilities.StringExtensions.Contains(that.name, name, StringComparison.CurrentCultureIgnoreCase);

        private static PostProcessEffectSettings FirstPostProcessEffectSettings(this PostProcessProfile that, string name) => that.settings.First(x => MatchPostProcessEffectSettings(x, name));
        private static PostProcessEffectSettings FirstOrDefaultPostProcessEffectSettings(this PostProcessProfile that, string name) => that.settings.FirstOrDefault(x => MatchPostProcessEffectSettings(x, name));
        private static bool MatchPostProcessEffectSettings(this PostProcessEffectSettings that, string name) => QFSW.QC.Utilities.StringExtensions.Contains(that.name, name, StringComparison.CurrentCultureIgnoreCase);
        
        private static void SetActive(this PostProcessEffectSettings that, bool active) => that.active = active;

        [Command]
        public static void SetActive(string profileName, string moduleName, bool enabled) {
            FirstPostProfile(profileName)?.FirstPostProcessEffectSettings(moduleName)?.SetActive(enabled);
            foreach (var postProcessLayer in LsLayers) {
                postProcessLayer.enabled = false;
                postProcessLayer.enabled = true;
            }
        }

        [Command]
        public static void SetActive(string moduleName, bool enabled) {
            var any = false;
            foreach (var postProcessProfile in Resources.FindObjectsOfTypeAll<PostProcessProfile>()) {
                var profile = postProcessProfile.FirstOrDefaultPostProcessEffectSettings(moduleName);
                if (!profile) continue;
                profile.SetActive(enabled);
                any = true;
            }
            if (!any) throw new InvalidOperationException($"No instances of module {moduleName} found");
            foreach (var postProcessLayer in LsLayers) {
                postProcessLayer.enabled = false;
                postProcessLayer.enabled = true;
            }
        }

        private static PostProcessLayer FirstPostLayer(string name = null) => string.IsNullOrWhiteSpace(name) ? LsLayers.FirstOrDefault() : LsLayers.FirstOrDefault(x => x.gameObject.name.Contains(name));

        [Command]
        [CommandDescription("None: 0\n" +
                            "FXAA: 1\n" +
                            "SMAA: 2\n" + 
                            "TXAA: 3")]
        public static PostProcessLayer.Antialiasing GetAntiAliasingMode (string name = "") => FirstPostLayer(name).antialiasingMode;

        [Command]
        [CommandDescription("None: 0\n" +
                            "FXAA: 1\n" +
                            "SMAA: 2\n" + 
                            "TXAA: 3")]
        public static void SetAntiAliasingMode (PostProcessLayer.Antialiasing antialiasingMode, string name = "") => FirstPostLayer(name).antialiasingMode = antialiasingMode;

        
        [CommandPrefix("FXAA.")]
        public static class FXAACommands {
            [Command]
            public static void Use() => FirstPostLayer().antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;

            [Command]
            public static bool FastMode {
                get => FirstPostLayer().fastApproximateAntialiasing.fastMode;
                set => FirstPostLayer().fastApproximateAntialiasing.fastMode = value;
            }
            
            [Command]
            public static bool KeepAlpha {
                get => FirstPostLayer().fastApproximateAntialiasing.keepAlpha;
                set => FirstPostLayer().fastApproximateAntialiasing.keepAlpha = value;
            }
        }      
        
        [CommandPrefix("SMAA.")]
        public static class SMAACommands {
            [Command]
            public static void Use() => FirstPostLayer().antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
            
            [Command]
            public static SubpixelMorphologicalAntialiasing.Quality Quality {
                get => FirstPostLayer().subpixelMorphologicalAntialiasing.quality;
                set => FirstPostLayer().subpixelMorphologicalAntialiasing.quality = value;
            }
        }
        
        [CommandPrefix("TXAA.")]
        public static class TXAACommands {
            [Command]
            public static void Use() => FirstPostLayer().antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
            
            [Command]
            public static  float JitterSpread {
                get => FirstPostLayer().temporalAntialiasing.jitterSpread;
                set => FirstPostLayer().temporalAntialiasing.jitterSpread = value;
            }
            
            [Command]
            public static  float StationaryBlending {
                get => FirstPostLayer().temporalAntialiasing.stationaryBlending;
                set => FirstPostLayer().temporalAntialiasing.stationaryBlending = value;
            }
            
            [Command]
            public static  float MotionBlending {
                get => FirstPostLayer().temporalAntialiasing.motionBlending;
                set => FirstPostLayer().temporalAntialiasing.motionBlending = value;
            }
            
            [Command]
            public static  float Sharpness {
                get => FirstPostLayer().temporalAntialiasing.sharpness;
                set => FirstPostLayer().temporalAntialiasing.sharpness = value;
            }
        }

        [CommandPrefix("MotionBlur.")]
        public static class MotionBlurCommands {
            private static IEnumerable<MotionBlur> GetModules => LsModules.OfType<MotionBlur>();
            
            [Command]
            public static void Active(bool active) {
                foreach (var motionBlur in GetModules) motionBlur.active = active;
            }
            
            [Command]
            public static void ShutterAngle(float value) {
                foreach (var motionBlur in GetModules) motionBlur.shutterAngle.value = value;
            }
            
            [Command]
            public static void SampleCount(int value) {
                foreach (var motionBlur in GetModules) motionBlur.sampleCount.value = value;
            }
        }
    }
}