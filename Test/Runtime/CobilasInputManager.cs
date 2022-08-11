using System;
using System.IO;
using System.Text;
using System.Collections;
using Cobilas.Collections;
using Cobilas.Unity.Utility;
using System.Collections.Generic;
using Cobilas.Unity.Management.Build;
using Cobilas.Unity.Management.Resources;
using Cobilas.Unity.Management.RuntimeInitialize;
using Cobilas.Unity.Management.InputManager.ALFCIC;

namespace Cobilas.Unity.Management.InputManager {
    public static class CobilasInputManager {
        private static InputCapsule[] inputCapsules;
        private static bool useMultipleKeys = false;
        private static bool buttonPressedDesactive = false;
        private static bool useSecondaryCommandKeys = false;
        private static InputCapsuleResult Result = new InputCapsuleResult();
        private static Action<InputCapsuleResult, KeyPressType> specificButtonPressed;

        public static bool UseMultipleKeys => useMultipleKeys;
        public static bool UseSecondaryCommandKeys => useSecondaryCommandKeys;
        public static int InputCapsuleCount => ArrayManipulation.ArrayLength(inputCapsules);
        public static IEnumerable<InputCapsule> InputCapsules => new InputCapsuleList(inputCapsules);
        public static string CustomInputCapsuleFolder => CobilasPaths.Combine(CobilasPaths.StreamingAssetsPath, "CIM_CustomInputCapsule");
        public static string CustomInputCapsuleFile => CobilasPaths.Combine(CustomInputCapsuleFolder, "CustomInputCapsule.cimcic");

#if UNITY_EDITOR
        private const string menuRefreshSettings = "Tools/Cobilas Input Manager/Refresh settings";
        private const string menuUseMultipleKeys = "Tools/Cobilas Input Manager/Use multiple keys";
        private const string menuUseSecondaryCommandKeys = "Tools/Cobilas Input Manager/Use secondary command keys";

        [UnityEditor.InitializeOnLoadMethod]
        private static void InitEditor() {
            CobilasBuildProcessor.EventOnPreprocessBuild += (pp, br) => {
                RefreshSettings();
            };
            CobilasEditorProcessor.playModeStateChanged += (pp, pm) => {
                if (pp == CobilasEditorProcessor.PriorityProcessor.Low &&
                pm == UnityEditor.PlayModeStateChange.EnteredPlayMode) {
                    RefreshSettings();
                    Init();
                }
            };
            useSecondaryCommandKeys = UnityEditor.EditorPrefs.GetBool(menuUseSecondaryCommandKeys, false);
            useMultipleKeys = UnityEditor.EditorPrefs.GetBool(menuUseMultipleKeys, false);
            UnityEditor.EditorApplication.delayCall += () => {
                ActionUseSecondaryCommandKeys(useSecondaryCommandKeys);
                ActionUseUseMultipleKeys(useMultipleKeys);
            };
        }

        [UnityEditor.MenuItem(menuRefreshSettings)]
        private static void RefreshSettings() {
            CobilasInputManagerSettings settings = CobilasResources.GetScriptableObject<CobilasInputManagerSettings>("cim_settings");
            if (settings == null) {
                settings = CobilasInputManagerSettings.GetCobilasInputManagerSettings();
                UnityEditor.AssetDatabase.CreateAsset(settings, $"Assets/Resources/Inputs/{settings.name}.asset");
            } else settings.SetSettings();
            UnityEditor.AssetDatabase.Refresh();
        }

        [UnityEditor.MenuItem(menuUseMultipleKeys)]
        private static void ToggleActionUseMultipleKeys()
            => ActionUseUseMultipleKeys(!UseMultipleKeys);

        private static void ActionUseUseMultipleKeys(bool enabled) {
            UnityEditor.Menu.SetChecked(menuUseMultipleKeys, enabled);
            UnityEditor.EditorPrefs.SetBool(menuUseMultipleKeys, enabled);
            useMultipleKeys = enabled;
        }

        [UnityEditor.MenuItem(menuUseSecondaryCommandKeys)]
        private static void ToggleActionUseSecondaryCommandKeys()
            => ActionUseSecondaryCommandKeys(!UseSecondaryCommandKeys);

        private static void ActionUseSecondaryCommandKeys(bool enabled) {
            UnityEditor.Menu.SetChecked(menuUseSecondaryCommandKeys, enabled);
            UnityEditor.EditorPrefs.SetBool(menuUseSecondaryCommandKeys, enabled);
            useSecondaryCommandKeys = enabled;
        }
#else
        [CRIOLM_BeforeSceneLoad]
#endif
        private static void Init() {
            CobilasInputManagerSettings settings = CobilasResources.GetScriptableObject<CobilasInputManagerSettings>("cim_settings");
            useMultipleKeys = settings.UseMultipleKeys;
            useSecondaryCommandKeys = settings.UseSecondaryCommandKeys;
            ResetInputs();
        }

        public static void ResetInputs() {
            ArrayManipulation.ClearArraySafe(ref inputCapsules);
            inputCapsules = CobilasResources.GetAllSpecificObjectInFolder<InputCapsule>("Resources/Inputs");
            for (int I = 0; I < InputCapsuleCount; I++)
                inputCapsules[I] = InputCapsule.CloneInputCapsule(inputCapsules[I]);
            specificButtonPressed = (Action<InputCapsuleResult, KeyPressType>)null;
            PopEvent(inputCapsules);
        }

        public static void BeginDisabledGroup(bool disabled) => buttonPressedDesactive = disabled;

        public static void EndDisabledGroup() => buttonPressedDesactive = false;

        public static bool ButtonPressed(string InputID)
            => Internal_ButtonPressed(InputID, KeyPressType.Press);

        public static bool ButtonPressedDown(string InputID)
            => Internal_ButtonPressed(InputID, KeyPressType.PressDown);

        public static bool ButtonPressedUp(string InputID)
            => Internal_ButtonPressed(InputID, KeyPressType.PressUp);

        public static void CreateCustomInputCapsule() {
            if (!Directory.Exists(CustomInputCapsuleFolder))
                Directory.CreateDirectory(CustomInputCapsuleFolder);

            using (FileStream file = File.Create(CustomInputCapsuleFile))
                file.Write(CIMCICConvert.CreateInputCapsuleCustom(inputCapsules), Encoding.UTF8);
        }

        public static InputCapsule GetInputCapsule(string InputID) {
            foreach (var item in InputCapsules)
                if (item.InputID == InputID)
                    return item;
            return (InputCapsule)null;
        }

        private static bool Internal_ButtonPressed(string InputID, KeyPressType type) {
            if (buttonPressedDesactive) return false;
            using (Result) {
                specificButtonPressed?.Invoke(Result.SetID(InputID), type);
                return Result.Result;
            }
        }

        private static void PopEvent(InputCapsule[] capsules) {
            for (int I = 0; I < ArrayManipulation.ArrayLength(capsules); I++)
                specificButtonPressed += capsules[I].SpecificButtonPressed;
        }

        public sealed class InputCapsuleList : IEnumerable<InputCapsule> {

            private InputCapsule[] inputCapsulesList;

            public InputCapsuleList(InputCapsule[] inputCapsules)
                => this.inputCapsulesList = inputCapsules;

            public IEnumerator<InputCapsule> GetEnumerator()
                => new ArrayToIEnumerator<InputCapsule>(inputCapsulesList);

            IEnumerator IEnumerable.GetEnumerator()
                => new ArrayToIEnumerator<InputCapsule>(inputCapsulesList);
        }
    }
}
