# Febucci Text Animator - Local References

## Project Paths

- Docs snapshot: `Assets/Plugins/Febucci/docs.febucci.com.md` or `skills/febucci-text-animator/references/docs.febucci.com.md` 
- Plugin root: `Assets/Plugins/Febucci/Text Animator for Unity`
- Samples: `Assets/Plugins/Febucci/Text Animator for Unity/Samples`
- Runtime scripts: `Assets/Plugins/Febucci/Text Animator for Unity/Scripts/Runtime`

## Useful Ripgrep Patterns

Use these against `Assets/Plugins/Febucci/docs.febucci.com.md`:

- `rg -n "quick start|install" ...`
- `rg -n "how to add effects|effects|tags|modifiers" ...`
- `rg -n "typewriter|show text|hide text" ...`
- `rg -n "settings|global settings|default tags" ...`
- `rg -n "custom effects|custom classes|scriptable" ...`

Use these against the runtime code:

- `rg -n "TextAnimator_TMP|AnimatedLabel|TypewriterComponent" Assets/Plugins/Febucci/Text Animator for Unity/Scripts/Runtime`
- `rg -n "SettingsScriptable|AnimationsDatabase|ActionDatabase" Assets/Plugins/Febucci/Text Animator for Unity/Scripts/Runtime`
