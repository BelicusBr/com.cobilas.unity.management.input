using System;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Cobilas.Unity.Management.InputManager;

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

        private InputCapsuleInspector serializedObject;
        private ReorderableList reorderableList;
        private GUIContent GUIContentHeader;
        private GUIContent GUIContentDisplayName;
        private GUIContent GUIContentMyKey;
        private GUIContent GUIContentPressType;
        private InputCapsule target;
        private InputType Typetemp;
        private static GetKey getKey;

        public InputValueInfoList(InputCapsuleInspector serializedObject, GUIContent Header, InputCapsuleInspector.TitleProperty title, SerializedProperty property) {
            GUIContentHeader = Header;
            this.serializedObject = serializedObject;
            reorderableList = new ReorderableList(
                serializedObject.serializedObject,
                property
                );
            target = serializedObject.serializedObject.targetObject as InputCapsule;
            SetTitle(title);
            SetElementHeight();
            reorderableList.drawHeaderCallback = DrawHeaderCallback;
            reorderableList.drawElementCallback = DrawElementCallback;
        }

        public void DrawList() {
            if ((!CobilasInputManager.UseMultipleKeys && reorderableList.serializedProperty.arraySize > 1) ||
                target.InputType == InputManagerType.MouseCommand)
                reorderableList.serializedProperty.arraySize = 1;
            SetElementHeight();
            reorderableList.DoLayoutList();
        }

        public void Dispose() {
            GUIContentDisplayName = GUIContentHeader = GUIContentMyKey = GUIContentPressType = (GUIContent)null;
            target = (InputCapsule)null;
            reorderableList = (ReorderableList)null;
            serializedObject = (InputCapsuleInspector)null;
            if (getKey != (GetKey)null)
                getKey.Close();
        }

        private void SetTitle(InputCapsuleInspector.TitleProperty title) {
            GUIContentDisplayName = title.tt_DisplayName;
            GUIContentMyKey = title.tt_MyKey;
            GUIContentPressType = title.tt_PressType;
        }

        private void SetElementHeight() {
            switch (target.InputType) {
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
            InputManagerType type = target.InputType;
            InputCapsuleTrigger[] list = reorderableList.serializedProperty.GetValue<InputCapsuleTrigger[]>();
            InputCapsuleTrigger valueInfo = list[index];

            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginDisabledGroup(true);
            //EditorGUI.PropertyField(rect, p_displayName, GUIContentDisplayName);
            _ = EditorGUI.TextField(rect, GUIContentDisplayName, valueInfo.DisplayName);
            EditorGUI.EndDisabledGroup();
            rect.y += EditorGUIUtility.singleLineHeight + 2f;
            EditorGUI.BeginChangeCheck();
            valueInfo = InputCapsuleTrigger.Editor_ModInputCapsuleTrigger(valueInfo,
                (KeyPressType)EditorGUI.EnumPopup(rect, GUIContentPressType, valueInfo.PressType)
                );
            if (EditorGUI.EndChangeCheck()) {
                list[index] = valueInfo;
                reorderableList.serializedProperty.SetValue(list);
            }
            rect.y += EditorGUIUtility.singleLineHeight + 2f;

            switch (type) {
                case InputManagerType.KeyboardCommand:
                    EditorGUI.BeginDisabledGroup(getKey != null);
                    if (GUI.Button(rect, valueInfo.MyKeyCode.ToString()))
                        getKey = GetKey.Init(valueInfo, type);
                    if (getKey != null) {
                        InputCapsuleTrigger trigger = getKey.Input;
                        if (valueInfo.MyKeyCode != trigger.MyKeyCode) {
                            list[index] = trigger;
                            reorderableList.serializedProperty.SetValue(list);
                        }
                        serializedObject.Repaint();
                    }
                    EditorGUI.EndDisabledGroup();
                    break;
                case InputManagerType.MouseCommand:
                    InputMouse mouse = (InputMouse)valueInfo.MyKeyCode;
                    EditorGUI.BeginChangeCheck();
                    mouse = (InputMouse)EditorGUI.EnumPopup(rect, GUIContentMyKey, mouse);
                    if (EditorGUI.EndChangeCheck()) {
                        valueInfo = InputCapsuleTrigger.Editor_ModInputCapsuleTrigger(valueInfo, (KeyCode)mouse);
                        valueInfo = InputCapsuleTrigger.Editor_ModInputCapsuleTrigger(valueInfo, valueInfo.MyKeyCode);
                        list[index] = valueInfo;
                        reorderableList.serializedProperty.SetValue(list);
                    }
                    break;
                case InputManagerType.MixedCommand:
                    Typetemp = (InputType)EditorGUI.EnumPopup(rect, Typetemp);

                    rect.y += EditorGUIUtility.singleLineHeight + 2f;
                    if (Typetemp == InputType.KeyboardCommand) goto case InputManagerType.KeyboardCommand;
                    else goto case InputManagerType.MouseCommand;
                    //break;
            }
        }
    }
}
