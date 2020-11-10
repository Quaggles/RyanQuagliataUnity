using System;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Playables;

namespace RyanQuagliataUnity.Extensions.QuantumConsole.Timeline {
    [CommandPrefix("PlayableDirector")]
    public static class PlayableDirectorCommands {
        [Command]
        public static bool IncludeInactive = false;
        
        [Command]
        public static void List() {
            var items = ObjectExtensions.FindLoadedObjectsOfType<PlayableDirector>(IncludeInactive);
            for (var i = 0; i < items.Count; i++) {
                var item = items[i];
                Debug.Log($"[{i}] {item.name}");
            }
        }

        public static PlayableDirector SearchPlayableDirectors(string name) {
            foreach (var playableDirector in ObjectExtensions.FindLoadedObjectsOfType<PlayableDirector>(IncludeInactive)) {
                if (playableDirector.name.Contains(name, StringComparison.InvariantCultureIgnoreCase)) return playableDirector;
            }
            throw new InvalidOperationException($"No {typeof(PlayableDirector).Name} with name {name} could be found");
        }
        
        private static PlayableDirector SearchPlayableDirectors(int index) => ObjectExtensions.FindLoadedObjectsOfType<PlayableDirector>()[index];

        [Command]
        public static double GetTime(string name) => SearchPlayableDirectors(name).time;

        [Command]
        public static double GetTimeIndex(int index) => SearchPlayableDirectors(index).time;

        [Command]
        public static void SetTime(string name, double time) => SearchPlayableDirectors(name).time = time;

        [Command]
        public static void SetTimeIndex(int index, double time) => SearchPlayableDirectors(index).time = time;
        
        [Command]
        public static void SetSpeed(string name, double speed) => SearchPlayableDirectors(name).playableGraph.GetRootPlayable(0).SetSpeed(speed);
        
        [Command]
        public static void GetSpeed(string name) => SearchPlayableDirectors(name).playableGraph.GetRootPlayable(0).GetSpeed();
        
        [Command]
        public static void Play(string name) => SearchPlayableDirectors(name).Play();

        [Command]
        public static void PlayIndex(int index) => SearchPlayableDirectors(index).Play();
        
        [Command]
        public static void Pause(string name) => SearchPlayableDirectors(name).Pause();
        
        [Command]
        public static void PauseIndex(int index) => SearchPlayableDirectors(index).Pause();
    }
}