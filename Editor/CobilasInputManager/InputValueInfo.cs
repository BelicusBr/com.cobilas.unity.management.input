using System;
using UnityEngine;

namespace Cobilas.Unity.Editor.Management.InputManager {
    using KeyPressType = Unity.Management.InputManager.CobilasInputManager.KeyPressType;
    [Serializable]
    public struct InputValueInfo {
        public KeyCode myKey;
        public string displayName;
        public KeyPressType pressType;
        public bool ReadKey;


        public InputValueInfo(KeyCode myKey, KeyPressType pressType, string displayName = "") {
            this.myKey = myKey;
            this.displayName = displayName.ToUpper();
            this.pressType = pressType;
            this.ReadKey = false;
        }
    }
}
