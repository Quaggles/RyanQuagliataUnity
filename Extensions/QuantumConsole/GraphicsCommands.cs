using QFSW.QC;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
    [CommandPrefix("Graphics")]
    public static class GraphicsCommands {
        [Command]
        public static bool Vsync {
            get => QualitySettings.vSyncCount > 0;
            set => QualitySettings.vSyncCount = value ? 1 : 0;
        }
        
        [Command]
        public static int VsyncCount {
            get => QualitySettings.vSyncCount;
            set => QualitySettings.vSyncCount = value;
        }
        
        [Command]
        public static float LodBias {
            get => QualitySettings.lodBias;
            set => QualitySettings.lodBias = value;
        }
        
                
        [Command]
        public static int MaximumLodLevel {
            get => QualitySettings.maximumLODLevel;
            set => QualitySettings.maximumLODLevel = value;
        }
        
        [Command]
        public static int ParticleRaycastBudget {
            get => QualitySettings.particleRaycastBudget;
            set => QualitySettings.particleRaycastBudget = value;
        }
        
        [Command]
        public static int MaxQueuedFrames {
            get => QualitySettings.maxQueuedFrames;
            set => QualitySettings.maxQueuedFrames = value;
        }
        
        [Command]
        public static bool SoftVegetation {
            get => QualitySettings.softVegetation;
            set => QualitySettings.softVegetation = value;
        }
        
        [CommandPrefix("Rendering")]
        public static class RenderingCommands {
            [Command]
            public static int PixelLightCount {
                get => QualitySettings.pixelLightCount;
                set => QualitySettings.pixelLightCount = value;
            }

            public enum TextureMipmapLevel {
                Full = 0,
                Half = 1,
                Quarter = 2,
                Eighth = 3,
            }
            
            [Command]
            [CommandDescription("Full: 0\n" +
                                "Half: 1\n" +
                                "Quarter: 2\n" +
                                "Eighth: 3")]
            public static TextureMipmapLevel TextureQuality {
                get => (TextureMipmapLevel)QualitySettings.masterTextureLimit;
                set => QualitySettings.masterTextureLimit = (int)value;
            }
            
            [Command]
            [CommandDescription("Disable: 0\n" +
                                "Enable: 1\n" +
                                "ForceEnable: 2")]
            public static AnisotropicFiltering AnisotropicFiltering {
                get => QualitySettings.anisotropicFiltering;
                set => QualitySettings.anisotropicFiltering = value;
            }

            [Command]
            public static int AntiAliasing {
                get => QualitySettings.antiAliasing;
                set => QualitySettings.antiAliasing = value;
            }
            
            [Command]
            public static bool SoftParticles {
                get => QualitySettings.softParticles;
                set => QualitySettings.softParticles = value;
            }
            
            [Command]
            public static bool RealtimeReflectionProbes {
                get => QualitySettings.realtimeReflectionProbes;
                set => QualitySettings.realtimeReflectionProbes = value;
            }
            
            [Command]
            public static bool BillboardsFaceCameraPosition {
                get => QualitySettings.billboardsFaceCameraPosition;
                set => QualitySettings.billboardsFaceCameraPosition = value;
            }
            
            [Command]
            public static float ResolutionScalingFixedDpiFactor {
                get => QualitySettings.resolutionScalingFixedDPIFactor;
                set => QualitySettings.resolutionScalingFixedDPIFactor = value;
            }
        }
        
        [CommandPrefix("Shadows")]
        public static class ShadowCommands {
            [Command]
            public static ShadowmaskMode ShadowmaskMode {
                get => QualitySettings.shadowmaskMode;
                set => QualitySettings.shadowmaskMode = value;
            }
            
            [Command]
            [CommandDescription("Disable: 0\n" +
                                "HardOnly: 1\n" +
                                "All: 2")]
            public static ShadowQuality Shadows {
                get => QualitySettings.shadows;
                set => QualitySettings.shadows = value;
            }
            
            [Command]
            [CommandDescription("Low: 0\n" +
                                "Medium: 1\n" +
                                "High: 2\n" +
                                "Very High: 3")]
            public static ShadowResolution ShadowResolution {
                get => QualitySettings.shadowResolution;
                set => QualitySettings.shadowResolution = value;
            }
            
            [Command]
            [CommandDescription("Close Fit: 0\n" +
                                "Stable Fit: 1")]
            public static ShadowProjection ShadowProjection {
                get => QualitySettings.shadowProjection;
                set => QualitySettings.shadowProjection = value;
            }
            
            [Command]
            public static float ShadowDistance {
                get => QualitySettings.shadowDistance;
                set => QualitySettings.shadowDistance = value;
            }
            
            [Command]
            public static float ShadowNearPlaneOffset {
                get => QualitySettings.shadowNearPlaneOffset;
                set => QualitySettings.shadowNearPlaneOffset = value;
            }
            
            [Command]
            public static int ShadowCascades {
                get => QualitySettings.shadowCascades;
                set => QualitySettings.shadowCascades = value;
            }
            
            [Command]
            public static float ShadowCascade2Split {
                get => QualitySettings.shadowCascade2Split;
                set => QualitySettings.shadowCascade2Split = value;
            }
            
            [Command]
            public static Vector3 ShadowCascade4Split {
                get => QualitySettings.shadowCascade4Split;
                set => QualitySettings.shadowCascade4Split = value;
            }
        }

        [CommandPrefix("TextureStreaming")]
        public static class TextureStreamingCommands {
            [Command]
            public static bool Active {
                get => QualitySettings.streamingMipmapsActive;
                set => QualitySettings.streamingMipmapsActive = value;
            }
            
            [Command]
            public static bool AddAllCameras {
                get => QualitySettings.streamingMipmapsAddAllCameras;
                set => QualitySettings.streamingMipmapsAddAllCameras = value;
            }
            
            [Command]
            public static float MemoryBudget {
                get => QualitySettings.streamingMipmapsMemoryBudget;
                set => QualitySettings.streamingMipmapsMemoryBudget = value;
            }
            
            [Command]
            public static int RenderersPerFrame {
                get => QualitySettings.streamingMipmapsRenderersPerFrame;
                //set => QualitySettings.streamingMipmapsRenderersPerFrame = value;
            }
            
            [Command]
            public static int MaxLevelReduction {
                get => QualitySettings.streamingMipmapsMaxLevelReduction;
                set => QualitySettings.streamingMipmapsMaxLevelReduction = value;
            }
            
            [Command]
            public static int MaxIORequests {
                get => QualitySettings.streamingMipmapsMaxFileIORequests;
                set => QualitySettings.streamingMipmapsMaxFileIORequests = value;
            }
        }
    }
}