using System;
using UnityEngine;
using Cobilas.Collections;

namespace Cobilas.Unity.Editor.Management.InputManager {
    using InputManagerType = Unity.Management.InputManager.CobilasInputManager.InputManagerType;
    [Serializable]
    public class InputCapsuleInfo {
        public bool InputCapsule_Collaps;
        public bool InputMain_Collaps;
        public bool SecondaryInput_Collaps;
        public InputManagerType inputType;
        public string inputName;
        public string inputID;
        public bool isHidden;
        public bool isFixedInput;
        public InputValueInfo[] inputMain;
        public InputValueInfo[] secondaryInput;

        public int InputMainCount => ArrayManipulation.ArrayLength(inputMain);
        public int SecondaryInputCount => ArrayManipulation.ArrayLength(secondaryInput);

        public InputCapsuleInfo(string inputName, string inputID, bool isHidden, bool isFixedInput, InputManagerType inputType) {
            this.inputName = inputName;
            this.inputID = inputID;
            this.inputType = inputType;
            this.isHidden = isHidden;
            this.isFixedInput = isFixedInput;
        }

        public InputCapsuleInfo(string inputName, string inputID, InputManagerType inputType) :
            this(inputName, inputID, false, false, inputType)
        { }
    }
}
