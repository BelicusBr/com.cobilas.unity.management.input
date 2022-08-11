using System.IO;
using System.Xml;
using UnityEngine;
using UnityEditor;
using Cobilas.Collections;
using Cobilas.Unity.Utility;
using System.Collections.Generic;
using Cobilas.Unity.Management.Build;
using Cobilas.Unity.Management.InputManager;

namespace Cobilas.Unity.Editor.Management.InputManager {
    [CreateAssetMenu(fileName = "new InputCapsuleObject", menuName = "Input Capsule Object")]
    public class InputCapsuleObject : ScriptableObject {
        [SerializeField] private InputCapsuleInfo input;
        private const string menuPath = "Tools/Cobilas Input Manager/Use secondary command keys";
        private const string menuRefreshPath = "Tools/Cobilas Input Manager/Refresh Input Manager";

        public InputCapsuleInfo Input => input;

        private static string persistentInputManagerFolder => CobilasPaths.Combine(Application.persistentDataPath, "Input manager");
        private static string persistentInputManagerFile => CobilasPaths.Combine(persistentInputManagerFolder, "List.cimtemp");
        private static string InputConfigsFolderPath => CobilasPaths.Combine(CobilasPaths.ResourcesPath, "Inputs");
        private static string InputConfigsPath => CobilasPaths.Combine(InputConfigsFolderPath, "InputConfigs.xml");

        //[MenuItem(menuRefreshPath)]
        private static void Refresh() {
            Debug.Log($"[Input Manager]Refresh input manager paths[{System.DateTime.Now}]");
            IEnumerator<InputCapsuleObject> enumerator = GetInputCapsuleObjectList();
            InputCapsuleInfo[] inputs = null;
            while (enumerator.MoveNext())
                ArrayManipulation.Add(enumerator.Current.input, ref inputs);
            if (!ArrayManipulation.EmpytArray(inputs))
                CreatePersistentInputManager(inputs);
        }

        private static void LoadPersistentInputManager() {
            if (!Directory.Exists(InputConfigsFolderPath))
                Directory.CreateDirectory(InputConfigsFolderPath);
            File.Copy(persistentInputManagerFile, InputConfigsPath, true);
            AssetDatabase.Refresh();
        }

        private static void CreatePersistentInputManager(InputCapsuleInfo[] capsulesClone) {
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

        private static IEnumerator<InputCapsuleObject> GetInputCapsuleObjectList() {
            string[] guis = AssetDatabase.FindAssets("t:InputCapsuleObject");
            for (int I = 0; I < ArrayManipulation.ArrayLength(guis); I++)
                yield return AssetDatabase.LoadAssetAtPath<InputCapsuleObject>(AssetDatabase.GUIDToAssetPath(guis[I]));
        }

        //[InitializeOnLoadMethod]
        private static void Init() {
            CobilasInputManager.UseSecondaryCommandKeys = EditorPrefs.GetBool(menuPath, false);
            EditorApplication.delayCall += () => {
                PerformAction(CobilasInputManager.UseSecondaryCommandKeys);
            };
            CobilasEditorProcessor.playModeStateChanged += (p, pm) => {
                if (p == CobilasEditorProcessor.PriorityProcessor.High &&
                    pm == PlayModeStateChange.EnteredPlayMode &&
                    File.Exists(persistentInputManagerFile)) {
                    LoadPersistentInputManager();
                }
            };
            CobilasBuildProcessor.EventOnPreprocessBuild += (p, b) => {
                if (p == CobilasEditorProcessor.PriorityProcessor.High &&
                    File.Exists(persistentInputManagerFile)) {
                    LoadPersistentInputManager();
                }
            };
        }

        //[MenuItem(menuPath)]
        private static void ToggleAction()
            => PerformAction(!CobilasInputManager.UseSecondaryCommandKeys);

        private static void PerformAction(bool enabled) {
            Menu.SetChecked(menuPath, enabled);
            EditorPrefs.SetBool(menuPath, enabled);
            CobilasInputManager.UseSecondaryCommandKeys = enabled;
        }
    }
}