using System.IO;
using System.Text;
using UnityEngine;
using Cobilas.Collections;
using System.Globalization;

namespace Cobilas.Unity.Management.InputManager.ALFCIC {
    public static class CIMCICConvert {
        public static string CreateInputCapsuleCustom(InputCapsule[] capsules) {
            StringBuilder builder = new StringBuilder();
            builder.Append("#@ cim_version : 1.0\r\n\r\n");
            for (int index = 0; index < ArrayManipulation.ArrayLength(capsules); ++index) {
                if (!capsules[index].IsFixedInput && !capsules[index].IsHidden && capsules[index].IsChange) {
                    builder.Append("#[ InputCapsuleCustom\r\n");
                    builder.AppendFormat("\t#& InputID : {0}\r\n", capsules[index].InputID);
                    InputCapsuleTrigger[] triggerFirst = capsules[index].TriggerFirst;
                    InputCapsuleTrigger[] secondaryTrigger = capsules[index].SecondaryTrigger;
                    builder.Append("\t#[ TriggerFirst\r\n");
                    CIMCICConvert.ReadInputCapsuleTrigger(builder, "\t\t", triggerFirst);
                    builder.Append("\t#]\r\n");
                    builder.Append("\t#[ SecondaryTrigger\r\n");
                    CIMCICConvert.ReadInputCapsuleTrigger(builder, "\t\t", secondaryTrigger);
                    builder.Append("\t#]\r\n");
                    builder.Append("#]\r\n");
                }
            }
            return builder.ToString();
        }

