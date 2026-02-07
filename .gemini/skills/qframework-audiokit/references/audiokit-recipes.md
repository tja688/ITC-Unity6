# AudioKit Recipes

## Canonical Playback

### 1) BGM and voice

```csharp
using QFramework;
using UnityEngine;

public sealed class DemoBgmVoice : MonoBehaviour
{
    private void Start()
    {
        AudioKit.PlayMusic("HomeBg", loop: true);
        AudioKit.PlayVoice("NarrationLine01", loop: false);
    }

    private void OnDisable()
    {
        AudioKit.StopVoice();
    }
}
```

### 2) SFX with null-safe handling

```csharp
using QFramework;
using UnityEngine;

public sealed class DemoSfx : MonoBehaviour
{
    public void PlayClick()
    {
        var player = AudioKit.PlaySound("UIClick", loop: false, volume: 0.9f, pitch: 1.0f);
        if (player == null)
        {
            // Sound may be disabled or blocked by PlaySoundMode policy.
            return;
        }
    }
}
```

## Settings and UI Binding

```csharp
using QFramework;
using UnityEngine;

public sealed class DemoAudioSettingsPanel : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture() => GameRootApp.Interface;

    private void OnEnable()
    {
        AudioKit.Settings.MusicVolume.RegisterWithInitValue(OnMusicVolumeChanged)
            .UnRegisterWhenDisabled(gameObject);
    }

    public void SetSound(bool on) => AudioKit.Settings.IsSoundOn.Value = on;
    public void SetMusic(bool on) => AudioKit.Settings.IsMusicOn.Value = on;
    public void SetVoice(bool on) => AudioKit.Settings.IsVoiceOn.Value = on;
    public void SetMusicVolume(float value) => AudioKit.Settings.MusicVolume.Value = value;

    private void OnMusicVolumeChanged(float value)
    {
        // Sync slider text/visuals here.
    }
}
```

## PlaySoundMode Policy

```csharp
using QFramework;

public static class AudioRuntimePolicy
{
    public static void Apply()
    {
        AudioKit.PlaySoundMode = AudioKit.PlaySoundModes.IgnoreSameSoundInSoundFrames;
        AudioKit.SoundFrameCountForIgnoreSameSound = 8;
    }
}
```

## ResKit Loader Integration

Use existing adapter instead of rewriting loader glue:

- `Assets/QFramework/Toolkits/SupportOldQF/Scripts/AudioKitWithResKitInit.cs`

Expected pattern:

1. Set `AudioKit.Config.AudioLoaderPool` at startup.
2. Loader uses `ResLoader.Allocate()` + `LoadSync<AudioClip>` or `Add2Load<AudioClip> + LoadAsync`.
3. Loader `Unload()` recycles loader and clears clip references.

## Anti-Patterns

- Calling `new AudioPlayer()` or manually managing pooled player internals in feature code.
- Assuming `AudioKit.PlaySound(...)` always returns a player.
- Bypassing `AudioKit.Settings` with direct PlayerPrefs mutation in business logic.
- Forgetting explicit teardown (`StopVoice`, `StopMusic`, `StopAllSound`) for scene-bound behavior when required.
- Writing custom audio loaders without deterministic `Unload()` cleanup.
- Mixing audio side effects with direct model mutation that bypasses command/CQRS ownership.
