using System;
using UnityEngine;

namespace Cobilas.Unity.Management.InputManager {
    [Serializable]
    public sealed class InputCapsuleJson {
        public InputManagerType inputType;
        public string displayName;
        public string _ID;
        public bool isHidden;
        public bool isFixedInput;
        public bool isChange;
        public InputCapsuleTrigger[] triggerFirst;
        public InputCapsuleTrigger[] secondaryTrigger;

        public static string ToJson(InputCapsuleJson input, bool prettyPrint)
            => JsonUtility.ToJson(input, prettyPrint);

        public static string ToJson(InputCapsuleJson input)
            => ToJson(input, false);

        public static InputCapsuleJson JsonToInputCapsuleJson(string txt)
            => JsonUtility.FromJson<InputCapsuleJson>(txt);
#if UNITY_EDITOR
        public static InputCapsule Editor_CloneInputCapsule(string txt)
            => InputCapsule.CloneInputCapsule(JsonToInputCapsuleJson(txt));
#endif
    }
}
