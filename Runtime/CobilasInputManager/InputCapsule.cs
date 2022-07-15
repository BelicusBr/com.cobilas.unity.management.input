using System;
using UnityEngine;
using Cobilas.Unity.Mono;
using Cobilas.Collections;

namespace Cobilas.Unity.Management.InputManager {
    using KeyStatus = CobilasInputManager.KeyStatus;
    using InputManagerType = CobilasInputManager.InputManagerType;
    using ButtonPressedResult = CobilasInputManager.ButtonPressedResult;
    [Serializable]
    public sealed class InputCapsule : IDisposable {
        [SerializeField, Rotulo] private InputManagerType inputType;
        [SerializeField, Rotulo] private string inputName;
        [SerializeField, Rotulo] private string inputID;
        [SerializeField, Rotulo] private bool isHidden;
        [SerializeField, Rotulo] private bool isFixedInput;
        private byte mark1 = 0;
        private byte mark2 = 0;
        private bool nextMark = false;
        private event Action<InputCapsule> buttonPressedMark1 = (Action<InputCapsule>)null;
        private event Action<InputCapsule> buttonPressedMark2 = (Action<InputCapsule>)null;
        [SerializeField] private InputValue[] inputMain;
        [SerializeField] private InputValue[] secondaryInput;
        private bool disposedValue;

        public string InputName { get => inputName; set => inputName = value; }
        public string InputID => inputID;
        public bool Disposed => disposedValue;
        public bool IsFixedInput => isFixedInput;
        public bool IsHidden => isHidden;
        public InputManagerType InputType => inputType;
        public InputValue[] InputMain => inputMain;
        public InputValue[] SecondaryInput => secondaryInput;
        public int InputMainCount => ArrayManipulation.ArrayLength(inputMain);
        public int SecondaryInputCount => CobilasInputManager.UseSecondaryCommandKeys ? ArrayManipulation.ArrayLength(secondaryInput) : 0;

        public InputCapsule(string inputName, string inputID, bool isHidden, bool isFixedInput, InputManagerType inputType) {
            this.inputName = inputName;
            this.inputID = inputID;
            this.inputType = inputType;
            this.isHidden = isHidden;
            this.isFixedInput = isFixedInput;
        }

        public InputCapsule(string inputName, string inputID, InputManagerType inputType) :
            this(inputName, inputID, false, false, inputType) { }

        ~InputCapsule()
            => Dispose();

        public void ClearInputMain() { 
            ArrayManipulation.ClearArraySafe(ref this.inputMain);
            buttonPressedMark1 = (Action<InputCapsule>)null;
        }

        public void ClearSecondaryInput() {
            ArrayManipulation.ClearArraySafe(ref this.secondaryInput);
            buttonPressedMark2 = (Action<InputCapsule>)null;
        }

        public void SetInputMain(InputValue[] inputs) {
            ClearInputMain();
            this.inputMain = PopulateTaggingEvents(inputs, false);
        }

        public void SetSecondaryInput(InputValue[] inputs) {
            ClearSecondaryInput();
            if (CobilasInputManager.UseSecondaryCommandKeys)
                this.secondaryInput = PopulateTaggingEvents(inputs, true);
        }

        public void SpecificButtonPressedStatus(string inputID, ButtonPressedResult result) {
            if (this.inputID != inputID || result == null || InputMainCount == 0) return;
            KeyStatus status = inputMain[0].ButtonPressedStatus();
            switch (result.KeyType) {
                case CobilasInputManager.KeyPressType.Press:
                    if (status.KeyPress)
                        result.Mark();
                    break;
                case CobilasInputManager.KeyPressType.PressDown:
                    if (status.KeyPressDown)
                        result.Mark();
                    break;
                case CobilasInputManager.KeyPressType.PressUp:
                    if (status.KeyPressUp)
                        result.Mark();
                    break;
            }
        }

        public void buttonPressed(string inputID, ButtonPressedResult result) {
            if (this.inputID != inputID || result == null) return;
            mark1 = mark2 = 0;
            nextMark = false;
            buttonPressedMark1?.Invoke(this);
            
            nextMark = true;
            if (CobilasInputManager.UseSecondaryCommandKeys) {
                buttonPressedMark2?.Invoke(this);
                if ((InputMainCount > 0 && mark1 == InputMainCount) || (SecondaryInputCount > 0 && mark2 == SecondaryInputCount))
                    result.Mark();
            } else {
                if (InputMainCount > 0 && mark1 == InputMainCount)
                    result.Mark();
            }
        }

        private InputValue[] PopulateTaggingEvents(InputValue[] inputs, bool Secondary) {
            for (int I = 0; I < ArrayManipulation.ArrayLength(inputs); I++) {
                if (Secondary) buttonPressedMark2 += inputs[I].ButtonPressed;
                else buttonPressedMark1 += inputs[I].ButtonPressed;
            }
            return inputs;
        }

        public void Dispose() {
            if (!disposedValue) {
                inputID = inputName = (string)null;
                ClearInputMain();
                ClearSecondaryInput();

                disposedValue = true;
            }
        }

#if UNITY_EDITOR
        public void ModInputName(string inputName) => this.inputName = inputName;

        public void ModInputID(string inputID) => this.inputID = inputID;

        public void ModInputType(InputManagerType inputType) => this.inputType = inputType;

        public void ModIsHidden(bool isHidden) => this.isHidden = isHidden;

        public void ModIsFixedInput(bool isFixedInput) => this.isFixedInput = isFixedInput;

        public void ModInputMain(InputValue[] inputMain)
            => SetInputMain(inputMain);

        public void ModSecondaryInput(InputValue[] secondaryInput)
            => SetSecondaryInput(secondaryInput);
#endif

        internal void MarkInput() {
            if (nextMark) mark2++;
            else mark1++;
        }
    }
}
