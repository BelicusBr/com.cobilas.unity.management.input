using System;
using System.IO;
using System.Text;
using UnityEditor;
using System.Collections;
using Cobilas.Collections;
using Cobilas.Unity.Utility;
using System.Collections.Generic;
using Cobilas.Unity.Management.Runtime;
using Cobilas.Unity.Management.Resources;
using Cobilas.Unity.Management.InputManager.ALFCIC;
#if UNITY_EDITOR
using Cobilas.Unity.Management.Build;
#endif

namespace Cobilas.Unity.Management.InputManager {
    public static class CobilasInputManager {
        private static InputCapsule[] inputCapsules;
        private static bool useMultipleKeys = false;
        private static bool buttonPressedDesactive = false;
        private static bool useSecondaryCommandKeys = false;
        private static readonly InputCapsuleResult Result = new InputCapsuleResult();
        private static Action<InputCapsuleResult, KeyPressType> specificButtonPressed;
#if UNITY_EDITOR
        private const string menuRefreshSettings = "Tools/Cobilas Input Manager/Refresh settings";
        private const string menuUseMultipleKeys = "Tools/Cobilas Input Manager/Use multiple keys";
        private const string menuUseSecondaryCommandKeys = "Tools/Cobilas Input Manager/Use secondary command keys";
#endif
        public static bool UseMultipleKeys => useMultipleKeys;
        public static bool UseSecondaryCommandKeys => useSecondaryCommandKeys;
        public static int InputCapsuleCount => ArrayManipulation.ArrayLength(inputCapsules);
        public static IEnumerable<InputCapsule> InputCapsules => new InputCapsuleList(inputCapsules);
        public static string CustomInputCapsuleFile => UnityPath.Combine(CustomInputCapsuleFolder, "CustomInputCapsule.cimcic");
        public static string CustomInputCapsuleFolder => UnityPath.Combine(UnityPath.StreamingAssetsPath, "CIM_CustomInputCapsule");

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void InitEditor() {
            CobilasBuildProcessor.EventOnPreprocessBuild += (pp, br) => RefreshSettings();
            
            useSecondaryCommandKeys = EditorPrefs.GetBool(menuUseSecondaryCommandKeys, false);
            useMultipleKeys = EditorPrefs.GetBool(menuUseMultipleKeys, false);
            
            EditorApplication.delayCall += () => {
                ActionUseSecondaryCommandKeys(useSecondaryCommandKeys);
                ActionUseUseMultipleKeys(useMultipleKeys);
            };

            if (!EditorApplication.isPlaying) return;
            RefreshSettings();
            Init();
        }

        [MenuItem(menuRefreshSettings)]
        //[CRIOLM_CallWhen(typeof(CobilasResources), CRIOLMType.BeforeSceneLoad)]
        [CallWhenStart(InitializePriority.High, "#ResourceManager")]
        private static void RefreshSettings() {
            CobilasInputManagerSettings scriptableObject = ResourceManager.GetScriptableObject<CobilasInputManagerSettings>("cim_settings");
            if (scriptableObject == null) {
                CobilasInputManagerSettings inputManagerSettings = CobilasInputManagerSettings.GetCobilasInputManagerSettings();
                string dirPath = UnityPath.Combine(UnityPath.AssetsPath, "Resources/Inputs");

                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                AssetDatabase.CreateAsset(inputManagerSettings, string.Format("Assets/Resources/Inputs/{0}.asset", inputManagerSettings.name));
            } else scriptableObject.SetSettings();
            if (EditorApplication.isPlaying) return;
            AssetDatabase.Refresh();
        }

        [MenuItem(menuUseMultipleKeys)]
        private static void ToggleActionUseMultipleKeys() 
            => ActionUseUseMultipleKeys(!UseMultipleKeys);

        private static void ActionUseUseMultipleKeys(bool enabled) {
            Menu.SetChecked(menuUseMultipleKeys, enabled);
            EditorPrefs.SetBool(menuUseMultipleKeys, enabled);
            useMultipleKeys = enabled;
            if (!EditorApplication.isPlaying) return;
            ResetInputs();
            LoadInputCapsuleCustom();
        }

        [MenuItem(menuUseSecondaryCommandKeys)]
        private static void ToggleActionUseSecondaryCommandKeys() 
            => ActionUseSecondaryCommandKeys(!UseSecondaryCommandKeys);

        private static void ActionUseSecondaryCommandKeys(bool enabled) {
            Menu.SetChecked(menuUseSecondaryCommandKeys, enabled);
            EditorPrefs.SetBool(menuUseSecondaryCommandKeys, enabled);
            useSecondaryCommandKeys = enabled;
            if (!EditorApplication.isPlaying) return;
            ResetInputs();
            LoadInputCapsuleCustom();
        }
        //[CRIOLM_CallWhen(typeof(CobilasResources), CRIOLMType.AfterSceneLoad)]
        [CallWhenStart(InitializePriority.Low, "#ResourceManager")]
#else
        //[CRIOLM_BeforeSceneLoad]
        [StartBeforeSceneLoad(InitializePriority.Low)]
#endif
        private static void Init() {
            CobilasInputManagerSettings scriptableObject = ResourceManager.GetScriptableObject<CobilasInputManagerSettings>("cim_settings");
            useMultipleKeys = scriptableObject.UseMultipleKeys;
            useSecondaryCommandKeys = scriptableObject.UseSecondaryCommandKeys;
            ResetInputs();
            LoadInputCapsuleCustom();
        }

