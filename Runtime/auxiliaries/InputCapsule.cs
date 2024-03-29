﻿using System;
using UnityEngine;
using Cobilas.Collections;

namespace Cobilas.Unity.Management.InputManager {
    [CreateAssetMenu(fileName = "new InputCapsule", menuName = "CobilasInputManager/Input Capsule")]
    public class InputCapsule : ScriptableObject {
        [SerializeField]
        private InputManagerType inputType;
        [SerializeField]
        private string displayName;
        [SerializeField]
        private string _ID;
        [SerializeField]
        private bool isHidden;
        [SerializeField]
        private bool isFixedInput;
        [SerializeField]
        [HideInInspector]
        private bool isChange;
        [SerializeField]
        private InputCapsuleTrigger[] triggerFirst;
        [SerializeField]
        private InputCapsuleTrigger[] secondaryTrigger;

        private event Action<InputCapsuleResult, KeyPressType> buttonPressed;
        private event Action<InputCapsuleResult, KeyPressType> specificButtonPressed;

        public string InputID => _ID;
        public bool IsHidden => isHidden;
        public bool IsChange => isChange;
        public bool IsFixedInput => isFixedInput;
        public string DisplayName => displayName;
        public InputManagerType InputType => inputType;
        public InputCapsuleTrigger[] TriggerFirst => triggerFirst;
        public InputCapsuleTrigger[] SecondaryTrigger => secondaryTrigger;
        public int TriggerFirstCount => ArrayManipulation.ArrayLength(triggerFirst);
        public int SecondaryTriggerCount => ArrayManipulation.ArrayLength(secondaryTrigger);

#if UNITY_EDITOR

        public void Editor_ResizeTriggerFirst(int size) {
            Array.Resize(ref triggerFirst, size);
            PopEvent(triggerFirst, false);
        }

        public void Editor_ResizeSecondaryTrigger(int size) {
            Array.Resize(ref secondaryTrigger, size);
            PopEvent(secondaryTrigger, true);
        }
#endif
        internal void SetInputManagerType(InputManagerType inputType) => this.inputType = inputType;

        internal void SetID(string ID) => this._ID = ID;

        internal void SetDisplayName(string displayName) => this.displayName = displayName;

        internal void SetIsHidden(bool isHidden) => this.isHidden = isHidden;

        internal void SetIsFixedInput(bool isFixedInput) => this.isFixedInput = isFixedInput;

        internal void SetIsChange(bool isChange) => this.isChange = isChange;

        internal void ClearEvent() 
            => specificButtonPressed = buttonPressed = (Action<InputCapsuleResult, KeyPressType>)null;

        internal void SetTriggerFirst(InputCapsuleTrigger[] triggerFirst) {
            ArrayManipulation.ClearArraySafe<InputCapsuleTrigger>(ref this.triggerFirst);
            PopEvent(this.triggerFirst = triggerFirst, false);
        }

        internal void SetSecondaryTrigger(InputCapsuleTrigger[] secondaryTrigger) {
            ArrayManipulation.ClearArraySafe<InputCapsuleTrigger>(ref this.secondaryTrigger);
            PopEvent(this.secondaryTrigger = secondaryTrigger, true);
        }

        internal void SpecificButtonPressed(InputCapsuleResult result, KeyPressType type) {
            if (result.IDTarget != _ID || TriggerFirstCount == 0 || result.Result)
                return;

            specificButtonPressed?.Invoke(result, KeyPressType.AnyPress);
            buttonPressed?.Invoke(result, type);

            if (result.Mark_TriggerFirst == TriggerFirstCount ||
                (result.Mark_SecondaryTrigger == SecondaryTriggerCount && CobilasInputManager.UseSecondaryCommandKeys && SecondaryTriggerCount > 0)) {
                result.Confirm();
            }
        }

        private void PopEvent(InputCapsuleTrigger[] triggers, bool secondaryTrigger) {
            if (!ArrayManipulation.EmpytArray(triggers)) {
                for (int index = 0; index < triggers.Length - 1; ++index) {
                    this.specificButtonPressed += triggers[index].SpecificButtonPressed;
                    triggers[index].SetIsSecondaryTrigger(secondaryTrigger);
                }
                this.buttonPressed += triggers[triggers.Length - 1].SpecificButtonPressed;
                triggers[triggers.Length - 1].SetIsSecondaryTrigger(secondaryTrigger);
            }
        }

        public static InputCapsuleTrigger[] CloneList(InputCapsuleTrigger[] list) 
            => list == null ? (InputCapsuleTrigger[])(object)null : (InputCapsuleTrigger[])list.Clone();

        internal static InputCapsule CloneInputCapsule(InputCapsuleJson inputCapsule) {
            InputCapsule inputCapsule1 = new InputCapsule();
            inputCapsule1.inputType = inputCapsule.inputType;
            inputCapsule1._ID = inputCapsule._ID;
            inputCapsule1.displayName = inputCapsule.displayName;
            inputCapsule1.isHidden = inputCapsule.isHidden;
            inputCapsule1.isFixedInput = inputCapsule.isFixedInput;
            inputCapsule1.ClearEvent();
            inputCapsule1.SetTriggerFirst(CloneList(inputCapsule.triggerFirst));
            inputCapsule1.SetSecondaryTrigger(CloneList(inputCapsule.secondaryTrigger));
            return inputCapsule1;
        }

        internal static InputCapsule CloneInputCapsule(InputCapsule inputCapsule) {
            InputCapsule inputCapsule1 = new InputCapsule();
            inputCapsule1.inputType = inputCapsule.inputType;
            inputCapsule1._ID = inputCapsule._ID;
            inputCapsule1.displayName = inputCapsule.displayName;
            inputCapsule1.isHidden = inputCapsule.isHidden;
            inputCapsule1.isFixedInput = inputCapsule.isFixedInput;
            inputCapsule1.ClearEvent();
            inputCapsule1.SetTriggerFirst(CloneList(inputCapsule.triggerFirst));
            inputCapsule1.SetSecondaryTrigger(CloneList(inputCapsule.secondaryTrigger));
            return inputCapsule1;
        }
    }
}
