# Changelog
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
## [1.5.0] (05/02/2024)
- ### Changed
- - Updated dependency `com.cobilas.unity.utility` to version `2.10.3`.
- - Updated dependency `com.cobilas.unity.core.net4x` to version `1.4.1`.
- - Updated dependency `com.cobilas.unity.management.runtime` to version `2.2.1`.
- - Updated dependency `com.cobilas.unity.management.resource` to version `2.1.1`.
- - This update includes bug fixes and new features that do not directly impact this package.
## [1.3.0] - 29/08/2023
### Changed
- As dependencias do pacote foram aluteradas.
## [1.2.0-ch1] - 28/08/2023
### Changed
- O autor do pacote foi alterado de `Cobilas CTB` para `BélicusBr`.
## [1.1.13] - 30/01/2023
### Changed
- Romoção de campos não utilizados.
- Remoção de atribuições desnecessárias.
- Transformando possiveis campos em `readonly`.
### Fixed
- Antes de criar o objeto `CobilasInputManagerSettings` é verificádo se o diretório 'Assets/Resources/Inputs' existe.
## [1.1.12] 17/11/2022
#### Change
O `CobilasInputManager` está usando o novo `StartMethodOnRun` para inicialização.
## [1.1.10] 19/08/2022
#### (Add) Pre-Definições
Agora `InputCapsuleCollection` possui pre-definições.
#### (Fix) InputCapsuleCollectionInspector.cs
Avia o problema de persistência das lista de entrada dos elementos InputCapsule<br/>
que fazia os valores não serem adicionados na lista pelo fato de que a class `InputCapsuleInspectorDrawer`<br/>
era instanciado e depóis destruído dentro do metódo `InputCapsuleCollectionInspector.OnInspectorGUI()`,<br/>
o que fazia com que a instâcia da classe `InputValueInfoList` criace um novo id e por consequência<br/>
a instâcia de `GetKey` não funcinasse corretamente.
## [1.1.9] 17/08/2022
#### (Add) InputCapsuleCollection.cs
Em vez de criar um unico objeto InputCapsule para criar uma entrada de teclado<br/>
agora se pode criar uma coleção de InputCapsules por meio do `InputCapsuleCollection`.
#### (Change) Runtime\auxiliaries\InputCapsule.cs
Ágora com um objeto InputCapsule que possua multiplas teclas de entrado a última<br/>
entrada será verificada nos metódos
```c#
	//class CobilasInputManager
	bool ButtonPressed(string InputID);
	bool ButtonPressedDown(string InputID);
	bool ButtonPressedUp(string InputID);
```
enquanto as outras entradas seram verificadas como `KeyPressType.AnyPress`, apesar<br/>
que o recomendado e usar `KeyPressType.Press`.
#### (Fix) Lista primaria é secundaria recebendo o mesmo valor
Avia o problema de que as duas lista de entrada receberem o mesmo valor por<br/>
falta uma forma de identificação da duas lista.<br/>
Isso foi resolvido criando dois parâmetros no metódo `GetKey:GetKey.Init(InputCapsuleTrigger, int, string, InputManagerType);`,<br/>
os parâmetros `int indexTarget, string guiTarget`.<br/>
O `indexTarget` é o indice do objeto `InputCapsuleTrigger` na lista.<br/>
O `guiTarget` é o id gerado por meio do `System.Guid.NewGuid()` da classe `InputValueInfoList` que é<br/>
gerada á cada nova instancia do objeto.
## [1.0.8] 13/08/2022
- Change Runtime\auxiliaries\InputCapsule.cs
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