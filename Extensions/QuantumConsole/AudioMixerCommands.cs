using QFSW.QC;
using UnityEngine;
using UnityEngine.Audio;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
    [CommandPrefix("AudioMixer")]
    public class AudioMixerCommands {
        [Command]
        public static void SetFloat(AudioMixer audioMixer, string propertyName, float volume) {
            if (Time.frameCount == 0) {
                DelayedAction.AfterFrame(() => SetFloat(audioMixer, propertyName, volume));
                return;
            }
            audioMixer.SetFloat(propertyName, volume);
        }
        
        [Command]
        public static float GetFloat(AudioMixer audioMixer, string propertyName) {
            audioMixer.GetFloat(propertyName, out var volume);
            return volume;
        }
    }
}