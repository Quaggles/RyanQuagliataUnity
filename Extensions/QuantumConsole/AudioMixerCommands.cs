using QFSW.QC;
using UnityEngine;
using UnityEngine.Audio;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
    [CommandPrefix("AudioMixer")]
    public class AudioMixerCommands : MonoBehaviour {
        public AudioMixer AudioMixer;

        [Command(targetType:MonoTargetType.AllInactive)]
        public void SetFloat(string propertyName, float volume) {
            if (Time.frameCount == 0) {
                DelayedAction.AfterFrame(() => SetFloat(propertyName, volume));
                return;
            }
            AudioMixer.SetFloat(propertyName, volume);
            Debug.Log($"{nameof(SetFloat)} {propertyName} {volume} {GetFloat(propertyName)}");
        }
        
        [Command(targetType:MonoTargetType.AllInactive)]
        public float GetFloat(string propertyName) {
            AudioMixer.GetFloat(propertyName, out var volume);
            return volume;
        }
    }
}