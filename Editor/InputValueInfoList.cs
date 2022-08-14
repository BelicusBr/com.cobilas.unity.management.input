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
            Mouse0 = 323, // 0x00000143
            Mouse1 = 324, // 0x00000144
            Mouse2 = 325, // 0x00000145
            Mouse3 = 326, // 0x00000146
            Mouse4 = 327, // 0x00000147
            Mouse5 = 328, // 0x00000148
            Mouse6 = 329 // 0x00000149
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

        public InputValueInfoList(
          InputCapsuleInspector serializedObject,
          GUIContent Header,
          InputCapsuleInspector.TitleProperty title,
          SerializedProperty property) {
            GUIContentHeader = Header;
            this.serializedObject = serializedObject;
            reorderableList = new ReorderableList(serializedObject.serializedObject, property);
            target = serializedObject.serializedObject.targetObject as InputCapsule;
            SetTitle(title);
            SetElementHeight();
            reorderableList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(DrawHeaderCallback);
            reorderableList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(DrawElementCallback);
        }

        public void DrawList() {
            if (!CobilasInputManager.UseMultipleKeys && reorderableList.serializedProperty.arraySize > 1 || target.InputType == InputManagerType.MouseCommand)
                reorderableList.serializedProperty.arraySize = 1;
            SetElementHeight();
            reorderableList.DoLayoutList();
        }

        public void Dispose() {
            GUIContentDisplayName = GUIContentHeader = GUIContentMyKey = GUIContentPressType = (GUIContent)null;
            target = (InputCapsule)null;
            reorderableList = (ReorderableList)null;
            serializedObject = (InputCapsuleInspector)null;
            if (getKey != null)
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
            InputManagerType inputType = target.InputType;
            InputCapsuleTrigger[] inputCapsuleTriggerArray = reorderableList.serializedProperty.GetValue<InputCapsuleTrigger[]>();
            InputCapsuleTrigger trigger = inputCapsuleTriggerArray[index];
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(rect, GUIContentDisplayName, trigger.DisplayName);
            EditorGUI.EndDisabledGroup();
            rect.y += EditorGUIUtility.singleLineHeight + 2f;
            EditorGUI.BeginChangeCheck();
            InputCapsuleTrigger inputCapsuleTrigger = InputCapsuleTrigger.Editor_ModInputCapsuleTrigger(trigger, (KeyPressType)EditorGUI.EnumPopup(rect, GUIContentPressType, trigger.PressType));
            if (EditorGUI.EndChangeCheck()) {
                inputCapsuleTriggerArray[index] = inputCapsuleTrigger;
                reorderableList.serializedProperty.SetValue(inputCapsuleTriggerArray);
            }
            rect.y += EditorGUIUtility.singleLineHeight + 2f;
            switch (inputType) {
                case InputManagerType.KeyboardCommand:
                    EditorGUI.BeginDisabledGroup(getKey != null);
                    if (GUI.Button(rect, inputCapsuleTrigger.MyKeyCode.ToString()))
                        getKey = GetKey.Init(inputCapsuleTrigger, inputType);
                    if (getKey != null) {
                        InputCapsuleTrigger input = getKey.Input;
                        if (inputCapsuleTrigger.MyKeyCode != input.MyKeyCode) {
                            inputCapsuleTriggerArray[index] = input;
                            reorderableList.serializedProperty.SetValue(inputCapsuleTriggerArray);
                        }
                        serializedObject.Repaint();
                    }
                    EditorGUI.EndDisabledGroup();
                    break;
                case InputManagerType.MouseCommand:
                    InputMouse myKeyCode = (InputMouse)inputCapsuleTrigger.MyKeyCode;
                    EditorGUI.BeginChangeCheck();
                    InputMouse inputMouse = (InputMouse)EditorGUI.EnumPopup(rect, GUIContentMyKey, myKeyCode);
                    if (!EditorGUI.EndChangeCheck())
                        break;
                    inputCapsuleTrigger = InputCapsuleTrigger.Editor_ModInputCapsuleTrigger(inputCapsuleTrigger, (KeyCode)inputMouse);
                    inputCapsuleTrigger = InputCapsuleTrigger.Editor_ModInputCapsuleTrigger(inputCapsuleTrigger, inputCapsuleTrigger.MyKeyCode);
                    inputCapsuleTriggerArray[index] = inputCapsuleTrigger;
                    reorderableList.serializedProperty.SetValue(inputCapsuleTriggerArray);
                    break;
                case InputManagerType.MixedCommand:
                    Typetemp = (InputType)EditorGUI.EnumPopup(rect, Typetemp);
                    rect.y += EditorGUIUtility.singleLineHeight + 2f;
                    if (Typetemp != InputType.KeyboardCommand)
                        goto case InputManagerType.MouseCommand;
                    else
                        goto case InputManagerType.KeyboardCommand;
            }
        }
    }
}
