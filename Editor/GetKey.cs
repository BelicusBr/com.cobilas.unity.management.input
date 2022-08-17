using System;
using UnityEditor;
using UnityEngine;
using Cobilas.Unity.Management.InputManager;

namespace Cobilas.Unity.Editor.Management.InputManager {
    public class GetKey : EditorWindow {
        private int indexTarget;
        private string guiTarget;
        private InputManagerType type;
        private InputCapsuleTrigger input;

        public string GUITarget => guiTarget;
        public int IndexTarget => indexTarget;
        public InputCapsuleTrigger Input => input;
        
        public static GetKey Init(InputCapsuleTrigger input, int indexTarget, string guiTarget, InputManagerType type) {
            GetKey window = GetWindow<GetKey>();
            window.guiTarget = guiTarget;
            window.indexTarget = indexTarget;
            window.type = type;
            window.input = input;
            window.titleContent = new GUIContent("Get key");
            window.maxSize = window.minSize = new Vector2(310f, 70f);
            window.Show();
            return window;
        }
        
        private void OnGUI() {
            Event current = Event.current;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(string.Format("InputManager type:{0}", type), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(string.Format("Key code:{0}", input.MyKeyCode), EditorStyles.boldLabel);
            EditorGUILayout.LabelField(string.Format("Display name:{0}", input.DisplayName), EditorStyles.boldLabel);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            switch (current.type) {
                case EventType.KeyDown:
                    if (current.keyCode != KeyCode.None)
                        input = InputCapsuleTrigger.Editor_ModInputCapsuleTrigger(input, current.keyCode);
                    if (input.MyKeyCode == KeyCode.None)
                        break;
                    string displayName1;
                    switch (current.character.InterpretEscapeSequence()) {
                        case EscapeSequence.SingleQuote:
                            displayName1 = "'";
                            break;
                        case EscapeSequence.DoubleQuote:
                            displayName1 = "\"";
                            break;
                        case EscapeSequence.BackSlash:
                            displayName1 = "BackSlash";
                            break;
                        case EscapeSequence.Null:
                            displayName1 = input.MyKeyCode.ToString();
                            break;
                        case EscapeSequence.Alert:
                            displayName1 = "Alert";
                            break;
                        case EscapeSequence.Backspace:
                            displayName1 = "BackSpace";
                            break;
                        case EscapeSequence.FormFeed:
                            displayName1 = "FormFeed";
                            break;
                        case EscapeSequence.NewLine:
                            string displayName2 = InputCapsuleUtility.KeyPadToDisplayName(input.MyKeyCode);
                            displayName1 = displayName2 == InputCapsuleUtility.DN_None ? "Return" : displayName2;
                            break;
                        case EscapeSequence.CarriageReturn:
                            displayName1 = input.MyKeyCode.ToString();
                            break;
                        case EscapeSequence.HorizontalTab:
                            displayName1 = "Tab";
                            break;
                        case EscapeSequence.VerticalTab:
                            displayName1 = "VerticalTab";
                            break;
                        default:
                            displayName1 = InputCapsuleUtility.KeyPadToDisplayName(input.MyKeyCode);
                            if (displayName1 == InputCapsuleUtility.DN_None)
                                displayName1 = input.MyKeyCode != KeyCode.Space ? current.character.ToString() : "Space";
                            break;
                    }
                    input = InputCapsuleTrigger.Editor_ModInputCapsuleTrigger(input, displayName1);
                    break;
                case EventType.KeyUp:
                    Repaint();
                    break;
            }
        }
    }
}
