using System;
using UnityEngine;
using UnityEditor;
using Cobilas.Unity.Management.InputManager;

namespace Cobilas.Unity.Editor.Management.InputManager {
    public class GetKey : EditorWindow {
        private InputCapsuleTrigger input;
        private InputManagerType type;

        public InputCapsuleTrigger Input => input;

        public static GetKey Init(InputCapsuleTrigger input, InputManagerType type) {
            GetKey key = GetWindow<GetKey>();
            key.type = type;
            key.input = input;
            key.titleContent = new GUIContent("Get key");
            key.minSize = key.maxSize = new Vector2(310f, 70f);
            key.Show();
            return key;
        }

        private void OnGUI()
        {
            Event current = Event.current;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"InputManager type:{type}", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField($"Key code:{input.MyKeyCode}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Display name:{input.DisplayName}", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.EndVertical();
            switch (current.type) {
                case EventType.KeyDown:
                    if (current.keyCode != KeyCode.None)
                        input = InputCapsuleTrigger.Editor_ModInputCapsuleTrigger(input, current.keyCode);
                    if (input.MyKeyCode != KeyCode.None) {
                        string displayName = string.Empty;
                        switch (current.character.InterpretEscapeSequence()) {
                            case EscapeSequence.Null:
                                displayName = input.MyKeyCode.ToString();
                                break;
                            case EscapeSequence.SingleQuote:
                                displayName = "'";
                                break;
                            case EscapeSequence.DoubleQuote:
                                displayName = "\"";
                                break;
                            case EscapeSequence.BackSlash:
                                displayName = "BackSlash";
                                break;
                            case EscapeSequence.Alert:
                                displayName = "Alert";
                                break;
                            case EscapeSequence.Backspace:
                                displayName = "BackSpace";
                                break;
                            case EscapeSequence.FormFeed:
                                displayName = "FormFeed";
                                break;
                            case EscapeSequence.NewLine:
                                displayName = InputCapsuleUtility.KeyPadToDisplayName(input.MyKeyCode);
                                displayName = displayName == InputCapsuleUtility.DN_None ? "Return" : displayName;
                                break;
                            case EscapeSequence.CarriageReturn:
                                displayName = input.MyKeyCode.ToString();
                                break;
                            case EscapeSequence.HorizontalTab:
                                displayName = "Tab";
                                break;
                            case EscapeSequence.VerticalTab:
                                displayName = "VerticalTab";
                                break;
                            default:
                                displayName = InputCapsuleUtility.KeyPadToDisplayName(input.MyKeyCode);
                                switch (displayName) {
                                    case InputCapsuleUtility.DN_None:
                                        if (input.MyKeyCode == KeyCode.Space) displayName = "Space";
                                        else displayName = current.character.EscapeSequenceToString();
                                        break;
                                }
                                break;
                        }
                        input = InputCapsuleTrigger.Editor_ModInputCapsuleTrigger(input, displayName);
                    }
                    break;
                case EventType.KeyUp:
                    Repaint();
                    break;
            }
        }
    }

}
