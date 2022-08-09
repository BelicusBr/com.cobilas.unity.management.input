using System;
using UnityEngine;
using UnityEditor;
using InputManagerType = Cobilas.Unity.Management.InputManager.CobilasInputManager.InputManagerType;

namespace Cobilas.Unity.Editor.Management.InputManager {
    public class GetKey : EditorWindow {
        private InputValueInfo input;
        private InputManagerType type;

        public InputValueInfo Input => input;

        public static GetKey Init(InputValueInfo input, InputManagerType type) {
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
            EditorGUILayout.LabelField($"Key code:{input.myKey}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Display name:{input.displayName}", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.EndVertical();
            switch (current.type) {
                case EventType.KeyDown:
                    if (current.keyCode != KeyCode.None)
                        input.myKey = current.keyCode;
                    if (input.myKey != KeyCode.None)
                    {
                        switch (current.character.InterpretEscapeSequence())
                        {
                            case EscapeSequence.Null:
                                input.displayName = input.myKey.ToString();
                                break;
                            case EscapeSequence.SingleQuote:
                                input.displayName = "'";
                                break;
                            case EscapeSequence.DoubleQuote:
                                input.displayName = "\"";
                                break;
                            case EscapeSequence.BackSlash:
                                input.displayName = "BackSlash";
                                break;
                            case EscapeSequence.Alert:
                                input.displayName = "Alert";
                                break;
                            case EscapeSequence.Backspace:
                                input.displayName = "BackSpace";
                                break;
                            case EscapeSequence.FormFeed:
                                input.displayName = "FormFeed";
                                break;
                            case EscapeSequence.NewLine:
                                input.displayName = input.myKey.ToString();
                                break;
                            case EscapeSequence.CarriageReturn:
                                input.displayName = input.myKey.ToString();
                                break;
                            case EscapeSequence.HorizontalTab:
                                input.displayName = "Tab";
                                break;
                            case EscapeSequence.VerticalTab:
                                input.displayName = "VerticalTab";
                                break;
                            default:
                                switch (input.myKey)
                                {
                                    case KeyCode.Keypad0:
                                    case KeyCode.Keypad1:
                                    case KeyCode.Keypad2:
                                    case KeyCode.Keypad3:
                                    case KeyCode.Keypad4:
                                    case KeyCode.Keypad5:
                                    case KeyCode.Keypad6:
                                    case KeyCode.Keypad7:
                                    case KeyCode.Keypad8:
                                    case KeyCode.Keypad9:
                                    case KeyCode.KeypadDivide:
                                    case KeyCode.KeypadMinus:
                                    case KeyCode.KeypadPlus:
                                    case KeyCode.KeypadMultiply:
                                        input.displayName = $"KP:{current.character}";
                                        break;
                                    case KeyCode.Space:
                                        input.displayName = input.myKey.ToString();
                                        break;
                                    default:
                                        input.displayName = current.character.EscapeSequenceToString();
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case EventType.KeyUp:
                    Repaint();
                    break;
            }
        }
    }

}