        private static void LoadInputCapsuleCustom() {
            if (File.Exists(CustomInputCapsuleFile))
                CIMCICConvert.LoadInputCapsuleCustom(CustomInputCapsuleFile, inputCapsules);
        }

        public static void ResetInputs() {
            ArrayManipulation.ClearArraySafe<InputCapsule>(ref inputCapsules);
            inputCapsules = ResourceManager.GetAllSpecificObjectInFolder<InputCapsule>("Resources/Inputs");
            InputCapsuleCollection[] collection = ResourceManager.GetAllSpecificObjectInFolder<InputCapsuleCollection>("Resources/Inputs");

            for (int I = 0; I < ArrayManipulation.ArrayLength(collection); I++)
                ArrayManipulation.Add(collection[I].Capsules, ref inputCapsules);

            for (int index = 0; index < InputCapsuleCount; ++index) {
                inputCapsules[index] = InputCapsule.CloneInputCapsule(inputCapsules[index]);
                inputCapsules[index].ClearEvent();
                if (!useMultipleKeys) {
                    if (inputCapsules[index].TriggerFirstCount > 0)
                        inputCapsules[index].SetTriggerFirst(new InputCapsuleTrigger[] { inputCapsules[index].TriggerFirst[inputCapsules[index].TriggerFirstCount - 1] });
                    if (!useSecondaryCommandKeys) {
                        inputCapsules[index].SetSecondaryTrigger((InputCapsuleTrigger[])null);
                    } else {
                        if (inputCapsules[index].SecondaryTriggerCount > 0)
                            inputCapsules[index].SetSecondaryTrigger(new InputCapsuleTrigger[] { inputCapsules[index].SecondaryTrigger[inputCapsules[index].SecondaryTriggerCount - 1] });
                    }
                } else {
                    inputCapsules[index].SetTriggerFirst(InputCapsule.CloneList(inputCapsules[index].TriggerFirst));
                    if (!useSecondaryCommandKeys) {
                        inputCapsules[index].SetSecondaryTrigger((InputCapsuleTrigger[])null);
                    } else {
                        if (inputCapsules[index].SecondaryTriggerCount > 0)
                            inputCapsules[index].SetSecondaryTrigger(InputCapsule.CloneList(inputCapsules[index].SecondaryTrigger));
                    }
                }
            }

            specificButtonPressed = (Action<InputCapsuleResult, KeyPressType>)null;
            PopEvent(inputCapsules);
        }

        public static void BeginDisabledGroup(bool disabled) 
            => buttonPressedDesactive = disabled;

        public static void EndDisabledGroup() 
            => buttonPressedDesactive = false;

        public static bool ButtonPressed(string InputID) 
            => Internal_ButtonPressed(InputID, KeyPressType.Press);

        public static bool ButtonPressedDown(string InputID) 
            => Internal_ButtonPressed(InputID, KeyPressType.PressDown);

        public static bool ButtonPressedUp(string InputID) 
            => Internal_ButtonPressed(InputID, KeyPressType.PressUp);

        public static void CreateCustomInputCapsule() {
            if (!Directory.Exists(CustomInputCapsuleFolder))
                Directory.CreateDirectory(CustomInputCapsuleFolder);

            using (FileStream fileStream = File.Create(CustomInputCapsuleFile))
                fileStream.Write(CIMCICConvert.CreateInputCapsuleCustom(inputCapsules), Encoding.UTF8);
            
            File.SetAttributes(CustomInputCapsuleFile, File.GetAttributes(CustomInputCapsuleFile) | FileAttributes.ReadOnly);
        }

        public static InputCapsule GetInputCapsule(string InputID) {
            foreach (InputCapsule inputCapsule in InputCapsules)
                if (inputCapsule.InputID == InputID)
                    return inputCapsule;
            return (InputCapsule)null;
        }

        private static bool Internal_ButtonPressed(string InputID, KeyPressType type) {
            if (buttonPressedDesactive)
                return false;
            using (Result) {
                    specificButtonPressed?.Invoke(Result.SetID(InputID), type);
                return Result.Result;
            }
        }

        private static void PopEvent(InputCapsule[] capsules) {
            for (int index = 0; index < ArrayManipulation.ArrayLength(capsules); ++index)
                specificButtonPressed += capsules[index].SpecificButtonPressed;
        }

        public sealed class InputCapsuleList : IEnumerable<InputCapsule> {
            private readonly InputCapsule[] inputCapsulesList;

            internal InputCapsuleList(InputCapsule[] inputCapsules) 
                => inputCapsulesList = inputCapsules;

            public IEnumerator<InputCapsule> GetEnumerator() 
                => new ArrayToIEnumerator<InputCapsule>(inputCapsulesList);

            IEnumerator IEnumerable.GetEnumerator() 
                => new ArrayToIEnumerator<InputCapsule>(inputCapsulesList);
        }
    }
}
