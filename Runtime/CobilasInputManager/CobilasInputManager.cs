using System;
using System.IO;
using System.Xml;
using UnityEngine;
using Cobilas.Collections;
using Cobilas.Unity.Utility;
using Cobilas.Unity.Management.Resources;
#if UNITY_EDITOR
using UnityEditor;
using Cobilas.Unity.Management.Build;
#else
using Cobilas.Unity.Management.RuntimeInitialize;
#endif

namespace Cobilas.Unity.Management.InputManager {
    //using UEResources = UnityEngine.Resources;

    public static class CobilasInputManager {

        public static bool UseSecondaryCommandKeys = false;
        private static InputCapsule[] capsules;
        private static event Action<string, ButtonPressedResult> buttonPressed;
        private static event Action<string, ButtonPressedResult> specificButtonPressedStatus;
        private static ButtonPressedResult result = new ButtonPressedResult();
        private static bool buttonPressedDesactive;

        public const string cimVersion = "1.11";

        public static int InputCapsuleCount => ArrayManipulation.ArrayLength(capsules);
        public static string InputCustomPath => CobilasPaths.Combine(Application.streamingAssetsPath, "CobilasInputManager\\InputCustom.xml");

        public enum InputManagerType {
            KeyboardCommand = 0,
            MouseCommand = 1,
            MixedCommand = 2
        }

        public enum KeyPressType {
            Press = 0,
            PressDown = 1,
            PressUp = 2,
            AnyKey = 3
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void EditorInit() {
            CobilasEditorProcessor.playModeStateChanged += (p, pm) => {
                if (p == CobilasEditorProcessor.PriorityProcessor.Low &&
                pm == PlayModeStateChange.EnteredPlayMode)
                    Init();
            };
        }
#else
        [CRIOLM_BeforeSceneLoad(1)]
#endif
        private static void Init() {

            using (ElementTag tag = GetElementTag(CobilasResources.GetTextAsset("InputConfigs"))) {
                capsules = ConvertCobilasInputManager.AssembleInputCapsuleList(tag, capsules, false);
                PopulateInputCapsuleEvent(capsules);
            }

            if(File.Exists(InputCustomPath))
                using (ElementTag tag = GetElementTag(new TextAsset(File.ReadAllText(InputCustomPath))))
                    capsules = ConvertCobilasInputManager.AssembleInputCapsuleList(tag, capsules, false);

            Application.quitting += DisposableInputCapsuleList;
        }

        public static void ResetInputCapsules() {
            //InputDefault
            using (ElementTag tag = GetElementTag(CobilasResources.GetTextAsset("InputConfigs")))
                capsules = ConvertCobilasInputManager.AssembleInputCapsuleList(tag, capsules, true);
        }

        public static InputCapsule[] GetInputCapsules()
            => capsules;

        public static InputCapsule GetInputCapsule(string inputID) {
            for (int I = 0; I < InputCapsuleCount; I++)
                if (capsules[I].InputID == inputID)
                    return capsules[I];
            return (InputCapsule)null;
        }

        public static KeyCode GetMouseButtonDown() {
            if (Input.GetKeyDown(KeyCode.Mouse0)) return KeyCode.Mouse0;
            else if (Input.GetKeyDown(KeyCode.Mouse1)) return KeyCode.Mouse1;
            else if (Input.GetKeyDown(KeyCode.Mouse2)) return KeyCode.Mouse2;
            else if (Input.GetKeyDown(KeyCode.Mouse3)) return KeyCode.Mouse3;
            else if (Input.GetKeyDown(KeyCode.Mouse4)) return KeyCode.Mouse4;
            else if (Input.GetKeyDown(KeyCode.Mouse5)) return KeyCode.Mouse5;
            else if (Input.GetKeyDown(KeyCode.Mouse6)) return KeyCode.Mouse6;
            return KeyCode.None;
        }

        public static void DesactiveButtonPressed() => buttonPressedDesactive = true;

        public static void ActiveButtonPressed() => buttonPressedDesactive = false;

        public static bool SpecificButtonPressedStatus(string inputID, KeyPressType keyType) {
            if (buttonPressedDesactive) return false;
            result.KeyType = keyType;
            result.Reset();
            if (specificButtonPressedStatus != (Action<string, ButtonPressedResult>)null)
                specificButtonPressedStatus(inputID, result);
            return result.Result;
        }

        public static bool ButtonPressed(string inputID) {
            if (buttonPressedDesactive) return false;
            result.Reset();
            if (buttonPressed != (Action<string, ButtonPressedResult>)null)
                buttonPressed(inputID, result);
            return result.Result;
        }

        public static void SaveInputCustom() {
            if (!Directory.Exists(Path.GetDirectoryName(InputCustomPath)))
                Directory.Exists(Path.GetDirectoryName(InputCustomPath));

            using (FileStream file = File.Create(InputCustomPath)) {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\r\n";
                ElementTag element;
                ConvertCobilasInputManager.AssembleInputCapsuleCustom(out element, capsules);
                using (element) {
                    using (XmlWriter writer = XmlWriter.Create(file, settings))
                        writer.WriteElementTag(element);
                }
            }
        }

        private static void PopulateInputCapsuleEvent(InputCapsule[] inputs) {
            buttonPressed = specificButtonPressedStatus = (Action<string, ButtonPressedResult>)null;
            for (int I = 0; I < ArrayManipulation.ArrayLength(capsules); I++) {
                specificButtonPressedStatus += capsules[I].SpecificButtonPressedStatus;
                buttonPressed += capsules[I].buttonPressed;
            }
        }

        private static ElementTag GetElementTag(TextAsset asset) {
            using (XmlReader reader = XmlReader.Create(new StringReader(asset.text)))
                return reader.GetElementTag();
        }

        private static void DisposableInputCapsuleList() {
            for (int I = 0; I < InputCapsuleCount; I++)
                capsules[I].Dispose();
            buttonPressed = specificButtonPressedStatus = (Action<string, ButtonPressedResult>)null;
            result = (ButtonPressedResult)null;
            ArrayManipulation.ClearArraySafe(ref capsules);
        }

        public sealed class ButtonPressedResult {
            public KeyPressType KeyType;
            private bool result;
            public bool Result => result;

            public void Reset() => result = false;
            public void Mark() => result = true;
        }

        public struct KeyStatus {
            public bool KeyPress;
            public bool KeyPressDown;
            public bool KeyPressUp;
        }
    }
}