        public static void LoadInputCapsuleCustom(string file_path, InputCapsule[] capsules) {
            CIMCICTag cimcicTag1 = new CIMCICTag("Root", "#[#]");
            using (StreamReader streamReader = new StreamReader(file_path)) {
                int num = 0;
                while (!streamReader.EndOfStream) {
                    ++num;
                    string line = streamReader.ReadLine().Trim();
                    if (!string.IsNullOrEmpty(line)) {
                        string cimcicType = GetCIMCICType(line);
                        string flag_name = string.Empty;
                        string flag_value = flag_name;
                        if (cimcicType != "#@") {
                            if (cimcicType != "#[") {
                                if (cimcicType != "#]") {
                                    if (cimcicType != "#&") {
                                        if (cimcicType != "#*")
                                            throw new CIMCICConvertException(string.Format("(Line:{0})Flag {1} not recognized!", num, cimcicType));
                                        cimcicType = CIMCICConvert.GetCIMCICStruct(line).Trim();
                                        cimcicTag1.Add(new CIMCICComment(cimcicType));
                                    } else {
                                        CIMCICConvert.GetCIMCICTag(line, out flag_name, out flag_value);
                                        if (flag_value == "Ragnar")
                                            throw new CIMCICConvertException(string.Format("(Line:{0})Flag [{1}] has an empty value!", num, flag_name));
                                        cimcicTag1.Add(new CIMCICContainer(flag_name, "#&", flag_value));
                                    }
                                } else cimcicTag1 = (CIMCICTag)cimcicTag1.Parent;
                            } else {
                                string name = CIMCICConvert.GetCIMCICStruct(line).Trim();
                                cimcicTag1.Add((cimcicTag1 = new CIMCICTag(name, "#[#]")));
                            }
                        } else {
                            CIMCICConvert.GetCIMCICTag(line, out flag_name, out flag_value);
                            if (flag_name != "cim_version")
                                throw new CIMCICConvertException(string.Format("(Line:{0})Invalid version flag, the version flag must contain the name \"cim_version\"", num));
                            if (flag_value == "Ragnar")
                                throw new CIMCICConvertException(string.Format("(Line:{0})Flag [{1}] has an empty value!", num, flag_name));
                            if (!float.TryParse(flag_value, NumberStyles.Float, CultureInfo.InvariantCulture, out float _))
                                throw new CIMCICConvertException(string.Format("(Line:{0})Invalid version[{1}], the version must be decimal.", num, flag_value));
                            cimcicTag1.Add(new CIMCICContainer(flag_name, "#@", flag_value));
                        }
                    }
                }
            }
            using (cimcicTag1) {
                foreach (CIMCICStream cimcicStream1 in cimcicTag1) {
                    if (cimcicStream1 is CIMCICTag cimcicTag33 &&
                        cimcicTag33.Name == "InputCapsuleCustom") {
                        InputCapsule inputCapsule = (InputCapsule)null;
                        foreach (CIMCICStream cimcicStream2 in cimcicTag33) {
                            if (cimcicStream2 is CIMCICContainer cimcicContainer6 &&
                                cimcicContainer6.Type == "#&" && cimcicContainer6.Name == "InputID") {
                                for (int index = 0; index < ArrayManipulation.ArrayLength(capsules); ++index) {
                                    if (cimcicContainer6.Value == capsules[index].InputID) {
                                        inputCapsule = capsules[index];
                                        inputCapsule.ClearEvent();
                                    }
                                }
                            }
                            if (cimcicStream2 is CIMCICTag cimcicTag34) {
                                InputCapsuleTrigger[] inputCapsuleTriggerArray = (InputCapsuleTrigger[])null;
                                switch (cimcicTag34.Name) {
                                    case "TriggerFirst":
                                        foreach (CIMCICStream cimcicStream4 in cimcicTag34) {
                                            if (cimcicStream4 is CIMCICTag cimcicTag36 && cimcicTag36.Name == "Trigger")
                                                ArrayManipulation.Add<InputCapsuleTrigger>(
                                                    new InputCapsuleTrigger(
                                                        cimcicTag36.GetValueFromValueFlag("DisplayName"), 
                                                        (KeyCode)int.Parse(cimcicTag36.GetValueFromValueFlag("KeyCode")),
                                                        (KeyPressType)int.Parse(cimcicTag36.GetValueFromValueFlag("KeyPressType"))),
                                                    ref inputCapsuleTriggerArray);
                                        }
                                        inputCapsule.SetTriggerFirst(inputCapsuleTriggerArray);
                                        break;
                                    case "SecondaryTrigger":
                                        foreach (CIMCICStream cimcicStream3 in cimcicTag34) {
                                            if (cimcicStream3 is CIMCICTag cimcicTag35 && cimcicTag35.Name == "Trigger")
                                                ArrayManipulation.Add<InputCapsuleTrigger>(
                                                    new InputCapsuleTrigger(
                                                        cimcicTag35.GetValueFromValueFlag("DisplayName"), 
                                                        (KeyCode)int.Parse(cimcicTag35.GetValueFromValueFlag("KeyCode")), 
                                                        (KeyPressType)int.Parse(cimcicTag35.GetValueFromValueFlag("KeyPressType"))),
                                                    ref inputCapsuleTriggerArray);
                                        }
                                        inputCapsule.SetSecondaryTrigger(inputCapsuleTriggerArray);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void GetCIMCICTag(string line, out string tag, out string value) {
            string cimcicStruct = GetCIMCICStruct(line);
            int startIndex = cimcicStruct.IndexOf(':');
            tag = startIndex < 0 ? cimcicStruct : cimcicStruct.Remove(startIndex).Trim();
            value = startIndex < 0 ? "Ragnar" : cimcicStruct.Remove(0, startIndex + 1).Trim();
        }

        private static string GetCIMCICStruct(string line) {
            int num = line.IndexOf(" ");
            return num < 0 ? string.Empty : line.Remove(0, num + 1);
        }

        private static string GetCIMCICType(string line) {
            int startIndex = line.IndexOf(" ");
            return startIndex < 0 ? line : line.Remove(startIndex);
        }

        private static void ReadInputCapsuleTrigger(
          StringBuilder builder,
          string spacing,
          InputCapsuleTrigger[] triggers) {
            for (int index = 0; index < ArrayManipulation.ArrayLength(triggers); ++index) {
                builder.AppendFormat("{0}#[ Trigger\r\n", spacing);
                builder.AppendFormat("{0}\t#& DisplayName : {1}\r\n", spacing, triggers[index].DisplayName);
                builder.AppendFormat("{0}\t#& KeyCode : {1}\r\n", spacing, (int)triggers[index].MyKeyCode);
                builder.AppendFormat("{0}\t#& KeyPressType : {1}\r\n", spacing, (int)triggers[index].PressType);
                builder.AppendFormat("{0}#]\r\n", spacing);
            }
        }
    }
}