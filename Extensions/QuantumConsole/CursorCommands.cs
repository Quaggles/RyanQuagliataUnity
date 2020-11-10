using QFSW.QC;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
    [CommandPrefix("Cursor")]
    public static class CursorCommands {
        [Command]
        public static bool Visible {
            get => Cursor.visible;
            set => Cursor.visible = value;
        }

        [Command]
        [CommandDescription("None: 0\n" +
                            "Locked: 1\n" +
                            "Confined: 2")]
        public static CursorLockMode LockState {
            get => Cursor.lockState;
            set => Cursor.lockState = value;
        }
    }
}