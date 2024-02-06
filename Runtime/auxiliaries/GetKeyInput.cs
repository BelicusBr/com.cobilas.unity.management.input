using System;
using UnityEngine;
using System.Collections;
using Cobilas.Collections;
using Cobilas.Unity.Management.Container;

namespace Cobilas.Unity.Management.InputManager {
    [AddSceneContainer]
    public class GetKeyInput : MonoBehaviour, ISerializationCallbackReceiver {
        [SerializeField]
        [HideInInspector]
        private InputManagerType managerType;
        [SerializeField]
        [HideInInspector]
        private InputKeyResult[] triggers;
        private InputKeyResult resultTemp;
        private bool Change;
        private static GetKeyInput input;
#if UNITY_EDITOR
        private bool AfterDeserialize = false;
#endif

        private void Awake() {
            if (input == null) {
                input = this;
                _ = StartCoroutine(ForEndOfFrame());
            } else if (input != this)
                Destroy(this);
        }

#if UNITY_EDITOR
        private void OnEnable() {
            if (!AfterDeserialize) return;
            AfterDeserialize = false;
            _ = StartCoroutine(ForEndOfFrame());
        }
#endif

        private IEnumerator ForEndOfFrame() {
            while (true) {
                yield return new WaitForEndOfFrame();
                Change = false;
            }
        }

        internal InputCapsuleTrigger[] Internal_GetInputCapsuleTriggers() {
            InputCapsuleTrigger[] inputCapsuleTriggerArray = new InputCapsuleTrigger[ArrayManipulation.ArrayLength(triggers)];
            for (int index = 0; index < inputCapsuleTriggerArray.Length; ++index)
                inputCapsuleTriggerArray[index] = new InputCapsuleTrigger(triggers[index].displayName, triggers[index].keyCode, KeyPressType.Press);
            return inputCapsuleTriggerArray;
        }

        private void Update() {
            if (managerType == InputManagerType.KeyboardCommand)
                return;
            GetMouseDown(KeyCode.Mouse0);
            GetMouseDown(KeyCode.Mouse1);
            GetMouseDown(KeyCode.Mouse2);
            GetMouseDown(KeyCode.Mouse3);
            GetMouseDown(KeyCode.Mouse4);
            GetMouseDown(KeyCode.Mouse5);
            GetMouseDown(KeyCode.Mouse6);
        }

        private void OnGUI() {
            Event current = Event.current;
            if (current.type != EventType.KeyDown ||
                managerType == InputManagerType.MouseCommand)
                return;

            if (current.keyCode != KeyCode.None)
                (resultTemp = GetInputKeyResult()).keyCode = current.keyCode;

            if (resultTemp.keyCode == KeyCode.None) return;
            Change = true;

            resultTemp.displayName = InputCapsuleUtility.KeyPadToDisplayName(resultTemp.keyCode);

            if (resultTemp.displayName == InputCapsuleUtility.DN_None)
                resultTemp.displayName = InputCapsuleUtility.CharacterToDisplayName(current.character);

            if (resultTemp.displayName == InputCapsuleUtility.DN_None) {
                EscapeSequence escapeSequence = current.character.InterpretEscapeSequence();
                switch (escapeSequence) {
                    case EscapeSequence.Null:
                    case EscapeSequence.NewLine:
                    case EscapeSequence.CarriageReturn:
                        resultTemp.displayName = resultTemp.keyCode.ToString();
                        break;
                    default:
                        current.character.ToString();
                        break;
                }
            }
        }

        private int LastIndex() {
            int num = ArrayManipulation.ArrayLength(triggers) - 1;
            return num < 0 ? 0 : num;
        }

        private void GetMouseDown(KeyCode keyCode) {
            if (!Input.GetKeyDown(keyCode))
                return;
            Change = true;
            InputKeyResult inputKeyResult = GetInputKeyResult();
            inputKeyResult.keyCode = keyCode;
            inputKeyResult.displayName = InputCapsuleUtility.MouseButtonToDisplayName(keyCode);
        }

        private InputKeyResult GetInputKeyResult()
        {
            if (managerType == InputManagerType.MouseCommand || !CobilasInputManager.UseMultipleKeys)
            {
                if (ArrayManipulation.EmpytArray(triggers))
                    triggers = new InputKeyResult[1]
                    {
            new InputKeyResult()
                    };
                else if (ArrayManipulation.ArrayLength(triggers) > 1)
                    Array.Resize<InputKeyResult>(ref triggers, 1);
            }
            else
                ArrayManipulation.Add<InputKeyResult>(new InputKeyResult(), ref triggers);
            return triggers[LastIndex()];
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
#if UNITY_EDITOR
            AfterDeserialize = true;
#endif
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        public static void InformInputManagerType(InputManagerType managerType) 
            => input.managerType = managerType;

        public static bool KeyChange() 
            => input.Change;

        public static InputCapsuleTrigger[] GetInputCapsuleTriggers() 
            => input.Internal_GetInputCapsuleTriggers();

        public static void ResetList() 
            => ArrayManipulation.ClearArraySafe<InputKeyResult>(ref input.triggers);

        [Serializable]
        private sealed class InputKeyResult {
            public string displayName;
            public KeyCode keyCode;
        }
    }
}
