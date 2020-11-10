using QFSW.QC;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
    [CommandPrefix("Audio")]
    public static class AudioCommands {
        public const string AudioSpeakerModeDescription = "0: Raw\n" +
                                                          "1: Mono\n" +
                                                          "2: Stereo\n" +
                                                          "3: Quad\n" +
                                                          "4: Surround\n" +
                                                          "5: Mode5point1\n" +
                                                          "6: Mode7point1\n" +
                                                          "7: Prologic";

        [Command]
        [CommandDescription(AudioSpeakerModeDescription)]
        public static AudioSpeakerMode DriverCapabilities => AudioSettings.driverCapabilities;

        [Command]
        public static double DspTime => AudioSettings.dspTime;

        [Command]
        public static string GetSpatializerPluginName => AudioSettings.GetSpatializerPluginName();

#if UNITY_EDITOR
        [Command]
        public static string[] GetSpatializerPluginNames => AudioSettings.GetSpatializerPluginNames();

        [Command]
        public static void SetSpatializerPluginName(string spatializerPluginName) => AudioSettings.SetSpatializerPluginName(spatializerPluginName);

#endif

        [Command]
        public static bool Reset() => AudioSettings.Reset(AudioSettings.GetConfiguration());

        [Command]
        [CommandDescription(AudioSpeakerModeDescription)]
        public static AudioSpeakerMode SpeakerMode {
            get => AudioSettings.GetConfiguration().speakerMode;
            set {
                var settings = AudioSettings.GetConfiguration();
                if (settings.speakerMode == value) return;
                settings.speakerMode = value;
                AudioSettings.Reset(settings);
            }
        }

        [Command]
        public static int SampleRate {
            get => AudioSettings.outputSampleRate;
            set {
                var settings = AudioSettings.GetConfiguration();
                if (settings.sampleRate == value) return;
                settings.sampleRate = value;
                AudioSettings.Reset(settings);
            }
        }

        [Command]
        public static int DspBufferSize {
            get => AudioSettings.GetConfiguration().dspBufferSize;
            set {
                var settings = AudioSettings.GetConfiguration();
                if (settings.dspBufferSize == value) return;
                settings.dspBufferSize = value;
                AudioSettings.Reset(settings);
            }
        }

        [Command]
        public static int NumRealVoices {
            get => AudioSettings.GetConfiguration().numRealVoices;
            set {
                var settings = AudioSettings.GetConfiguration();
                if (settings.numRealVoices == value) return;
                settings.numRealVoices = value;
                AudioSettings.Reset(settings);
            }
        }

        [Command]
        public static int NumVirtualVoices {
            get => AudioSettings.GetConfiguration().numVirtualVoices;
            set {
                var settings = AudioSettings.GetConfiguration();
                if (settings.numVirtualVoices == value) return;
                settings.numVirtualVoices = value;
                AudioSettings.Reset(settings);
            }
        }
    }
}