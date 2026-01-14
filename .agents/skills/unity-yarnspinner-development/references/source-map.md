# Yarn Spinner Unity source map

## Package roots
- `Library/PackageCache/dev.yarnspinner.unity@040b0f4542b5/` (installed cache)
- `Packages/dev.yarnspinner.unity/` (embedded/local copy for edits)

## Runtime core
- `Runtime/DialogueRunner/DialogueRunner.cs` : runtime orchestration, line/options/command dispatch, events, tokens.
- `Runtime/DialogueRunner/DialogueRunner.ActionRegistration.cs` : hooks for command and function registration.
- `Runtime/DialogueRunner/DialogueRunner.Utility.cs` : helpers like command parsing and coroutine waits.
- `Runtime/Views/DialoguePresenterBase.cs` : base class for line/option presenters.
- `Runtime/LineProviders/LineProviderBase.cs` : `ILineProvider` and `LineProviderBehaviour`.
- `Runtime/LineProviders/BuiltinLocalisedLineProvider.cs` : built-in localization provider.
- `Runtime/LineProviders/UnityLocalisedLineProvider.*.cs` : Unity Localization integration.
- `Runtime/Storage/VariableStorageBehaviour.cs` : variable storage contract.
- `Runtime/Storage/InMemoryVariableStorage.cs` : default storage.
- `Runtime/YarnProject/YarnProject.cs` : compiled program, node list, localization access.
- `Runtime/Localisation/Localization.cs` : built-in localization tables and assets.
- `Runtime/Commands/*.cs` : command/function attributes, dispatcher, defaults.
- `Runtime/Views/*.cs` : built-in presenters, typewriter, markup handlers, TMP shim.
- `Runtime/YarnTask/*.cs` : async abstraction used by presenters and runner.

## Editor pipeline
- `Editor/Importers/YarnImporter.cs` : imports .yarn files.
- `Editor/Importers/YarnProjectImporter.cs` : compiles to `YarnProject`.
- `Editor/Importers/YarnProjectUnityLocalizationUpdater.cs` : Unity Localization sync.
- `Editor/Editors/DialogueRunnerEditor.cs` : runner inspector.
- `Editor/Editors/YarnProjectEditor.cs` : project inspector.
- `Editor/Editors/YarnImporterEditor.cs` : yarn file inspector.
- `Editor/Editors/LocalizationEditor.cs` : built-in localization editor.
- `Editor/Editors/Commands/CommandsWindow.cs` : command/function discovery window.
- `Editor/Editors/YarnSpinnerProjectSettings*.cs` : project settings providers.

## Other locations
- `Prefabs/` : built-in UI prefabs and examples.
- `SourceGenerator/` : prebuilt source generator for command/function discovery.
