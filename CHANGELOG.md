# Changelog
## [1.0.8] 12/08/2022
### Change
- Change Editor\Cobilas.Unity.Editor.Management.Input.asmdef
- Change Runtime\Cobilas.Unity.Management.Input.asmdef
- Change Test\Runtime\cim_TDS001.cs
- Change Test\Runtime\Cobilas.Unity.Test.Management.Input.asmdef
### Move
- Move Test\Editor\GetKey.cs > Editor\GetKey.cs
- Move Test\Runtime\CobilasInputManager.cs > Runtime\CobilasInputManager.cs
- Move Test\Editor\CobilasInputManagerSettingsInspector.cs > Editor\CobilasInputManagerSettingsInspector.cs
- Move Test\Editor\InputCapsuleInspector.cs > Editor\InputCapsuleInspector.cs
- Move Test\Editor\InputValueInfoList.cs > Editor\InputValueInfoList.cs
- Move Test\Runtime\CIMCICConvert.cs > Runtime\interpreter\CIMCICConvert.cs
- Move Test\Runtime\CIMCICConvertException.cs > Runtime\interpreter\CIMCICConvertException.cs
- Move Test\Runtime\CobilasInputManagerSettings.cs > Runtime\auxiliaries\CobilasInputManagerSettings.cs
- Move Test\Runtime\GetKeyInput.cs > Runtime\auxiliaries\GetKeyInput.cs
- Move Test\Runtime\InputCapsule.cs > Runtime\auxiliaries\InputCapsule.cs
- Move Test\Runtime\InputCapsuleResult.cs > Runtime\Values\InputCapsuleResult.cs
- Move Test\Runtime\InputCapsuleTrigger.cs > Runtime\Values\InputCapsuleTrigger.cs
- Move Test\Runtime\InputCapsuleUtility.cs > Runtime\auxiliaries\InputCapsuleUtility.cs
- Move Test\Runtime\InputManagerTypeEnum.cs > Runtime\Values\InputManagerTypeEnum.cs
- Move Test\Runtime\KeyPressTypeEnum.cs > Runtime\Values\KeyPressTypeEnum.cs
### Add
- Add Runtime\interpreter\Tags\CIMCICComment.cs
- Add Runtime\interpreter\Tags\CIMCICContainer.cs
- Add Runtime\interpreter\Tags\CIMCICStream.cs
- Add Runtime\interpreter\Tags\CIMCICTag.cs
### Remove
- Remove Editor\CobilasInputManager\CobilasInputManagerWin.cs
- Remove Editor\CobilasInputManager\ConvertCobilasInputManagerEditor.cs
- Remove Editor\CobilasInputManager\GetKey.cs
- Remove Editor\CobilasInputManager\InputCapsuleInfo.cs
- Remove Editor\CobilasInputManager\InputCapsuleObject.cs
- Remove Editor\CobilasInputManager\InputCapsuleObjectInspector.cs
- Remove Editor\CobilasInputManager\InputValueInfo.cs
- Remove Editor\CobilasInputManager\InputValueInfoList.cs
- Remove Runtime\CobilasInputManager\CobilasInputManager.cs
- Remove Runtime\CobilasInputManager\ConvertCobilasInputManager.cs
- Remove Runtime\CobilasInputManager\Escopo CIM_XML.txt
- Remove Runtime\CobilasInputManager\Escopo CIM_XML2.txt
- Remove Runtime\CobilasInputManager\InputCapsule.cs
- Remove Runtime\CobilasInputManager\InputValue.cs
- Remove Test\Editor\Cobilas.Unity.Editor.Test.Management.Input.asmdef
## [1.0.8] 11/08/2022
- Change CHANGELOG.md
- Change Editor\Cobilas.Unity.Editor.Management.Input.asmdef
- Change Runtime\Cobilas.Unity.Management.Input.asmdef
- Change Editor\CobilasInputManager\InputCapsuleObject.cs
- Add Test\Editor\Cobilas.Unity.Editor.Test.Management.Input.asmdef
- Add Test\Editor\CobilasInputManagerSettingsInspector.cs
- Add Test\Editor\GetKey.cs
- Add Test\Editor\InputCapsuleInspector.cs
- Add Test\Editor\InputValueInfoList.cs
- Add Test\Runtime\cim_TDS001.cs
- Add Test\Runtime\CIMCICConvert.cs
- Add Test\Runtime\CIMCICConvertException.cs
- Add Test\Runtime\Cobilas.Unity.Test.Management.Input.asmdef
- Add Test\Runtime\CobilasInputManager.cs
- Add Test\Runtime\CobilasInputManagerSettings.cs
- Add Test\Runtime\GetKeyInput.cs
- Add Test\Runtime\InputCapsule.cs
- Add Test\Runtime\InputCapsuleResult.cs
- Add Test\Runtime\InputCapsuleTrigger.cs
- Add Test\Runtime\InputCapsuleUtility.cs
- Add Test\Runtime\InputManagerTypeEnum.cs
- Add Test\Runtime\KeyPressTypeEnum.cs
## [1.0.8] 09/08/2022
- Change CHANGELOG.md
- Change Editor\CobilasInputManager\CobilasInputManagerWin.cs
- Add Editor\CobilasInputManager\GetKey.cs
- Add Editor\CobilasInputManager\InputCapsuleObject.cs
- Add Editor\CobilasInputManager\InputCapsuleObjectInspector.cs
- Add Editor\CobilasInputManager\InputValueInfoList.cs
- Fix Runtime\CobilasInputManager\ConvertCobilasInputManager.cs
- Fix Editor\CobilasInputManager\ConvertCobilasInputManagerEditor.cs
## [1.0.8] 07/08/2022
- Fix CHANGELOG.md
- Fix package.json
- Fix ConvertCobilasInputManagerEditor.cs
## [1.0.7] 31/07/2022
- Add CHANGELOG.md
- Fix package.json
- Add Cobilas MG Input.asset
- Remove Runtime\DependencyWarning.cs
- Remove Editor\DependencyWarning.cs
## [1.0.6] 31/07/2022
- Add CHANGELOG.md
- Fix package.json
- Change CobilasInputManagerWin.cs
## [1.0.5] 25/07/2022
- Add CHANGELOG.md
- Fix package.json
- Fix CobilasInputManagerWin.cs
- > Á propriedade `private static string InputConfigsFolderPath { get; }` foi adicionado.
- > No método `private static void Init();` foi adicionado uma verificação para determinar se o diretório "Resources/Inputs" existe, caso não será cirado.
## [1.0.4] 23/07/2022
- Add CHANGELOG.md
- Fix package.json
## [1.0.3] 22/07/2022
- Fix LICENSE.md
- Add Cobilas.Unity.Editor.Management.Input.asmdef
- Add Cobilas.Unity.Management.Input.asmdef
- Add Editor/DependencyWarning.cs
- Add Runtime/DependencyWarning.cs
## [1.0.2] 17/07/2022
- Fix package.json
- Delete main.yml
- Delete README.md
## [1.0.0] 15/07/2022
- Add main.yml
- Add package.json
- Add LICENSE.md
- Add folder:Editor
- Add folder:Runtime
## [0.0.1] 15/07/2022
### Repositorio com.cobilas.unity.management.input iniciado
- Lançado para o GitHub