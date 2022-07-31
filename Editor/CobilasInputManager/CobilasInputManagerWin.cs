using System;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Cobilas.Collections;
using Cobilas.Unity.Utility;
using Cobilas.Unity.Editor.Utility;
using Cobilas.Unity.Management.Build;
using Cobilas.Unity.Management.Resources;
using Cobilas.Unity.Management.RuntimeInitialize;
using CobilasInputManagerClass = Cobilas.Unity.Management.InputManager.CobilasInputManager;

namespace Cobilas.Unity.Editor.Management.InputManager {
    using InputManagerType = CobilasInputManagerClass.InputManagerType;
    using KeyPressType = CobilasInputManagerClass.KeyPressType;

    public class CobilasInputManagerWin : EditorWindow {
        private Vector2 scrollView;
        [SerializeField] private bool UseSecondaryCommandKeys;
        [SerializeField] private InputCapsuleInfo[] capsulesClone;

        private static string persistentInputManagerFolder => CobilasPaths.Combine(Application.persistentDataPath, "Input manager");
        private static string persistentInputManagerFile => CobilasPaths.Combine(persistentInputManagerFolder, "List.cimtemp");
        private static string InputConfigsFolderPath => CobilasPaths.Combine(CobilasPaths.ResourcesPath, "Inputs");
        private static string InputConfigsPath => CobilasPaths.Combine(InputConfigsFolderPath, "InputConfigs.xml");

        [MenuItem("Window/Cobilas/Input manager")]
        private static void DoIt() {
            CobilasInputManagerWin managerWin = GetWindow<CobilasInputManagerWin>();
            managerWin.titleContent = new GUIContent("Cobilas input manager");

            if (File.Exists(persistentInputManagerFile))
                using (FileStream file = File.OpenRead(persistentInputManagerFile)) {
                    using (XmlReader reader = XmlReader.Create(file))
                        managerWin.capsulesClone = ConvertCobilasInputManagerEditor.AssembleInputCapsuleList(reader.GetElementTag(), managerWin.capsulesClone);
                }
            managerWin.UseSecondaryCommandKeys = CobilasInputManagerClass.UseSecondaryCommandKeys;
            managerWin.Show();
        }

        [InitializeOnLoadMethod]
        private static void Init() {
            CobilasEditorProcessor.playModeStateChanged += (p, pm) => {
                if (p == CobilasEditorProcessor.PriorityProcessor.High &&
                    pm == PlayModeStateChange.EnteredPlayMode &&
                    File.Exists(persistentInputManagerFile)) {
                    LoadInputConfig();
                }
            };
            CobilasBuildProcessor.EventOnPreprocessBuild += (p, b) => {
                if (p == CobilasEditorProcessor.PriorityProcessor.High &&
                    File.Exists(persistentInputManagerFile)) {
                    LoadInputConfig();
                }
            };
        }

        [CRIOLM_CallWhen(typeof(CobilasResources), CRIOLMType.BeforeSceneLoad)]
        private static void LoadInputConfig() {
            if (!Directory.Exists(InputConfigsFolderPath))
                Directory.CreateDirectory(InputConfigsFolderPath);
            File.Copy(persistentInputManagerFile, InputConfigsPath, true);
            AssetDatabase.Refresh();
        }

        private T[] SecureCloning<T>(T[] capsulesClone)
            => ArrayManipulation.EmpytArray(capsulesClone) ? (T[])null : (T[])capsulesClone.Clone();

        private void OnDestroy() {
            ElementTag element;
            ConvertCobilasInputManagerEditor.AssembleInputCapsuleConfigs(out element, capsulesClone);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\r\n";

            if (!Directory.Exists(persistentInputManagerFolder))
                Directory.CreateDirectory(persistentInputManagerFolder);

            using (FileStream fileStream = File.Create(persistentInputManagerFile)) {
                using (XmlWriter writer = XmlWriter.Create(fileStream, settings))
                    writer.WriteElementTag(element);
            }
        }

