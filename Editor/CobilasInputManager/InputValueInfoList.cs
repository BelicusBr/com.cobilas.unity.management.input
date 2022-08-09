using System;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using InputManagerType = Cobilas.Unity.Management.InputManager.CobilasInputManager.InputManagerType;

namespace Cobilas.Unity.Editor.Management.InputManager {
    public class InputValueInfoList : IDisposable {

        public enum InputType {
            KeyboardCommand = 0,
            MouseCommand = 1
        }

        public enum InputMouse {
            None = 0,
            Mouse0 = 323,
            Mouse1 = 324,
            Mouse2 = 325,
            Mouse3 = 326,
            Mouse4 = 327,
            Mouse5 = 328,
            Mouse6 = 329
        }

        private InputCapsuleObjectInspector serializedObject;
        private ReorderableList reorderableList;
        private GUIContent GUIContentHeader;
        private GUIContent GUIContentDisplayName;
        private GUIContent GUIContentMyKey;
        private GUIContent GUIContentPressType;
        private InputCapsuleObject target;
        private InputType Typetemp;
        private static GetKey getKey;

        public InputValueInfoList(InputCapsuleObjectInspector serializedObject, GUIContent Header, InputCapsuleObjectInspector.TitleProperty title, SerializedProperty property) {
            GUIContentHeader = Header;
            this.serializedObject = serializedObject;
            reorderableList = new ReorderableList(
                serializedObject.serializedObject,
                property
                );
            target = serializedObject.serializedObject.targetObject as InputCapsuleObject;
            SetTitle(title);
            SetElementHeight();
            reorderableList.drawHeaderCallback = DrawHeaderCallback;
            reorderableList.drawElementCallback = DrawElementCallback;
        }

        public void DrawList() {
            SetElementHeight();
            reorderableList.DoLayoutList();
        }

        public void Dispose() {
            GUIContentDisplayName = GUIContentHeader = GUIContentMyKey = GUIContentPressType = (GUIContent)null;
            target = (InputCapsuleObject)null;
            reorderableList = (ReorderableList)null;
            serializedObject = (InputCapsuleObjectInspector)null;
            if (getKey != (GetKey)null)
                getKey.Close();
        }

        private void SetTitle(InputCapsuleObjectInspector.TitleProperty title) {
            GUIContentDisplayName = title.tt_DisplayName;
            GUIContentMyKey = title.tt_MyKey;
            GUIContentPressType = title.tt_PressType;
        }

        private void SetElementHeight() {
            switch (target.Input.inputType) {
                case InputManagerType.KeyboardCommand:
                case InputManagerType.MouseCommand:
                    reorderableList.elementHeight = (EditorGUIUtility.singleLineHeight + 2f) * 3f;
                    break;
                case InputManagerType.MixedCommand:
                    reorderableList.elementHeight = (EditorGUIUtility.singleLineHeight + 2f) * 4f;
                    break;
            }
        }

        private void DrawHeaderCallback(Rect rect)
            => EditorGUI.LabelField(rect, GUIContentHeader, EditorStyles.boldLabel);

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused) {
            InputManagerType type = target.Input.inputType;

            SerializedProperty p_index = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty p_displayName = p_index.FindPropertyRelative("displayName");
            SerializedProperty p_pressType = p_index.FindPropertyRelative("pressType");
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(rect, p_displayName, GUIContentDisplayName);
            EditorGUI.EndDisabledGroup();
            rect.y += EditorGUIUtility.singleLineHeight + 2f;
            EditorGUI.PropertyField(rect, p_pressType, GUIContentPressType);
            rect.y += EditorGUIUtility.singleLineHeight + 2f;
            InputValueInfo valueInfo = (target as InputCapsuleObject).Input.inputMain[index];

            switch (type) {
                case InputManagerType.KeyboardCommand:
                    EditorGUI.BeginDisabledGroup(getKey != null);
                    if (GUI.Button(rect, valueInfo.myKey.ToString()))
                        getKey = GetKey.Init(valueInfo, type);
                    if (getKey != null) {
                        valueInfo = getKey.Input;
                        serializedObject.Repaint();
                    }
                    EditorGUI.EndDisabledGroup();
                    break;
                case InputManagerType.MouseCommand:
                    InputMouse mouse = (InputMouse)valueInfo.myKey;
                    EditorGUI.BeginChangeCheck();
                    mouse = (InputMouse)EditorGUI.EnumPopup(rect, GUIContentMyKey, mouse);
                    if (EditorGUI.EndChangeCheck()) {
                        valueInfo.myKey = (KeyCode)mouse;
                        valueInfo.displayName = MouseToDisplayName(mouse);
                    }
                    break;
                case InputManagerType.MixedCommand:
                    EditorGUI.BeginChangeCheck();
                    Typetemp = (InputType)EditorGUI.EnumPopup(rect, Typetemp);
                    if (EditorGUI.EndChangeCheck()) valueInfo.ReadKey = false;

                    rect.y += EditorGUIUtility.singleLineHeight + 2f;
                    if (Typetemp == InputType.KeyboardCommand) goto case InputManagerType.KeyboardCommand;
                    else goto case InputManagerType.MouseCommand;
                    //break;
            }
            (target as InputCapsuleObject).Input.inputMain[index] = valueInfo;
        }

        private string MouseToDisplayName(InputMouse mouse) {
            switch (mouse) {
                case InputMouse.Mouse0: return "Button Left";
                case InputMouse.Mouse1: return "Button Right";
                case InputMouse.Mouse2: return "Button Mid";
                case InputMouse.Mouse3: return "Trigger 3";
                case InputMouse.Mouse4: return "Trigger 4";
                case InputMouse.Mouse5: return "Trigger 5";
                case InputMouse.Mouse6: return "Trigger 6";
                default: return string.Empty;
            }
        }
    }
}
