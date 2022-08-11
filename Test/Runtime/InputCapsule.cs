using System;
using UnityEngine;
using Cobilas.Collections;

namespace Cobilas.Unity.Management.InputManager {
    [CreateAssetMenu(fileName = "new InputCapsule", menuName = "InputCapsule")]
    public class InputCapsule : ScriptableObject, ISerializationCallbackReceiver {
        [SerializeField] private InputManagerType inputType;
        [SerializeField] private string displayName;
        [SerializeField] private string _ID;
        [SerializeField] private bool isHidden;
        [SerializeField] private bool isFixedInput;
        [SerializeField, HideInInspector] private bool isChange;
        [SerializeField] private InputCapsuleTrigger[] triggerFirst;
        [SerializeField] private InputCapsuleTrigger[] secondaryTrigger;
        private event Action<InputCapsuleResult, KeyPressType> specificButtonPressed;
        private bool AfterDeserialize = false;

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

        private void OnEnable() {
#if UNITY_EDITOR
            if (AfterDeserialize) {
                AfterDeserialize = false;
                specificButtonPressed = (Action<InputCapsuleResult, KeyPressType>)null;
                PopEvent(this.triggerFirst, false);
                PopEvent(this.secondaryTrigger, true);
            }
#endif
        }

        internal void SetInputManagerType(InputManagerType inputType) => this.inputType = inputType;
        internal void SetID(string ID) => this._ID = ID;
        internal void SetDisplayName(string displayName) => this.displayName = displayName;
        internal void SetIsHidden(bool isHidden) => this.isHidden = isHidden;
        internal void SetIsFixedInput(bool isFixedInput) => this.isFixedInput = isFixedInput;
        internal void SetIsChange(bool isChange) => this.isChange = isChange;

        internal void ClearEvent() => specificButtonPressed = (Action<InputCapsuleResult, KeyPressType>)null;

        internal void SetTriggerFirst(InputCapsuleTrigger[] triggerFirst) {
            ArrayManipulation.ClearArraySafe(ref this.triggerFirst);
            PopEvent(this.triggerFirst = triggerFirst, false);
        }

        internal void SetSecondaryTrigger(InputCapsuleTrigger[] secondaryTrigger) {
            ArrayManipulation.ClearArraySafe(ref this.secondaryTrigger);
            PopEvent(this.secondaryTrigger = secondaryTrigger, true);
        }

        internal void SpecificButtonPressed(InputCapsuleResult result, KeyPressType type) {
            if (result.IDTarget != _ID || TriggerFirstCount == 0 || result.Result) return;
            specificButtonPressed?.Invoke(result, type);
            bool t1 = result.Mark_TriggerFirst == TriggerFirstCount;
            bool t2 = result.Mark_SecondaryTrigger == SecondaryTriggerCount &&
                CobilasInputManager.UseSecondaryCommandKeys && SecondaryTriggerCount != 0;
            if (t1 || t2)
                result.Confirm();
        }

        private void PopEvent(InputCapsuleTrigger[] triggers, bool secondaryTrigger) {
            for (int I = 0; I < ArrayManipulation.ArrayLength(triggers); I++) {
                specificButtonPressed += triggers[I].SpecificButtonPressed;
                triggers[I].SetIsSecondaryTrigger(secondaryTrigger);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize() => AfterDeserialize = true;

        public static InputCapsuleTrigger[] CloneList(InputCapsuleTrigger[] list)
            => (InputCapsuleTrigger[])(list == null ? null : list.Clone());

        internal static InputCapsule CloneInputCapsule(InputCapsule inputCapsule) {
            InputCapsule clone = new InputCapsule();
            clone.inputType = inputCapsule.inputType;
            clone._ID = inputCapsule._ID;
            clone.displayName = inputCapsule.displayName;
            clone.isHidden = inputCapsule.isHidden;
            clone.isFixedInput = inputCapsule.isFixedInput;
            clone.ClearEvent();
            clone.SetTriggerFirst(CloneList(inputCapsule.triggerFirst));
            clone.SetSecondaryTrigger(CloneList(inputCapsule.secondaryTrigger));
            return clone;
        }
    }
}