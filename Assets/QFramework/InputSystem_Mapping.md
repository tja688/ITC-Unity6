# QFramework New Input System Mapping

This document outlines the **New Input System** integration for QFramework.
The legacy `UnityEngine.Input` calls have been refactored to support the New Input System (package `com.unity.inputsystem`).

## Usage
To use the New Input System with QFramework components:
1. Ensure the **Input System** package is installed and `ENABLE_INPUT_SYSTEM` is defined (Unity handles this automatically).
2. For the components listed below, you can assign an `InputActionReference` in the Inspector to customize the input.
3. If no Action is assigned, the system falls back to hardcoded defaults using the New Input System API (`Keyboard.current`, `Touchscreen.current`).

## Input Mapping Table

| Feature / System | Component Class | Legacy Input | New Input System Default | Input Action Field | Description |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Console Window** | `QFramework.ConsoleWindow` | **F1** (Desktop)<br>**Escape** (Android)<br>**4-Finger Touch** (iOS) | **F1** (Desktop)<br>**Escape** (Android)<br>**4-Finger Touch** (iOS) | `ToggleConsoleAction` | Toggles the in-game debug console. Assign a custom action (e.g. "ToggleConsole") to override or formalize this input. |
| **ResKit Debug Info** | `QFramework.ResMgr` | **F1** (Hold) | **F1** (Hold) | `ResKitDebugAction` | Displays Resource Kit runtime status (ref counts, loaded assets) while the key/action is held down. |

## Automatic Association
The refactored code attempts to be generic. However, since `F1` (Debug) and `4-Finger Touch` are not standard actions in Unity's `DefaultInputActions.cs` (which usually contains Move, Look, Fire), we cannot automatically associate them with standard actions.
QFramework exposes `public InputActionReference` fields to allow you to link these to your own Input Action Asset if desired.
