using System.IO;
using System.Text;
using UnityEngine;
using Cobilas.Collections;

#warning testar o método LoadInputCapsuleCustom
#warning colocar as exeções nos devidos lugares
namespace Cobilas.Unity.Management.InputManager.ALFCIC {
    public static class CIMCICConvert {
        public static string CreateInputCapsuleCustom(InputCapsule[] capsules) {
            StringBuilder builder = new StringBuilder();
            builder.Append("#@ cim_version : 1.0\r\n\r\n");
            for (int I = 0; I < ArrayManipulation.ArrayLength(capsules); I++) {
                if (capsules[I].IsFixedInput || capsules[I].IsHidden || !capsules[I].IsChange)
                    continue;
                builder.Append("#[ InputCapsuleCustom\r\n");
                builder.AppendFormat("\t#& InputID : {0}\r\n", capsules[I].InputID);

                InputCapsuleTrigger[] triggerFirst = capsules[I].TriggerFirst;
                InputCapsuleTrigger[] secondaryTrigger = capsules[I].SecondaryTrigger;

                builder.Append("\t#[ TriggerFirst\r\n");
                ReadInputCapsuleTrigger(builder, "\t\t", triggerFirst);
                builder.Append("\t#]\r\n");
                builder.Append("\t#[ SecondaryTrigger\r\n");
                ReadInputCapsuleTrigger(builder, "\t\t", secondaryTrigger);
                builder.Append("\t#]\r\n");
                builder.Append("#]\r\n");
            }
            return builder.ToString();
        }

        [UnityEditor.MenuItem("Tools/Cobilas Input Manager/LoadInputCapsuleCustom")]
        public static void LoadInputCapsuleCustom(InputCapsule[] capsules) {
            CICItem[] cICItens = null;
            CICItem temp = null;
            byte tagType = 0;
            using (StreamReader reader = new StreamReader(CobilasInputManager.CustomInputCapsuleFile)) {
                KeyCode keyCode = KeyCode.None;
                string displayName = string.Empty;
                KeyPressType keyPressType = KeyPressType.Press;
                while (!reader.EndOfStream) {
                    string line = reader.ReadLine().Trim();
                    if (!string.IsNullOrEmpty(line)) {
                        string cm = GetCIMCICType(line);
                        string str = string.Empty;
                        switch (cm) {
                            case "#@":
                                if (tagType != 0) Debug.Log("Erro!!");
                                Debug.Log(GetCIMCICStruct(line));
                                break;
                            case "#[":
                                switch (str = GetCIMCICStruct(line)) {
                                    case "InputCapsuleCustom":
                                        if (tagType != 0) Debug.Log("Erro!!");
                                        tagType = 1;
                                        temp = new CICItem();
                                        break;
                                    case "TriggerFirst":
                                        if (tagType != 1) Debug.Log("Erro!!");
                                        tagType = 2;
                                        break;
                                    case "SecondaryTrigger":
                                        if (tagType != 1) Debug.Log("Erro!!");
                                        tagType = 3;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "#]":
                                if (tagType == 0) Debug.Log("Erro!!");
                                if (tagType == 1) {
                                    tagType = 0;
                                    ArrayManipulation.Add(temp, ref cICItens);
                                }
                                if (tagType == 2) {
                                    tagType = 1;
                                    ArrayManipulation.Add(new InputCapsuleTrigger(displayName, keyCode, keyPressType), ref temp.triggers1);
                                }
                                if (tagType == 3) {
                                    tagType = 1;
                                    ArrayManipulation.Add(new InputCapsuleTrigger(displayName, keyCode, keyPressType), ref temp.triggers2);
                                }
                                break;
                            case "#&":
                                string tag;
                                string value;
                                switch (tagType) {
                                    case 1:
                                        GetCIMCICTag(line, out tag, out value);
                                        if (tag == "InputID") temp.InputID = value;
                                        else Debug.Log("Erro!!");
                                        break;
                                    case 2:
                                    case 3:
                                        GetCIMCICTag(line, out tag, out value);
                                        if (tag == "DisplayName") displayName = value;
                                        else if (tag == "KeyCode") keyCode = (KeyCode)int.Parse(value);
                                        else if (tag == "KeyPressType") keyPressType = (KeyPressType)int.Parse(value);
                                        else Debug.Log("Erro!!");
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "#*":
                                break;
                            default:
                                Debug.Log("Erro!!");
                                break;
                        }
                    }
                }
            }
            for (int I = 0; I < ArrayManipulation.ArrayLength(cICItens); I++)
                for (int J = 0; J < ArrayManipulation.ArrayLength(capsules); J++)
                    if (cICItens[I].InputID == capsules[J].InputID) {
                        capsules[J].ClearEvent();
                        capsules[J].SetTriggerFirst(cICItens[I].triggers1);
                        capsules[J].SetSecondaryTrigger(cICItens[I].triggers2);
                    }
        }

        private static void GetCIMCICTag(string line, out string tag, out string value) {
            string str = GetCIMCICStruct(line);
            int sep = str.IndexOf(':');
            if (sep < 0) {
                tag = str;
                value = "Ragnar";
            } else {
                tag = str.Remove(sep);
                value = str.Remove(0, sep + 1);
            }
        }

        private static string GetCIMCICStruct(string line) {
            int index = line.IndexOf(" ");
            if (index < 0) return string.Empty;
            return line.Remove(0, index + 1);
        }

        private static string GetCIMCICType(string line) {
            int index = line.IndexOf(" ");
            if (index < 0) return line;
            return line.Remove(index);
        }

        private static void ReadInputCapsuleTrigger(StringBuilder builder, string spacing, InputCapsuleTrigger[] triggers) {
            for (int J = 0; J < ArrayManipulation.ArrayLength(triggers); J++) {
                builder.AppendFormat("{0}#[ Trigger\r\n", spacing);
                builder.AppendFormat("{0}\t#& DisplayName : {1}\r\n", spacing, triggers[J].DisplayName);
                builder.AppendFormat("{0}\t#& KeyCode : {1}\r\n", spacing, (int)triggers[J].MyKeyCode);
                builder.AppendFormat("{0}\t#& KeyPressType : {1}\r\n", spacing, (int)triggers[J].PressType);
                builder.AppendFormat("{0}#]\r\n", spacing);
            }
        }

        private sealed class CICItem {
            public string InputID;
            public InputCapsuleTrigger[] triggers1;
            public InputCapsuleTrigger[] triggers2;
        }
    }
}
