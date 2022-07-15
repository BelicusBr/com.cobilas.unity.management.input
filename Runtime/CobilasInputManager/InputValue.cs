using System;
using UnityEngine;
using Cobilas.Unity.Mono;

namespace Cobilas.Unity.Management.InputManager {
    using KeyPressType = CobilasInputManager.KeyPressType;
    using KeyStatus = CobilasInputManager.KeyStatus;
    [Serializable]
    public struct InputValue {
        [SerializeField, Rotulo] private KeyCode myKey;
        [SerializeField, Rotulo] private string displayName;
        [SerializeField, Rotulo] private KeyPressType pressType;

        public KeyCode MyKey => myKey;
        public string DisplayName => displayName;
        public KeyPressType PressType => pressType;
        public bool IsMouse {
            get {
                switch (myKey) {
                    case KeyCode.Mouse0: case KeyCode.Mouse1: case KeyCode.Mouse2:
                    case KeyCode.Mouse3: case KeyCode.Mouse4: case KeyCode.Mouse5:
                    case KeyCode.Mouse6: return true;
                    default: return false;
                }
            }
        }

        public InputValue(KeyCode myKey, KeyPressType pressType, string displayName = "") {
            this.myKey = myKey;
            this.displayName = displayName.ToUpper();
            this.pressType = pressType;
        }

        public void ButtonPressed(InputCapsule capsule) {
            switch (pressType) {
                case KeyPressType.Press:
                    if (Input.GetKey(myKey))
                        capsule.MarkInput();
                    break;
                case KeyPressType.PressDown:
                    if (Input.GetKeyDown(myKey))
                        capsule.MarkInput();
                    break;
                case KeyPressType.PressUp:
                    if (Input.GetKeyUp(myKey))
                        capsule.MarkInput();
                    break;
            }
        }

        public KeyStatus ButtonPressedStatus() {
            KeyStatus status = new KeyStatus();
            status.KeyPress = Input.GetKey(myKey);
            status.KeyPressDown = Input.GetKeyDown(myKey);
            status.KeyPressUp = Input.GetKeyUp(myKey);
            return status;
        }

#if UNITY_EDITOR
        public void ModMyKey(KeyCode myKey) => this.myKey = myKey;

        public void ModDisplayName(string displayName) => this.displayName = displayName;

        public void ModPressType(KeyPressType pressType) => this.pressType = pressType;
#endif
    }
}