        private void OnGUI() {
            Event eventTemp = Event.current;

            EditorGUILayout.LabelField($"cimVersion:{CobilasInputManagerClass.cimVersion}");

            using (CBEditorWindowUtility.CreateDisabledScope(EditorApplication.isCompiling)) {
                CobilasInputManagerClass.UseSecondaryCommandKeys =
                    UseSecondaryCommandKeys =
                    EditorGUILayout.ToggleLeft("Use Secondary Command Keys", UseSecondaryCommandKeys);

                if (GUILayout.Button("Add InputCapsule", GUILayout.Width(130f)))
                    ArrayManipulation.Add(CreateDefaultInputCapsule(), ref capsulesClone);

                scrollView = EditorGUILayout.BeginScrollView(scrollView);
                for (int I = 0; I < ArrayManipulation.ArrayLength(capsulesClone); I++) {
                    InputCapsuleInfo capsule = capsulesClone[I];
                    EditorGUILayout.BeginHorizontal();
                    capsule.InputCapsule_Collaps = EditorGUILayout.Foldout(capsule.InputCapsule_Collaps, $"Capsule:{capsule.inputName}");
                    if (GUILayout.Button("Remove", GUILayout.Width(100f))) {
                        ArrayManipulation.Remove(I, ref capsulesClone);
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (capsule.InputCapsule_Collaps) {
                        EditorGUI.indentLevel++;
                        capsule.isFixedInput = EditorGUILayout.Toggle("Is fixed input", capsule.isFixedInput);
                        capsule.isHidden = EditorGUILayout.Toggle("Is hidden", capsule.isHidden);
                        capsule.inputName = EditorGUILayout.TextField("Input name", capsule.inputName, GUILayout.Width(350f));
                        capsule.inputID = EditorGUILayout.TextField("Input ID", capsule.inputID, GUILayout.Width(350f));
                        capsule.inputType = (InputManagerType)EditorGUILayout.EnumPopup("Input type", capsule.inputType, GUILayout.Width(350f));

                        InputValueInfo[] inputs = SecureCloning<InputValueInfo>(capsule.inputMain);
                        if (capsule.InputMain_Collaps = EditorGUILayout.Foldout(capsule.InputMain_Collaps, "Input Main")) {
                            EditorGUI.indentLevel++;
                            for (int IM = 0; IM < ArrayManipulation.ArrayLength(inputs); IM++) {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField($"Input:{IM}");
                                if (GUILayout.Button("Remove", GUILayout.Width(100f))) {
                                    ArrayManipulation.Remove(IM, 1, ref inputs);
                                    IM -= 1;
                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUI.indentLevel++;

                                inputs[IM].ReadKey = EditorGUILayout.Toggle("Read key", inputs[IM].ReadKey);


                                if (inputs[IM].ReadKey) {
                                    KeyCode keyTemp = KeyCode.None;
                                    //bool mouseCheck = false;
                                    EditorGUI.BeginChangeCheck();
                                    if (capsule.inputType == InputManagerType.MouseCommand ||
                                        capsule.inputType == InputManagerType.MixedCommand)
                                        keyTemp = GetMouseButton();
                                    if (eventTemp.type == EventType.KeyDown || EditorGUI.EndChangeCheck()) {
                                        if (capsule.inputType == InputManagerType.KeyboardCommand) keyTemp = eventTemp.keyCode;
                                        else if (eventTemp.keyCode != KeyCode.None && capsule.inputType == InputManagerType.MixedCommand)
                                            keyTemp = eventTemp.keyCode;
                                        char charTemp = eventTemp.character;
                                        if (keyTemp != KeyCode.None) {
                                            EscapeSequence escape = charTemp.InterpretEscapeSequence();
                                            if (escape != EscapeSequence.NewLine &&
                                                escape != EscapeSequence.CarriageReturn && escape != EscapeSequence.HorizontalTab &&
                                                escape != EscapeSequence.VerticalTab) {
                                                inputs[IM].displayName = keyTemp.ToString();
                                            } else inputs[IM].displayName = charTemp.ToString();
                                            inputs[IM].myKey = keyTemp;
                                        }
                                    }
                                }

                                GUI.SetNextControlName("Labist");
                                EditorGUILayout.LabelField($"Display name:{inputs[IM].displayName}");
                                if (inputs[IM].ReadKey)
                                    GUI.FocusControl("Labist");
                                
                                inputs[IM].pressType = (KeyPressType)EditorGUILayout.EnumPopup("Press type", inputs[IM].pressType, GUILayout.Width(350f));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.indentLevel--;

                            if (Button("Add Key", GUILayout.Width(100f)))
                                ArrayManipulation.Add(CreateDefaultInputValue(), ref inputs);
                        }

                        capsule.inputMain = inputs;

                        if (CobilasInputManagerClass.UseSecondaryCommandKeys) {
                            inputs = SecureCloning<InputValueInfo>(capsule.secondaryInput);
                            if (capsule.SecondaryInput_Collaps = EditorGUILayout.Foldout(capsule.SecondaryInput_Collaps, "Secondary Input")) {
                                EditorGUI.indentLevel++;
                                for (int IS = 0; IS < ArrayManipulation.ArrayLength(inputs); IS++) {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField($"Input:{IS}");
                                    if (GUILayout.Button("Remove", GUILayout.Width(100f))) {
                                        ArrayManipulation.Remove(IS, 1, ref inputs);
                                        IS -= 1;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUI.indentLevel++;
                                    EditorGUI.BeginChangeCheck();
                                    inputs[IS].myKey = (KeyCode)EditorGUILayout.EnumPopup("My key", inputs[IS].myKey, GUILayout.Width(350f));
                                    inputs[IS].pressType = (KeyPressType)EditorGUILayout.EnumPopup("Press type", inputs[IS].pressType, GUILayout.Width(350f));
                                    EditorGUI.indentLevel--;
                                }
                                EditorGUI.indentLevel--;
                                if (Button("Add Key", GUILayout.Width(100f)))
                                    ArrayManipulation.Add(CreateDefaultInputValue(), ref inputs);
                            }

                            capsule.secondaryInput = inputs;
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUILayout.EndScrollView();

                //CobilasInputManagerClass.SetInputCapsules(capsulesClone.ToArray());
            }
        }

        private bool Button(string text, params GUILayoutOption[] options) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(4f + (4f * EditorGUI.indentLevel));
            bool Res = GUILayout.Button(text, options);
            EditorGUILayout.EndHorizontal();
            return Res;
        }

        private InputCapsuleInfo CreateDefaultInputCapsule()
            => new InputCapsuleInfo("DefaultInput", "ID_DefaultInput", InputManagerType.KeyboardCommand);

        private InputValueInfo CreateDefaultInputValue()
            => new InputValueInfo(KeyCode.None, KeyPressType.Press);

        private int indexMos;
        private string[] mouselist = new string[] {
            KeyCode.Mouse0.ToString(),
            KeyCode.Mouse1.ToString(),
            KeyCode.Mouse2.ToString(),
            KeyCode.Mouse3.ToString(),
            KeyCode.Mouse4.ToString(),
            KeyCode.Mouse5.ToString(),
            KeyCode.Mouse6.ToString()
        };

        private KeyCode GetMouseButton() {
            switch (indexMos = EditorGUILayout.Popup("Mouse button", indexMos, mouselist, GUILayout.Width(350f))) {
                case 0: return KeyCode.Mouse0;
                case 1: return KeyCode.Mouse1;
                case 2: return KeyCode.Mouse2;
                case 3: return KeyCode.Mouse3;
                case 4: return KeyCode.Mouse4;
                case 5: return KeyCode.Mouse5;
                case 6: return KeyCode.Mouse6;
                default: return KeyCode.None;
            }
        }
    }
}