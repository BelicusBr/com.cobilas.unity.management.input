using System;
using UnityEngine;

namespace Cobilas.Unity.Management.InputManager {
    [Serializable]
    public struct InputCapsuleTrigger {
        [SerializeField]
        private KeyCode myKeyCode;
        [SerializeField]
        private KeyPressType pressType;
        [SerializeField]
        private string displayName;
        [SerializeField]
        private bool isSecondaryTrigger;

        public KeyCode MyKeyCode => myKeyCode;
        public KeyPressType PressType => pressType;
        public string DisplayName => displayName;
        public bool IsSecondaryTrigger => isSecondaryTrigger;

        internal InputCapsuleTrigger(string displayName, KeyCode myKeyCode, KeyPressType pressType) {
            this.displayName = displayName;
            this.isSecondaryTrigger = false;
            this.myKeyCode = myKeyCode;
            this.pressType = pressType;
        }

        internal void SetKeyPressType(KeyPressType pressType) => this.pressType = pressType;

        internal void SetIsSecondaryTrigger(bool isSecondaryTrigger) => this.isSecondaryTrigger = isSecondaryTrigger;

        internal void SpecificButtonPressed(InputCapsuleResult result, KeyPressType type) {
            switch (type) {
                case KeyPressType.Press:
                    if (pressType == KeyPressType.Press)
                        GetKey(result);
                    break;
                case KeyPressType.PressDown:
                    if (pressType == KeyPressType.PressDown)
                        GetKeyDown(result);
                    break;
                case KeyPressType.PressUp:
                    if (pressType == KeyPressType.PressUp)
                        GetKeyUp(result);
                    break;
                case KeyPressType.AnyPress:
                    switch (pressType) {
                        case KeyPressType.Press:
                            GetKey(result);
                            return;
                        case KeyPressType.PressDown:
                            GetKeyDown(result);
                            return;
                        case KeyPressType.PressUp:
                            GetKeyUp(result);
                            return;
                        default:
                            return;
                    }
            }
        }

        private void MarkResult(InputCapsuleResult result) {
            if (isSecondaryTrigger && !CobilasInputManager.UseSecondaryCommandKeys)
                return;
            if (isSecondaryTrigger) result.MarkSecondaryTrigger();
            else result.MarkTriggerFirst();
        }

        private void GetKey(InputCapsuleResult result) {
            if (Input.GetKey(myKeyCode))
                MarkResult(result);
        }

        private void GetKeyDown(InputCapsuleResult result) {
            if (Input.GetKeyDown(myKeyCode))
                MarkResult(result);
        }

        private void GetKeyUp(InputCapsuleResult result) {
            if (Input.GetKeyUp(myKeyCode))
                MarkResult(result);
        }

        public static InputCapsuleTrigger Editor_ModInputCapsuleTrigger(
          InputCapsuleTrigger trigger,
          KeyCode keyCode) {
            trigger.myKeyCode = keyCode;
            return trigger;
        }

        public static InputCapsuleTrigger Editor_ModInputCapsuleTrigger(
          InputCapsuleTrigger trigger,
          KeyPressType pressType) {
            trigger.pressType = pressType;
            return trigger;
        }

        public static InputCapsuleTrigger Editor_ModInputCapsuleTrigger(
          InputCapsuleTrigger trigger,
          string displayName) {
            trigger.displayName = displayName;
            return trigger;
        }
    }
}
