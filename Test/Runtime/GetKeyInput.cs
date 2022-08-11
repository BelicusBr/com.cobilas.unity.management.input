using System;
using UnityEngine;
using System.Collections;
using Cobilas.Collections;
using Cobilas.Unity.Management.Container;

namespace Cobilas.Unity.Management.InputManager {
    [AddToPermanentContainer]
    public class GetKeyInput : MonoBehaviour, ISerializationCallbackReceiver {
        [SerializeField, HideInInspector] private InputManagerType managerType;
        [SerializeField, HideInInspector] private InputKeyResult[] triggers;
        private InputKeyResult resultTemp;
        private Coroutine coroutine;
        private bool Change;
        private bool AfterDeserialize = false;

        private static GetKeyInput input;

        private void Awake() {
            if (input == null) {
                input = this;
                coroutine = StartCoroutine(ForEndOfFrame());
            } else {
                if (input != this)
                    Destroy(this);
            }
        }

        private void OnEnable() {
#if UNITY_EDITOR
            if (AfterDeserialize) {
                AfterDeserialize = false;
                coroutine = StartCoroutine(ForEndOfFrame());
            }
#endif
        }

        private IEnumerator ForEndOfFrame() {
            while (true) {
                yield return new WaitForEndOfFrame();
                Change = false;
            }
        }

        internal InputCapsuleTrigger[] Internal_GetInputCapsuleTriggers() {
            InputCapsuleTrigger[] triggers = new InputCapsuleTrigger[ArrayManipulation.ArrayLength(this.triggers)];
            for (int I = 0; I < triggers.Length; I++)
                triggers[I] = new InputCapsuleTrigger(this.triggers[I].displayName, this.triggers[I].keyCode, KeyPressType.Press);
            return triggers;
        } 

        private void Update() {
            if (managerType == InputManagerType.KeyboardCommand) return;
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
            
            switch (current.type) {
                case EventType.KeyDown:
                    Change = true;
                    if (managerType == InputManagerType.MouseCommand) break;

                    if (current.keyCode != KeyCode.None)
                        (resultTemp = GetInputKeyResult()).keyCode = current.keyCode;

                    if (resultTemp.keyCode != KeyCode.None) {
                        resultTemp.displayName = InputCapsuleUtility.KeyPadToDisplayName(resultTemp.keyCode);
                        if (resultTemp.displayName == InputCapsuleUtility.DN_None)
                            resultTemp.displayName = InputCapsuleUtility.CharacterToDisplayName(current.character);
                        if (resultTemp.displayName == InputCapsuleUtility.DN_None) {
                            switch (current.character.InterpretEscapeSequence()) {
                                case EscapeSequence.Null:
                                    resultTemp.displayName = resultTemp.keyCode.ToString();
                                    break;
                                case EscapeSequence.NewLine:
                                    resultTemp.displayName = resultTemp.keyCode.ToString();
                                    break;
                                case EscapeSequence.CarriageReturn:
                                    resultTemp.displayName = resultTemp.keyCode.ToString();
                                    break;
                                default:
                                    resultTemp.displayName = current.character.ToString();
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        private int LastIndex() {
            int index = ArrayManipulation.ArrayLength(triggers) - 1;
            return index < 0 ? 0 : index;
        }

        private void GetMouseDown(KeyCode keyCode) {
            if (Input.GetKeyDown(keyCode)) {
                Change = true;
                InputKeyResult temp = GetInputKeyResult();
                temp.keyCode = keyCode;
                temp.displayName = InputCapsuleUtility.MouseButtonToDisplayName(keyCode);
            }
        }

        private InputKeyResult GetInputKeyResult() {
            if (managerType == InputManagerType.MouseCommand || !CobilasInputManager.UseMultipleKeys) {
                if (ArrayManipulation.EmpytArray(triggers)) triggers = new InputKeyResult[] { new InputKeyResult() };
                else if (ArrayManipulation.ArrayLength(triggers) > 1)
                    Array.Resize(ref triggers, 1);
            } else ArrayManipulation.Add(new InputKeyResult(), ref triggers);
            return triggers[LastIndex()];
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() => AfterDeserialize = true;

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        public static void InformInputManagerType(InputManagerType managerType)
            => GetKeyInput.input.managerType = managerType;

        public static bool KeyChange() => GetKeyInput.input.Change;

        public static InputCapsuleTrigger[] GetInputCapsuleTriggers()
            => GetKeyInput.input.Internal_GetInputCapsuleTriggers();

        public static void ResetList()
            => ArrayManipulation.ClearArraySafe(ref GetKeyInput.input.triggers);

        [Serializable]
        private sealed class InputKeyResult {
            public string displayName;
            public KeyCode keyCode;
        }
    }
}