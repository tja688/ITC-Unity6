# 📄 Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity

**Welcome** to the documentation of **Text Animator for Unity 3.X**! We can't wait to have you animate your texts and get familiar with the plugin.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F74b3Q86Y180EtGnu7Jg5%252FGames%2520Using%2520Text%2520Animator.png%3Falt%3Dmedia%26token%3D9912a72f-fab2-4003-b8f7-3499fc676a33&width=768&dpr=4&quality=100&sign=91cad09e&sv=2)

We're writing a documentation that's as short and concise as possible, but that is also able to help you when you need it the most. **Please make sure to read the first and most important pages**! They take a few minutes now but will save _a lot of time_ later.

Useful links:

[Purchase](https://www.textanimatorforgames.com/unity#pricing)
 [Website](https://www.textanimatorforgames.com/unity)

#### 

[](https://docs.febucci.com/text-animator-unity#good-to-know)

Good to know

*   This documentation is available in different languages: English, Chinese, Korean, Japanese.
    
*   You can browse different versions and languages of this documentation at the top of this page.
    
*   Text Animator is also available in other engines. [Learn more here](https://www.textanimatorforgames.com/)
    .
    

And if you need any help at any time, feel free to visit the [troubleshooting page](https://docs.febucci.com/text-animator-unity/other/troubleshooting)
 (common issues and how to fix them) or the support page!

[![Logo](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fframerusercontent.com%2Fimages%2Fy1LCEnd5hyGjuX0kKaGBUorzMc.png&width=20&dpr=4&quality=100&sign=82d1be32&sv=2)Support Requests | Text Animator for Unity, Godot and Unrealwww.textanimatorforgames.com](https://www.textanimatorforgames.com/support)

#### 

[](https://docs.febucci.com/text-animator-unity#requirements)

Requirements

Please visit the [Requirements & Limitations](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations)
 page before purchasing or importing, and we also replied many [frequently asked questions here](https://docs.febucci.com/text-animator-unity/welcome/faq)
. Thanks!

* * *

**Have fun** and don't forget to join our [Discord](https://discord.com/invite/j4pySDa5rU)
 to join the conversation and show what you've been creating!

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Requirements & Limitations | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations

Text Animator is a very powerful tool with very few requirements and limitations. **Please read them here before purchasing!**

*   [Requirements](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#requirements)
    
*   [Limitations](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#limitations)
    

You might also be interested in:

*   [Integrations](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#integrating-third-party-dialogue-systems-and-plugins)
    
*   [Frequently Asked Questions](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#frequently-asked-questions)
    
*   [Known Issues](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#known-issues)
    

* * *

[](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#requirements)

Requirements


--------------------------------------------------------------------------------------------------------------------

**The asset works with the following UI and Unity versions**:

*   **Text Mesh Pro** (Unity 2022.3 and up)
    
*   **UI Toolkit** (Unity 6.3 and up).
    

It also supports the new Unity input system (and the legacy one, too).

Please note that we do not officially support Unity Alpha and Beta versions! We have no way to know if Unity changed APIs etc. _the day_ they published the new alpha or beta, so we use these versions to test and make sure the asset works on release/production versions. Thanks!

* * *

[](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#integrating-third-party-dialogue-systems-and-plugins)

Integrations


------------------------------------------------------------------------------------------------------------------------------------------------------------

Integrating third party dialogue systems and plugins:

We are porting all third party integrations in the next weeks! Read more here [Integrated Plugins & Dialogues Systems](https://docs.febucci.com/text-animator-unity/integrations/integrated-plugins-and-dialogues-systems)

* * *

[](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#limitations)

Limitations


------------------------------------------------------------------------------------------------------------------

This is what the asset cannot _(currently)_ achieve.

"Bars" are not animated (by choice)[](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#bars-are-not-animated-by-choice)

“Bars” in texts (`strikethroughs` **and** `underlines`) are not animated by choice.

(Here is how animated bars look like. Since they’re not that good-looking, it’s been chosen to keep them static.)

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2Fj1zQb4UQUOp9BRiaMwTh%2Fbarsnotanimated.gif&width=300&dpr=4&quality=100&sign=1b0918ec&sv=2)

Removing tags when uninstalling the package[](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#removing-tags-when-uninstalling-the-package)

As known (like TMPro), if you uninstall this package you must remove all this plugin’s tags manually from your dialogues.

👍🏻 If you’re worried about this _for any reason,_ **you could set the plugin to use ‘fallback effects’** only (which are applied **to the entirety of the text** without requiring any tag) and everything will be left untouched in case you remove the plugin. Yay!

Using \\r and \\b[](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#using-r-and-b)

You can’t erase or replace _parts_ of the text midtime.

❌ Backspaces (e.g. , `\b` ) are currently not supported

✔️ You can erase/change/replace the **entire** text midtime, and/or hide specific parts of the text.

* * *

[](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#frequently-asked-questions)

Frequently Asked Questions


------------------------------------------------------------------------------------------------------------------------------------------------

Please also read the [Frequently Asked Questions](https://docs.febucci.com/text-animator-unity/welcome/faq)
for common issues and how to fix them. Thanks!

* * *

[](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations#known-issues)

Known Issues


--------------------------------------------------------------------------------------------------------------------

**We are working on a fix and will update the asset as soon as possible anyways**!

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 How to add effects | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects

You can add effects to your texts in the following ways:

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects#set-effects-to-specific-parts-of-the-text)

Set effects to specific parts of the text

You can add effects to specific parts of your text by using **rich text tags.**

The effects tag will look like this:

*   **Persistent**: `<tagID>` to open, `</tagID>` to close
    
*   **Appearances**: `{tagID}` to open, `{/tagID}` to close
    
*   **Disappearances**: `{#tagID}` to open, `{/#tagID}` to close _(basically an appearance tag with a_ `_#_` _before it, to simply remind you that disappearances are appearances in reverse)_.
    

#### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects#extra-notes-about-rich-text-formatting)

Extra notes about Rich Text formatting

By using TextAnimator for Unity:

*   You can stack multiple effects together (e.g. “`<shake><size>`”). (also have a look at [Styles](https://docs.febucci.com/text-animator-unity/customization/styles)
    )
    
*   You can close **all** currently opened effects with a single ‘`/`’ character, like:
    
    *   ”`</>`” for Persistent Effects
        
    *   ”`{/}`” for Appearance Effects
        
    *   ”`{/#}`” for Disappearance Effects.
        
    
*   There is no need to close tags if you’re at the end of the text, since Text Animator starts applying effect from the moment you open a tag. (e.g. "`<shake>hello`" will result in hello already animating).
    

You can change the different

* * *

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects#set-default-effects-to-the-entire-text)

Set default effects to the entire text

You can decide which effect(s) will be applied to all letters by default, **without having to write effects tags inside your texts** thanks to [Animator Settings](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/animator-settings)
.

UI Toolkit

Text Mesh Pro

AnimatedLabel's settings are handled via different scriptable objects (in this case, the one highlighted in the image below). Read more here on [how to create one](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/animator-settings)
.

If you didn't set one, the one in the [Global Settings](https://docs.febucci.com/text-animator-unity/customization/global-settings)
 will be used!

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FagcdeSBrmD3NCQLoVswt%252FScreenshot%25202025-11-15%2520alle%252018.33.00.png%3Falt%3Dmedia%26token%3D6d57fa83-5f8f-475d-940f-280151ce67d5&width=768&dpr=4&quality=100&sign=9f360673&sv=2)

For Text Mesh Pro, settings can be "local" (bound to the component), or "shared" (between other Text Animator instances).

*   To modify **local** settings, simply head over to the "TextAnimator - Text Mesh Pro" component inspector and tweak their values.
    
*   To modify **shared** settings, assign the relative ScriptableObject instance. [Read more here](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/animator-settings)
    .
    

Inside the settings:

1.  Visit the “Default Tags” section
    
2.  Expand the effect’s category you want to edit
    
3.  Add any effect tag you want to include, for example:
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FMGbfDEQeK1CRnktW6aue%252FScreenshot%25202025-11-15%2520alle%252018.48.23.png%3Falt%3Dmedia%26token%3D2a7db44e-c31a-48ae-a317-871ca6006070&width=768&dpr=4&quality=100&sign=f335720&sv=2)

If you don’t want any effect applied by default, simply set the effects’ count to zero.

You can also change the "**Default Tags Mode**" to "**Constant**" if you want effects to be applied all the time, on top of everything.

You can add Modifiers to each array element, like "shake a=5", read more here: [Modifiers](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers)

Example: Fallbacks[](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects#example-fallbacks)

Let's say that we have one default effect ("size"), but we want to apply a specific part of the text with the "fade" effect. We can achieve that result by writing: "default default \`{fade}\` fade fade fade \`{/fade}\` default default"

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FfkwPOWUP3UA38XjdRWRQ%2Ftext-animator-override-appearances-example-ezgif.com-video-to-gif-converter.gif&width=300&dpr=4&quality=100&sign=a2f2d030&sv=2)

As you can see, the letters that are outside the "fade" tags will have the default effect(s) applied, while the part inside "{fade}" and "{/fade}" will only have "fade".

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Install and Quick Start | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/quick-start/install-and-quick-start

Using the asset is a matter of a few clicks (import -> add components -> press play), but to better understand everything please have a look at the following pages, so that you can start even faster and in the right direction.

[](https://docs.febucci.com/text-animator-unity/quick-start/install-and-quick-start#how-to-implement-text-animator)

1\. Import Text Animator for Unity


-----------------------------------------------------------------------------------------------------------------------------------------------------------

As the very first step, you need to import Text Animator for Unity in your project.

#### 

[](https://docs.febucci.com/text-animator-unity/quick-start/install-and-quick-start#compatibility-check)

Compatibility Check

**The asset works with the following UI and Unity versions**:

*   **Text Mesh Pro** (Unity 2022.3 and up)
    
*   **UI Toolkit** (Unity 6.3 and up).
    

It also supports the new Unity input system (and the legacy one, too).

#### 

[](https://docs.febucci.com/text-animator-unity/quick-start/install-and-quick-start#import-the-package)

Import the Package

Once your project is set up correctly, you can import Text Animator from the Package Manager (Asset Store tab).

Make sure to include the "Samples/BuiltIn" folder, or the asset might not work.

After a succesful installation, the **welcome window** will show up and Text Animator is ready to animate your texts!

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252Fo6lFhmxUPaki6oAtVVXZ%252FScreenshot%25202025-11-15%2520alle%252017.40.31.png%3Falt%3Dmedia%26token%3D729acbd3-556d-4808-9726-7f3918afec84&width=768&dpr=4&quality=100&sign=6e6af103&sv=2)

A part of the Welcome Window, which shows after import

In case the about window doesn't show up, or if you want to seet it later, you can access it anytime from the Menu at Tools/Febucci/TextAnimator/About Window!

[](https://docs.febucci.com/text-animator-unity/quick-start/install-and-quick-start#id-2.-example-scenes)

2\. Example Scenes


---------------------------------------------------------------------------------------------------------------------------------

You can learn about most Text Animator features directly from the inspector, and see how we've set up things and their direct result from the example scenes.

Start from the scene called "**00 - Welcome**", or click "Get Started" on Text Animator's welcome window.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FLgTsSXatpKk3M2Nr36VN%252FScreenshot%25202025-11-15%2520alle%252017.45.47.png%3Falt%3Dmedia%26token%3D624c13da-2e67-4653-9caa-076cc5cfa24d&width=768&dpr=4&quality=100&sign=5af94292&sv=2)

To access the example scenes, make sure you have imported them! You can safely remove/delete them once you don't need them anymore, too.

[](https://docs.febucci.com/text-animator-unity/quick-start/install-and-quick-start#animating-your-first-texts)

3\. Animating your first texts


---------------------------------------------------------------------------------------------------------------------------------------------------

You can get your texts running in a few clicks!

UI Toolkit

Text Mesh Pro

_P.S. Assuming you already know_ [_how to use UI Toolkit_](https://docs.unity3d.com/Documentation/Manual/UIElements.html)
 _and what it does._

#### 

[](https://docs.febucci.com/text-animator-unity/quick-start/install-and-quick-start#from-the-ui-builder)

From the UI Builder

*   Go to Library -> Project
    
*   Drag "AnimatedLabel" from "Custom Controls/Febucci/Text Animator for Unity" in your hierarchy!
    

We are working to make sure you can animate built in Labels and Buttons from UI toolkit directly! _(Unity 6.3 and up.)_ Stay updated!

Your .uxml should look like this:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZNwCUmAugxLNcVVO9oBk%252FScreenshot%25202025-11-15%2520alle%252018.02.51.png%3Falt%3Dmedia%26token%3Dced34791-d558-4883-b646-2197664dd637&width=768&dpr=4&quality=100&sign=74a39d74&sv=2)

#### 

[](https://docs.febucci.com/text-animator-unity/quick-start/install-and-quick-start#via-code)

Via Code

You can create an instance of the "Febucci.TextAnimatorForUnity.AnimatedLabel" class and add it to your UI document, like this:

Copy

    using UnityEngine;
    using UnityEngine.UIElements;
    using Febucci.TextAnimatorForUnity; // <- import Text Animator's namespace
    
    public class ExampleScript : MonoBehaviour
    { 
        [SerializeField] UIDocument document;
    
        void Start()
        {
            var container = document.rootVisualElement.contentContainer;
            var animatedLabel = new AnimatedLabel(); // <- create an animated label
            container.Add(animatedLabel); // <- add it to the content container
            // [..]
            animatedLabel.SetText("<wave>hello"); // <- set the text
        }
    }

_P.S. Assuming you already know_ [_how to use Text Mesh Pro_](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html)
 _and how it works._

Add a Text Animator - Text Mesh Pro component on the same GameObject that has a TextMeshPro component (either UI or world space!):

Your inspector should look like this:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FT3h66pIPFdakGOCfToEY%252FScreenshot%25202025-11-15%2520alle%252017.59.18.png%3Falt%3Dmedia%26token%3D26196c49-f0f5-457b-85dd-da358f43c823&width=768&dpr=4&quality=100&sign=38546e2c&sv=2)

You can read [Setting up texts](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts)
 for more details and suggestions!

### 

[](https://docs.febucci.com/text-animator-unity/quick-start/install-and-quick-start#id-1-writing-effects-in-your-text)

Writing effects in your text

One way to adding effects in your text is using rich text tags, like this: “`I'm <shake>freezing</shake>`”, where "shake" is an ID for a built-in effect.

*   Try writing a text by experimenting with the following tags: `<wiggle>` `<shake>` `<wave>` `<bounce>`, like “`<wiggle>I'm joking</wiggle> hehe now <shake>I'm scared</shake>`”, then enter Unity’s Play mode.
    

Your text is animating letters based on the effects you’ve written!

* * *

Have fun animating your texts! You can proceed to the next page for a more in-depth look on all the asset's features.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Setting up texts | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/effects/setting-up-texts

You can set texts to Text Animator from two different UI systems:

*   [UI Toolkit](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#ui-toolkit)
    
*   [Text Mesh Pro](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#text-mesh-pro)
    

This page contains some information already present in [Install and Quick Start](https://docs.febucci.com/text-animator-unity/quick-start/install-and-quick-start)
, but also other details and suggestions for each system and in general. Make sure to read the [Best Practices](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#best-practices)
 section!

* * *

[](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#ui-toolkit)

UI Toolkit


----------------------------------------------------------------------------------------------------

_P.S. Assuming you already know_ [_how to use UI Toolkit_](https://docs.unity3d.com/Documentation/Manual/UIElements.html)
 _and what it does._

#### 

[](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#from-the-ui-builder)

From the UI Builder

*   Go to Library -> Project
    
*   Drag "AnimatedLabel" from "Custom Controls/Febucci/Text Animator for Unity" in your hierarchy!
    

We are working to make sure you can animate built in Labels and Buttons from UI toolkit directly! _(Unity 6.3 and up.)_ Stay updated!

Your .uxml should look like this:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZNwCUmAugxLNcVVO9oBk%252FScreenshot%25202025-11-15%2520alle%252018.02.51.png%3Falt%3Dmedia%26token%3Dced34791-d558-4883-b646-2197664dd637&width=768&dpr=4&quality=100&sign=74a39d74&sv=2)

#### 

[](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#via-code)

Via Code

You can create an instance of the "Febucci.TextAnimatorForUnity.AnimatedLabel" class and add it to your UI document, like this:

Copy

    using UnityEngine;
    using UnityEngine.UIElements;
    using Febucci.TextAnimatorForUnity; // <- import Text Animator's namespace
    
    public class ExampleScript : MonoBehaviour
    { 
        [SerializeField] UIDocument document;
    
        void Start()
        {
            var container = document.rootVisualElement.contentContainer;
            var animatedLabel = new AnimatedLabel(); // <- create an animated label
            container.Add(animatedLabel); // <- add it to the content container
            // [..]
            animatedLabel.SetText("<wave>hello"); // <- set the text
        }
    }

That's all!! You are ready for [How to add effects](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects)

* * *

[](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#text-mesh-pro)

Text Mesh Pro


----------------------------------------------------------------------------------------------------------

_P.S. Assuming you already know_ [_how to use Text Mesh Pro_](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html)
 _and how it works._

Add a Text Animator - Text Mesh Pro component on the same GameObject that has a TextMeshPro component (either UI or world space!):

Your inspector should look like this:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FT3h66pIPFdakGOCfToEY%252FScreenshot%25202025-11-15%2520alle%252017.59.18.png%3Falt%3Dmedia%26token%3D26196c49-f0f5-457b-85dd-da358f43c823&width=768&dpr=4&quality=100&sign=38546e2c&sv=2)

That's all!! You are ready for [How to add effects](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects)

If you're seeing empty texts (but have set them in the component), make sure that you have clicked at least once on a TextMeshPro component and imported the "Essentials" (once their window will pop up and ask you to do so).

#### 

[](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#best-practices-for-setting-text-via-code)

Best Practices for setting text via code

To set the text to your TextMeshPro object via code, please reference Text Animator's script instead of TMPro, like the following:

Copy

    using UnityEngine;
    using TMPro; 
    using Febucci.TextAnimatorForUnity.TextMeshPro; // <- import Text Animator's namespace
    
    public class ExampleScript : MonoBehaviour
    {
        [SerializeField] TMP_Text textMeshPro;
        [SerializeField] TextAnimator_TMP textAnimator;
    
        void Start()
        {
            // 🚫 Don't: set text through TMPro
            textMeshPro.SetText("<wave>hello");
    
            // ✅ Do: set text through Text Animator directly
            textAnimator.SetText("<wave>hello");
        }
    
    }

P.S. Referencing TMPro will work anyways, but setting the text with TextAnimator is better integrated as we have more control on the text.

* * *

[](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#best-practices)

Best Practices


------------------------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#set-the-entire-text-dialogue-only-once)

Set the entire text/dialogue only once

Please try to set text just once, and use the typewriter / visibility methods to control how it appears.

If you really need to append text later in time, you can use the "textAnimator.AppendText" method.

Example[](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#example)

If you have a character that says "Helloooo how are you doing?", and you want to display it letter by letter, simply do: `typewriter.ShowText("Hellooooo how are you doing?");` and that's it! [Show and hide letters dynamically](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically)

* * *

If you're building a dynamic string, you can still do that before setting its value to the typewriter/animator.

Copy

    int apples = 5; //later taken from the game state
    string playerName = "Bob";
    
    // build the entire dialogue line first
    string dialogue = $"Hello {playerName}, you've got {apples} apples";
    
    // then set the text once
    typewriter.ShowText(dialogue);

(If you're using a Dialogue System, they'll do this for you - no worries ! [Integrations](https://docs.febucci.com/text-animator-unity/integrations/integrated-plugins-and-dialogues-systems)
)

Why should I set the entire text once, instead of character by character?[](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#why-should-i-set-the-entire-text-once-instead-of-character-by-character)

Performance! (Even if you didn't have Text Animator.)

Every time you set the text, TextMeshPro or UI toolkit need to calculate its mesh, positioning etc., and Text Animator has then to re-calculate character durations and more. This means that if you change it multiple times per second (e.g. adding more letters), you're doing these calculations every time.

To display characters one by one, you can simply set the full text once, and then start the typewriter: [Show and hide letters dynamically](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically)

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Core Concepts | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/quick-start/core-concepts

### 

[](https://docs.febucci.com/text-animator-unity/quick-start/core-concepts#effects)

Effects

You can apply effects at different phases of a character's "life":

**Appearances**

![An example of the Appearance Effect {vertexp}](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FT7U4C8xOWPU5tjtdhxHT%2Fverticalexpandpreview.gif&width=300&dpr=4&quality=100&sign=2d90d0dc&sv=2)

For animating letters only when they’re appearing on screen. _(More...__)_

**Persistent**

![An example of the Behavior Effect <wiggle>](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FkXQFZNbm8mSv67m9nubS%2Fwigglepreviewfebucci.gif&width=300&dpr=4&quality=100&sign=1ff9ee43&sv=2)

For animating letters effects continuously during time, as long as a letter is visible.

**Disappearances**

![An example of the Disappearance Effect {#size}](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FsHBEkEs6y1POC6EOORwf%2Fdecreasing%2520size%2520text%2520animator%2520unity4.gif&width=300&dpr=4&quality=100&sign=89a11fab&sv=2)

For animating letters when they just became not-visible.

Since Text Animator 3.0, any effect can be played in any stage of a letter! (Appearance, Persistent or Disappearance)

#### 

[](https://docs.febucci.com/text-animator-unity/quick-start/core-concepts#mix-and-match-values)

Mix and match values

Even if you do have "default" effects and values, you can always modify them through the inspector or via text.

* * *

### 

[](https://docs.febucci.com/text-animator-unity/quick-start/core-concepts#settings-accessibility)

Settings accessibility

Text Animator uses many different settings, from animations to typewriters and more.

For most occasions, you can apply different these settings at three different levels:

*   **Locally:** settings are bound to that component
    
*   **Shared:** settings are stored in a ScriptableObject, and will be shared among other instances that have that ScriptableObject reference.
    
*   **Global:** either the settings will be applied on top of others (e.g. in the case of recognizing effects), or will be used _only_ if no other setting was specified (as a "fallback", like in the case of curves).
    

* * *

### 

[](https://docs.febucci.com/text-animator-unity/quick-start/core-concepts#databases)

Databases

Text Animator uses ScriptableObjects to store information about _what exists_ and can be used, as well as the building blocks for animations and typewriters (effects, wait times, curves etc.).

* * *

### 

[](https://docs.febucci.com/text-animator-unity/quick-start/core-concepts#editor-tooltips)

Editor Tooltips

You can hover the mouse above many options and fields in the inspector to show some tooltips and extra information!

Last updated 1 month ago

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Frequently Asked Questions | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/welcome/faq

### 

[](https://docs.febucci.com/text-animator-unity/welcome/faq#localization)

Localization

Does Text Animator work with multiple languages?[](https://docs.febucci.com/text-animator-unity/welcome/faq#does-text-animator-work-with-multiple-languages)

Short answer: **yes,** _**but it doesn't depend from Text Animator**_.

*   About translated text: Yes, but localization is not handled by Text Animator. Localization is handled by external scripts instead (it could be your own localization manager, a dialogue system, \[…\]. In other words, Text Animator is not a localization plugin. If you have a text that contains a rich text tag, it must have the same layout in the translated Language (example “hello <shake> how are you?”, should be translated to “ciao <shake> come stai?”). Then, you can simply call "textAnimatorComponent.ShowText(translatedText);". (This localization process also applies to any other game/project \[...\])
    
*   About different fonts: Yes, but it's not handled by Text Animator (it's handled by Text Mesh Pro instead). If TMPro supports a language, text animator will do as well. This is because Text Animator only animate letters, which are generated by TextMeshPro.
    

Is RTL text supported? (right to left)[](https://docs.febucci.com/text-animator-unity/welcome/faq#is-rtl-text-supported-right-to-left)

Yes! Behind the scenes TextAnimator only moves the mesh, but it's TextMeshPro that generates it. TMPro supports RTL text (you can enable it from the component's inspector), and consequently so does TextAnimator.

_Please_ be aware that external packages like "RTLTMPro" might not be supported entirely, as we're strictly referring to TMPro, so have a look at the [Integrated Plugins & Dialogues Systems](https://docs.febucci.com/text-animator-unity/integrations/integrated-plugins-and-dialogues-systems)
 instead.

* * *

### 

[](https://docs.febucci.com/text-animator-unity/welcome/faq#integrations-and-versions)

Integrations & Versions

Which Unity versions are supported?[](https://docs.febucci.com/text-animator-unity/welcome/faq#which-unity-versions-are-supported)

You can discover which Unity versions are supported by reading here: [Requirements & Limitations](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations)

Is my \[insert dialogue system here\] supported?[](https://docs.febucci.com/text-animator-unity/welcome/faq#is-my-insert-dialogue-system-here-supported)

You can discover which third party plugin is already integrated with Text Animator by reading here: [Integrated Plugins & Dialogues Systems](https://docs.febucci.com/text-animator-unity/integrations/integrated-plugins-and-dialogues-systems)

Is UIToolkit Supported?[](https://docs.febucci.com/text-animator-unity/welcome/faq#is-uitoolkit-supported)

Yes! From Unity 6.3 and above.

* * *

### 

[](https://docs.febucci.com/text-animator-unity/welcome/faq#effects-and-parsing)

Effects and parsing

Can I change the symbols for rich text parsing? (e.g. "\[shake\]" instead of "<shake>")[](https://docs.febucci.com/text-animator-unity/welcome/faq#can-i-change-the-symbols-for-rich-text-parsing-e.g.-shake-instead-of-less-than-shake-greater-than)

Yes! From the [Global Settings](https://docs.febucci.com/text-animator-unity/customization/global-settings)
 file.

When are effects applied? After opening tags or after closing them?[](https://docs.febucci.com/text-animator-unity/welcome/faq#when-are-effects-applied-after-opening-tags-or-after-closing-them)

An effect is applied from the moment you open its tag.

"<shake>hello" will already have the word "hello" shaking from the moment you set the first '\>' character.

Does TextAnimator preview effects in Edit Mode?[](https://docs.febucci.com/text-animator-unity/welcome/faq#does-textanimator-preview-effects-in-edit-mode)

Yes! Simply click on an effect Scriptable Object to see its preview. [How to edit effects](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects)

* * *

### 

[](https://docs.febucci.com/text-animator-unity/welcome/faq#other)

Other

I have some questions about licensing[](https://docs.febucci.com/text-animator-unity/welcome/faq#i-have-some-questions-about-licensing)

You can read [info about licensing here](https://www.textanimatorforgames.com/unity#faq)
.

Can I use Text Animator in a web build?[](https://docs.febucci.com/text-animator-unity/welcome/faq#can-i-use-text-animator-in-a-web-build)

Yes!

Can I delete the plugin's "Example" folder?[](https://docs.febucci.com/text-animator-unity/welcome/faq#can-i-delete-the-plugins-example-folder)

Sure, you can delete the plugin's example folder if you don't need it.

_Who is awesome?_[](https://docs.febucci.com/text-animator-unity/welcome/faq#who-is-awesome)

You are awesome!

* * *

### 

[](https://docs.febucci.com/text-animator-unity/welcome/faq#ask-us-something)

Ask us something

In case you have extra questions, please feel free to contact us!

[![Logo](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fframerusercontent.com%2Fimages%2Fy1LCEnd5hyGjuX0kKaGBUorzMc.png&width=20&dpr=4&quality=100&sign=82d1be32&sv=2)Support Requests | Text Animator for Unity, Godot and Unrealwww.textanimatorforgames.com](https://www.textanimatorforgames.com/support)

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Built-in effects list | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/effects/built-in-effects-list

Here is the default/built-in database we created, which is already available (import the "Samples" folder!) and contains many effects ready to use in your games.

You can also create your own effects anytime!

*   [Create your own effects](https://docs.febucci.com/text-animator-unity/customization/create-your-own-effects)
    
*   [Writing Custom Effects (C#)](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-effects-c)
    

Since Text Animator for Unity 3.0, any effect can be played as an Appearance, Persistant and Disappearance, and you can also play them once or based on other conditions!

You can also use [Modifiers](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers)
, which let you change the characteristics of your Behavior effects individually.

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2F577I8LcLJl1quOreidHC%2Fpendulumpreview.gif&width=490&dpr=4&quality=100&sign=fa9422c7&sv=2)

**Pendulum**

Tag

pend

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FewfXieMBJaRjEcihXyeT%2Fdanglepreview.gif&width=490&dpr=4&quality=100&sign=d00e4c63&sv=2)

**Dangle**

Tag

dangle

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2Fd0wCTrvN7t49jUBGNqI0%2Ffadepreview.gif&width=490&dpr=4&quality=100&sign=4a33090f&sv=2)

**Fade**

Tag

fade

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FCbIcUivK6TUlvvPHQx9l%2Frainbowpreviewfebucci.gif&width=490&dpr=4&quality=100&sign=fa7368ab&sv=2)

**Rainbow**

Tag

rainb

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FONRSbf0b6oeC6tUYL7Ef%2Frotatingpreviewfebucci.gif&width=490&dpr=4&quality=100&sign=2bfdc2cd&sv=2)

**Rotate**

Tag

rot

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2Fbhm0HLqRADQj3RCVHUN2%2Fbouncepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=ba59014d&sv=2)

**Bounce**

Tag

bounce

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FImNDiBy3MuZpT1fB0UxF%2Fslidepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=5c1b22c2&sv=2)

**Slide**

Tag

slideh

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2F9zSq1hqy61sKFcWpOxNI%2Fswingpreviewfebucci.gif&width=490&dpr=4&quality=100&sign=dec9d5f5&sv=2)

**Swing**

Tag

swing

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FaZftI1kdTYBEZedse6qJ%2Fwavepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=9cb0fc71&sv=2)

**Wave**

Tag

wave

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FT3x704G3ZSzv4Hi4h4jA%2Fsizepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=8e27b570&sv=2)

**Increase Size**

Tag

incr

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2F21sLOk7GG8dv7I0XaGMO%2Fshakepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=13725beb&sv=2)

**Shake**

Tag

shake

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2Fcwposy2qWMvqTWq81T5e%2Fwigglepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=b01cd84b&sv=2)

**Wiggle**

Tag

wiggle

### 

[](https://docs.febucci.com/text-animator-unity/effects/built-in-effects-list#glossary)

Glossary

Modifier ID

Modifier Value

Name

In other words

a

floating point number, example: 3

amplitude

effect's strength

s

floating point number, example: 3

speed

speed

*   `Tag`: represents the effect tag, unique in its category (eg. <shake>)
    

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 How to edit effects | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects

You can edit any effect by clicking on its Scriptable Object in the project window. You will find a live preview in Edit mode (from Unity 6.3, other versions coming asap) that shows you how the effect is applied to different stages of a letter (appearing, disappearing and persistent).

You can also modify an effect through Rich Text Tags, with [Modifiers](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers)
 (e.g. **<wave s=2>** to make it twice as fast).

* * *

It is important that you always set the **Tag ID**, otherwise the effect will not be recognized in the database!

In the inspector you will also find additional parameters, useful to modify effects even more, like:

*   **Bake curves**: keep this to on! It optimizes your effects especially for critical contexts (if you have a lot of letters and a lot of effects on top of each other)
    
*   **Override** [Global Settings](https://docs.febucci.com/text-animator-unity/customization/global-settings)
     with a custom [Curves](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/curves)
     or [Playbacks](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/playbacks)
     than default
    

Time to Sync persistant is WIP! Let us know your feedback!

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F6JMmtL11b32xG7FmgEv7%252FScreenshot%25202025-11-16%2520alle%252017.18.36.png%3Falt%3Dmedia%26token%3Db4a87c65-eb10-44be-864a-c27ceba45445&width=768&dpr=4&quality=100&sign=51b0c6e&sv=2)

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Show and hide letters dynamically | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically

**You can use a Typewriter to show and hide letters dynamically**, choosing different pauses for any kind of characters (punctuation, letters, \[…\]), trigger events and more.

* * *

[](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#showing-text)

Showing Text


----------------------------------------------------------------------------------------------------------------------------

The typewriter contains general settings and events listeners, and allows for different pauses/timing modes:

*   **By Character**: shows one letter after the other.
    
*   **By Word**: progresses text word after word.
    

This new architecture (from 3.0) allows you to change typewriter timings during development (for whatever reason) while keeping event references and settings intact! <3

**Your typewriter should look like this:**

Text Mesh Pro

UI Toolkit

From the TypewriterComponent in the Inspector:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F4QBGWiDjjQq4LXVbhNfc%252FScreenshot%25202025-11-16%2520alle%252018.15.08.png%3Falt%3Dmedia%26token%3Daeb76665-1ea4-498e-9181-091ddf322063&width=768&dpr=4&quality=100&sign=3832d3ba&sv=2)

From the AnimatedLabel in the the UI Builder:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FB0i06unOYgu5XhHcdZN8%252FScreenshot%25202025-11-16%2520alle%252018.21.21.png%3Falt%3Dmedia%26token%3Dcf1193d6-cac5-47d4-93bf-b89a121f0046&width=768&dpr=4&quality=100&sign=5c5442b9&sv=2)

Make sure to assign the timings scriptable object, or the typewriter will show the entire text instantly!

* * *

You can start the typewriter in two main ways:

### 

[](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#a-via-code-recommended)

A) Via Code (Recommended)

If you want to use the typewriter, **it is recommended that you set the text directly to that component via code.**

Text Mesh Pro

UI Toolkit

If you are using TextMeshPro, please replace scripts that reference TMPro or Text Animator ([Setting up texts](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts)
) and reference `Febucci.TextAnimatorForUnity.TypewriterComponent` instead.

*   ❌ Don't: “`tmproText.text = textValue;`” , or "`textAnimator.SetText(textValue);`"
    
*   ✅ Do: `typewriter.ShowText(textValue);`
    

Via UI Toolkit, the `AnimatedLabel` already has a "`Typewriter`" value that you can interact with! You don't need to do anything else, except for making sure you have assigned typewriter delays.

### 

[](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#b-via-the-easy-integration)

B) Automatic recognition

In case you haven't followed the step above, TextAnimator will still try to start the typewriter automatically if you have added a Typewriter component or have set up "Timings" through the AnimatedLabel in UI Toolkit.

Easy Integration might happen one frame behind (as it has to discover that something has changed first, which usually was done on the previous frame, and then start the typewriter). If this is an issue, either follow the step [A) Via Code (Recommended)](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#a-via-code-recommended)
, or see [When I set the text, I see the previous one for one frame before showing the new one](https://docs.febucci.com/text-animator-unity/other/troubleshooting#when-i-set-the-text-i-see-the-previous-one-for-one-frame-before-showing-the-new-one)

* * *

[](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#controlling-letters)

Controlling Letters


------------------------------------------------------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#start-and-stop-typing)

Start and Stop Typing

Inside the component’s Inspector you’ll find some options to control how the typewriter start should be triggered:

*   `Start Typewriter Mode`: tells the typewriter when to start showing letters.
    

Value

Explanation

**From Script Only**

The typerwiter can only be started by invoking [TextAnimatorPlayer.StartShowingText()](https://www.api.febucci.com/tools/text-animator-unity/api/Febucci.UI.Core.TypewriterCore.html#Febucci_UI_Core_TypewriterCore_ShowText_System_String_)

**OnEnable**

The typewriter starts every time the gameObject is set active

**OnShowText**

The typewriter starts as soon as a new text is set ([as explained in the “Showing Text” section](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#showing-text)
)

**Automatically From All Events**

All of the above

*   `Reset Typing Speed At Startup`: true if you want the typewriter’s speed to reset back to 1 every time a new text is show, otherwise it will save the last one used.
    

You can pause the typewriter at any time by invoking `typewriter.StopShowingText()`, and you can start/resume it by invoking `typewriter.StartShowingText()`.

### 

[](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#skip)

Skip the Entire Text

To Skip the entire typewriter, you can invoke the `typewriter.SkipTypewriter()` method.

You can also find a few options to control how it behaves:

*   `Hide Appearances On Skip`: true if you want to prevent appearance effects from playing whenever the typewriter skips (meaning that the text will be shown instantly).
    
*   `Trigger Events On Skip`: true if you want to trigger all remaining events once the typewriter skips (be careful with that if you’re running some game logic with them, as everything will be run at once). Read more about events here: [Trigger Events when typing](https://docs.febucci.com/text-animator-unity/typewriter/trigger-events-when-typing)
    

### 

[](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#skip-specific-parts-of-the-text)

Skip Specific Parts of the Text

This feature is under testing for 3.0 and will be restored from the next version very soon! Thanks for your understanding!

### 

[](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#hiding-text)

Hiding Text

You can hide letters dynamically via script, by invoking `typewriter.StartDisappearingText()`, and you can also stop it at any time by invoking `typewriter.StopDisappearingText()`.

* * *

You can create your own timing waits (read [here](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-typing-waits-c)
 how via C#) or you can use the built-in ones.

[](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#options)

Options


------------------------------------------------------------------------------------------------------------------

Typewriters might share the same settings and also have specific ones, so be sure to hover the mouse cursor above its fields in the Inspector to show the tooltips for each one.

Here is a quick overview of the most important/common ones:

### 

[](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#callbacks-unity-events)

Callbacks (Unity Events)

You can use Unity Events that will be triggered based on the typewriter activity (example: when it just ended showing text).

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FWHU9EqhOj3uN5AI3PURA%252FScreenshot%25202025-11-16%2520alle%252018.34.38.png%3Falt%3Dmedia%26token%3D7757f0b7-300e-4637-8495-542fab1b0fe5&width=768&dpr=4&quality=100&sign=a793fbd3&sv=2)

Event

Explanation

`OnTextShowed`

Event called after the entire text has been shown (_if you’ve set “Use Typewriter” to true, it will wait until all letters are shown_)

`OnTextDisappeared`

Called as soon as the script starts hiding the last letter

The following below only work if the “**use typewriter**” is set to **true**:

Event

Explanation

`OnTypewriterStart`

Called right before the typewriter starts showing its first letter. It doesn’t work if the typewriter is off, since it would coincide with the “OnTextShowed” event _(in this case you can use that one instead)_

`OnCharacterVisible(Char)`

Called each time a character became visible

`OnMessage(EventMarker)`

Invoked every time the typewriter meets a message/event in text. Read more about events [here](https://docs.febucci.com/text-animator-unity/typewriter/trigger-events-when-typing)

A typewriter uses its linked Text Animator **Time Scale** to progress time (you can read more here: [Time Scale](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/animator-settings#time-scale)
), meaning that if the time is set to "Unscaled", then the typewriter will progress even if your game is paused.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Wait Actions when typing | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/typewriter/wait-actions-when-typing

**You can perform actions once the typewriter reaches a specific position in the text**. _For this reason, actions work only if the typewriter is enabled._

Example: waiting for X seconds or waiting for the player input.

* * *

[](https://docs.febucci.com/text-animator-unity/typewriter/wait-actions-when-typing#how-to-add-actions-in-your-text)

How to add actions in your text


---------------------------------------------------------------------------------------------------------------------------------------------------------

You can add actions to your text by using rich text tags.

Actions’ formatting follows this formula: “`<actionID>`” or “`<actionID=attribute1,attribute2,...>`” for eventual parameters/attributes (just like events/messages).

Actions tags are case insensitive, `<waitfor>` and `<waitFor>` will produce the same results.

#### 

[](https://docs.febucci.com/text-animator-unity/typewriter/wait-actions-when-typing#parameters)

Parameters

Actions support multiple parameters, after the ‘`=`’ sign and all separated by a `comma`.

Example: `<waitfor=1.5>` or `<playaudio=tada,laugh,dub>`

*   ⚠️ Floating point numbers must use a `period`, not a `comma`.
    
    *   ✔️ <speed=0.5>
        
    *   ❌ <speed=0,5>
        
    

* * *

[](https://docs.febucci.com/text-animator-unity/typewriter/wait-actions-when-typing#databases)

Databases


-------------------------------------------------------------------------------------------------------------

As with Effects, you’ll find Actions stored inside their databases. You can add and remove as many as you prefer, create specific ones and also [program your own via C#](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-actions-c)
.

[](https://docs.febucci.com/text-animator-unity/typewriter/wait-actions-when-typing#built-in-actions)

Built-in a


---------------------------------------------------------------------------------------------------------------------

You can use the following built-in actions in your text.

**Wait for Seconds**

Waits for X seconds before continuing to show the text

Tag

waitfor

Attributes

float (wait duration)

Example

<waitfor=3>

**Wait for Input**

Waits for the player input

Tag

waitinput

Attributes

N/A

Example

<waitinput>

**Speed**

Multiplies the typewriter speed

Tag

speed

Attributes

float (speed multiplier)

Example

<speed =2>

[](https://docs.febucci.com/text-animator-unity/typewriter/wait-actions-when-typing#component-actions)

Component Actions


-----------------------------------------------------------------------------------------------------------------------------

Some actions are available only if they exist on scene (you need to create them as components).

**Play Sound**

Plays an Audio Source (referenced in the inspector) and waits until it finishes

Tag

psound

Attributes

N/A

Example

<psound>

### 

[](https://docs.febucci.com/text-animator-unity/typewriter/wait-actions-when-typing#local-actions)

Local Actions

You can make an action _local_, meaning it is only recognized if you create them next to a Typewriter Component. (only works for **TextMeshPro**)

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FclEP11Pk4aO6mj1dRttg%252FScreenshot%25202025-11-17%2520alle%252015.33.37.png%3Falt%3Dmedia%26token%3D9370c9b0-eb9c-4408-8d7c-da316d4a77c1&width=768&dpr=4&quality=100&sign=d877e4cb&sv=2)

### 

[](https://docs.febucci.com/text-animator-unity/typewriter/wait-actions-when-typing#global-actions)

Global Actions

Global actions are accessed by any Typewriter currently typing on scene, as long as you set "Make Available Globally" to ON.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Trigger Events when typing | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/typewriter/trigger-events-when-typing

Events are special tags that let you send messages (string) to any listener script, once the typewriter has reached a specific part of the text. _(For this reason, events work only if the typewriter is enabled)_

![textanimatorgif2febucci](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2F3UxVpaMvfQpqNMeoWA2v%2Ftextanimatorgif2febucci.gif&width=768&dpr=4&quality=100&sign=b35a2221&sv=2)

Scene 'Example 3 - Events'

* * *

[](https://docs.febucci.com/text-animator-unity/typewriter/trigger-events-when-typing#overview)

Overview


-------------------------------------------------------------------------------------------------------------

You can write events in your text by using rich text tags.

### 

[](https://docs.febucci.com/text-animator-unity/typewriter/trigger-events-when-typing#formatting)

Formatting

Event’s messages are preceded by a question mark, like this: `<?eventMessage>`.

**Example:** To call an event named ‘shakeCamera’, write: `<?shakeCamera>`

*   👍🏻 An event can have any kind of tag, including built-in effect’s ones.
    
*   ⚠️ Events are case sensitive. Writing `<?camshake>` is not the same as writing `<?camShake>`. Be careful! (or use the `string.ToLower()` method in your scripts to account for that.)
    

### 

[](https://docs.febucci.com/text-animator-unity/typewriter/trigger-events-when-typing#parameters)

Parameters

Events can have one or multiple parameters (starting with the `=` sign for the first, and then separating the others with a comma `,`), to allow you to send multiple data to your scripts.

*   One parameter: `<?eventID=parameter1>`, will result in a message “eventID” and one parameter “parameter1”.
    
*   Multiple parameters: `<?eventID=p1,p2>`, will result in a message “eventID” and parameters “p1” and “p2”.
    

* * *

[](https://docs.febucci.com/text-animator-unity/typewriter/trigger-events-when-typing#listening-to-events)

Listening to events


-----------------------------------------------------------------------------------------------------------------------------------

The scripts that you want to listen from events/messages must subscribe to the `onMessage` callback inside the `Typewriter` class. ([Scripting Api](https://www.api.febucci.com/tools/text-animator-unity/api/Febucci.UI.Core.TypewriterCore.html#Febucci_UI_Core_TypewriterCore_onMessage)
).

Example:

Copy

    // inside your script
    [SerializeField] TypewriterComponent typewriter;
    
    // adds and removes callbacks
    void OnEnable() => typewriter.onMessage.AddListener(OnMessage);
    void OnDisable() => typewriter.onMessage.RemoveListener(OnMessage);
    
    // does stuff based on the received marker
    void OnMessage(EventMarker marker)
    {
        switch (marker.name)
        {
            // once the typewriter meets the "<?something>" tag
            
            case "something":
                // do something
                break;
        }
    }

👍🏻 Note how the “message” string has no ‘<‘, ‘?’ and ‘>’ characters, but only contains the message.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Play sounds when a letter is shown | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/typewriter/play-sounds-when-a-letter-is-shown

To implement typewriter sounds in your game, you can subscribe to the Typewriter’s “`OnCharacterVisible`” event and play sounds based on it.

The event passes a “char” as a parameter, so you can play different sounds based on different letters as well.

_P.S. The event is also triggered with spaces, so be sure to play sounds based on the type of character you prefer._

* * *

### 

[](https://docs.febucci.com/text-animator-unity/typewriter/play-sounds-when-a-letter-is-shown#example-package)

Example Package

As an example, you can install the “TypeWriter Sounds” package found inside the “Extra” folder and check its implementation.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FLld5xGmtqLsHHY6U2x1g%2FUntitled.png&width=768&dpr=4&quality=100&sign=6f3e25a8&sv=2)

Variable

Explanation

`Source`

Main audio source where sounds will be played

`MinSoundDelay`

Minimum time that has to pass before playing another sound

`Interrupt Previous Sound`

If true, the previous audio will be stopped

`Random Sequence`

If true, the next audio clip to play will be chosen randomly from the “Sounds” array. If false, sounds will be played subsequently

`Sounds`

Typewriter sounds to play

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Create your own effects | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/customization/create-your-own-effects

In TextAnimator for Unity you can create custom effects in many different ways.

*   [Creating Effects from the Inspector](https://docs.febucci.com/text-animator-unity/customization/create-your-own-effects#creating-effects-from-the-inspector)
    
*   [Writing Custom Effects (C#)](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-effects-c)
    

Feel free to choose the best methods that better fit you!

### 

[](https://docs.febucci.com/text-animator-unity/customization/create-your-own-effects#recommendations)

Recommendations

As you will discover the more you dive into Text Animator, thanks to custom effects, curves and playbacks you’re able to create pretty powerful combinations and results! That said (as with anything powerful) **it’s up to you to use them wisely**! In theory you could create an endlessly reference of animations inside each one, resulting in a stack-overflow, or per-vertex animations that are too demanding for your target hardware, resulting in a frame drop if you have too many texts/animations on screen, so please be careful to not overdo things!

That said… have fun!

* * *

[](https://docs.febucci.com/text-animator-unity/customization/create-your-own-effects#creating-effects-from-the-inspector)

Creating Effects from the Inspector


-------------------------------------------------------------------------------------------------------------------------------------------------------------------

Other than already available built-in effects, **you can create your own effects from the inspector directly (without having to write any code)**.

P.S. If you _do_ want to write custom effects via C#, please have a look at [Writing Custom Effects (C#)](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-effects-c)

As always, to create a custom effect head over to the Project Window -> Create -> Text Animator for Unity and then select any element from the "Effects" menu.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F0e1f9LNQxJvGr7X9eaKJ%252FScreenshot%25202025-11-16%2520alle%252018.45.04.png%3Falt%3Dmedia%26token%3Db64409f1-f23d-4242-b7ca-2b74890cdf6e&width=768&dpr=4&quality=100&sign=d05b2ea&sv=2)

You can create two different type of effects from the inspector:

*   [Direct Effects](https://docs.febucci.com/text-animator-unity/customization/create-your-own-effects/direct-effects)
    
*   [Curve Effects](https://docs.febucci.com/text-animator-unity/customization/create-your-own-effects/curve-effects)
    

Last updated 1 month ago

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Global Settings | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/customization/global-settings

You can use global settings to handle many of the assets features.

A "TextAnimatorSettings" ScriptableObject is required to be placed in the Resources folder. The plugin should do it for you automatically when you import it, and if it doesn't find it it should fix it automatically as well!

The main options are:

*   Toggles to enable or disable animation categories **globally** (for all text animators)
    
*   Set different databases to be recognized automatically for
    
*   Change the parsing symbols (e.g. "\[\]" brackest instead of "<>" for persistent effects)
    
*   Set "fallbacks" that will be used in case optios are not set in components
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F0AZYkreB0l3zafPMLFNT%252FScreenshot%25202025-11-17%2520alle%252014.14.40.png%3Falt%3Dmedia%26token%3Dc1205e8b-c0dc-420c-91bb-5f16383b7489&width=768&dpr=4&quality=100&sign=1377a1af&sv=2)

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Integrated Plugins & Dialogues Systems | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/integrations/integrated-plugins-and-dialogues-systems

We are re-writing all the integration pages to make sure Text Animator 3.0 is up to date with all the previous 3rd party assets:

*   Dialogue System for Unity
    
*   Ink
    
*   Game Creator 2
    
*   Unity Localization Package
    
*   Unity Visual Scripting
    
*   Playmaker
    
*   Naninovel
    

We are also working to integrate more packages as well, for example:

*   Adventure Creator
    

### 

[](https://docs.febucci.com/text-animator-unity/integrations/integrated-plugins-and-dialogues-systems#easy-integration)

Officially Supported Third Parties

**Yarn Spinner**

### 

[](https://docs.febucci.com/text-animator-unity/integrations/integrated-plugins-and-dialogues-systems#easy-integration-1)

Easy Integration

Most assets should work through an _easy integration_, meaning that the asset should be able to pick text changes from Text Mesh Pro and start the typewriter from that. BUT official integrations are coming soon!

Invisible tags appended to your text

_If you're using Easy Integration, TextAnimator will add two invisible tags in appendix to your text in order to work. No worries, the text placement/layout will be left unchanged and it will act like if the tags are not written at all._

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Styles | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/customization/styles

Styles quickly replace parts of the text with something else, for example to create a combo of effects, typewriter actions and events, which would otherwise require you a lot of typing for recurring tags.

If you're using TMPro, please use Text Animator Styles and not TMPro ones for this, as the latter (TMPro's) can't recognize Text Animator tags and will result in them being added to the text.

* * *

Simply open the stylesheet scriptable object of your choice (you can create one in the Project Folder, via the Create menu -> Text Animator-> StyleSheet) and start adding/editing tags.

You can have a Global stylesheet ( [Global Settings](https://docs.febucci.com/text-animator-unity/customization/global-settings)
 ) and also a local one.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FbEgcnrQ9RzsRjy1jCb7m%2Ftextanimator%2520settings%2520stylesheet%2520example.png&width=768&dpr=4&quality=100&sign=f266ed25&sv=2)

From the example above, whenever you write the style tag “`<style1>`” in the text, it will be replaced with “`<wave><play=5><rainb><shake>`” - and closing it with “`</style1>`” will be replaced with “`</wave></rainb></shake><?ended>`”.

Styles tags are case insensitive (writing "<style1>" and "<Style1>" will produce the same result).

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Advanced Concepts | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts

Behind the scenes, Text Animator is doing a lot of work and optimization to make sure:

*   There is 0 garbage collection during animations _(there is still some when the text is set, as does TMPro and also Text Animator 2.0, but we're working on it!)_
    
*   The asset is compatible with different Unity versions, systems and platforms
    
*   There is an API that is as simple as possible for you _(putting the pain on us, but that's the whole point!)_
    
*   Things work even if there is a wrong setup with null references (as humanly possible)
    

That said, there are some key concepts inside Text Animator for Unity that are important to know when you start writing custom scripts:

*   [Core Library](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts#core-library)
    
*   [Stateless vs Referenced elements](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts#stateless-vs-referenced-elements)
    

* * *

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts#core-library)

Core Library


------------------------------------------------------------------------------------------------------------------------

Text Animator is divided in two main namespaces:

*   The "`Febucci.TextAnimatorCore`" is our **core library,** a runtime DLL shipped inside the package and that is foundamental to make things work.
    
*   The "`Febucci.TextAnimatorUnity`" is the **Unity implementation**, from Scriptable Objects to Monobehaviors and more.
    

You will find how to set up scripts as intended in the next pages/guides, but please be careful about what you inherit, modify or re-implement!

I'll keep updating the core library to implement new features or reorganize the structure, and it's impossible to know any kind of variation and use case people might do in C# (especially if not intended) - so please follow the guides! I'll mark things internal as much as possible anyways and I'll keep the Unity implementation as backwards compatible as possible between versions (as I always did in the past years, also including an updating guide where applicable) - but if you want to do some not-planned modification do it at your own risk!

If you do upgrade Unity version mid-project, please remove the asset and re-download it from the package manager (it will download the package built for that Unity version, behind the scenes!)

[I upgraded Unity version (2022.3->Unity 6.3) and there are some errors with Text Animator](https://docs.febucci.com/text-animator-unity/other/troubleshooting#i-upgraded-unity-version-2022.3-greater-than-unity-6.3-and-there-are-some-errors-with-text-animator)

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts#stateless-vs-referenced-elements)

Stateless vs Referenced elements


----------------------------------------------------------------------------------------------------------------------------------------------------------------

Most Text Animator elements, from effects, actions, playbacks and curves, are implemented in two ways. One is independent from Unity and GameObjects/ScriptableObjects in general, and the other keeps references from the game state / files and classes.

Type

Pros

Cons

Stateless

*   Better optimized (also prepared for Burst in the future, TBD)
    
*   No race conditions between elements
    

*   Some code wrappers, BUT mitigated through the asset's custom classes!
    
*   Can't modify animations/typewriters based on the game state
    

Referenced

*   Can access the game state and make things happen differently based on it
    

*   Possible race conditions if not implemented correctly (e.g. two typewriters accessing the same action, which has a timer or makes things happen, at the same time)
    
*   Can't be optimized through Burst (but should be negligible in most occasions, as built-in ones do the heavy part)
    

We are also investigating for a way to give you _**Direct**_ elements, which mean: remove all or own implementations and just let you hook things how you want (which should accomodate like the 1% of the users, given all the other tools available, but still an important option in our opinion).

*   **Pros**: Do it yourself.
    
*   **Cons**: Do it yourself.
    

It's up to you to decide how to customize your elements.

*   Opt for stateless types when you are in performance-critical context (e.g. having many letters)
    

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Writing Custom Typing Waits (C#) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-typing-waits-c

By using “Text Animator for Unity” you can create your own **custom typewriter waits**, setting different types of delays between letters and much more.

If you want to learn about the default typewriter instead, [read here](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically)

Be sure to have read the [Advanced Concepts](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts)
 page as well.

* * *

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-typing-waits-c#adding-custom-typewriters)

Adding Custom Typewriter Waits


-------------------------------------------------------------------------------------------------------------------------------------------------------------------

In order to create a custom typewriter wait you need to to create a Scriptable Object class that inherits from `Febucci.TextAnimatorForUnity.TypingsTimingsScriptableBase`

Here is a simple example script:

Copy

    // import the necessary Febucci namespaces
    using Febucci.TextAnimatorCore;
    using Febucci.TextAnimatorCore.Text;
    using Febucci.TextAnimatorForUnity;
    
    using UnityEngine;
    
    [System.Serializable] // <--- remember to serialize your scriptables!
    [CreateAssetMenu(fileName = "Custom Typewriter Waits")]
    class CustomTypingWaits : TypingsTimingsScriptableBase
    {
        // --- put your properties here as normal
        [SerializeField] float delay = .1f;
        
        // custom waits when showing text
        public override float GetWaitAppearanceTimeOf(CharacterData character, TextAnimator animator)
        {
            // example: skips spaces
            if (char.IsWhiteSpace(character.info.character))
                return 0;
    
            return delay;
        }
    
        // custom waits when disappearing text
        public override float GetWaitDisappearanceTimeOf(CharacterData character, TextAnimator animator)
        {
            // in this case, it's the same as appearances
            return GetWaitAppearanceTimeOf(character, animator);
        }
    }

* * *

That’s it!

Don’t forget to create the scriptable object in your assets folder, and to assign it to your Typewriter component. Read more here: [Show and hide letters dynamically](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically)

Have fun implementing your own typewriters <3

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Troubleshooting | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/other/troubleshooting

When I set the text, I see the previous one for one frame before showing the new one[](https://docs.febucci.com/text-animator-unity/other/troubleshooting#when-i-set-the-text-i-see-the-previous-one-for-one-frame-before-showing-the-new-one)

This is probably due to the text being set to TMPro/UITK, and not Text Animator directly.

**Solution**: Please have a look at the [Setting up texts](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts)
 page for best practices and [Show and hide letters dynamically](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically)
!

**Workaround**: If the issue still persists, make sure that you clear text (e.g. on disable) before showing the new one.

I upgraded Unity version (2022.3->Unity 6.3) and there are some errors with Text Animator[](https://docs.febucci.com/text-animator-unity/other/troubleshooting#i-upgraded-unity-version-2022.3-greater-than-unity-6.3-and-there-are-some-errors-with-text-animator)

There might be some errors if you upgrade version between Unity 2022.3 and Unity 6.3, on the same project, and with Text Animator still inside. We actually have different versions of the package for different versions of Unity, so you need to switch the Text Animator version as well.

**Solution**: Simply remove the package (not the effects/data! only the scripts) and then import it again from the Package Manager.

I updated the asset from 2.X to 3.X and there are some errors[](https://docs.febucci.com/text-animator-unity/other/troubleshooting#i-updated-the-asset-from-2.x-to-3.x-and-there-are-some-errors)

Yes! Unfortunately that was expected (as we also wrote in the blog post, announcement and made a Major Upgrade in the asset store).

**Solution** (kinda): since version 3.0 brings some important changes, we do recommend that you stay at version 2.X for this project and switch to Text Animator 3.0 only on a new one. If you have written custom integrations/scripts on top, please have a look at the [Upgrading from 2.X to 3.X](https://docs.febucci.com/text-animator-unity/other/changelog/upgrading-from-2.x-to-3.x)
 for it!

The typewriter shows the entire text instantly[](https://docs.febucci.com/text-animator-unity/other/troubleshooting#the-typewriter-shows-the-entire-text-instantly)

**Solution**: Make sure to assign the timings scriptable object from the inspector/UI Builder! [Show and hide letters dynamically](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically)

### 

[](https://docs.febucci.com/text-animator-unity/other/troubleshooting#common-errors)

Common Errors

NullReferenceException: Object reference not set to an instance of an object TMPro.TMP\_Settings.get\_defaultStyleSheet[](https://docs.febucci.com/text-animator-unity/other/troubleshooting#nullreferenceexception-object-reference-not-set-to-an-instance-of-an-object-tmpro.tmp_settings.get_d)

Make sure you have imported TextMeshPro correctly and initialized the "essentials". Read more here [Setting up texts](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts)

### 

[](https://docs.febucci.com/text-animator-unity/other/troubleshooting#warnings)

Warnings

Camera Main Camera does not contain an additional camera data component. Open the Game Object in the inspector to add additional camera data.[](https://docs.febucci.com/text-animator-unity/other/troubleshooting#camera-main-camera-does-not-contain-an-additional-camera-data-component.-open-the-game-object-in-the)

This happens during example scenes if you have URP installed or similar, but the example scene doesn't. It's not an issue! Follow the warning instruction to add additional data, but the asset will work anyways!

### 

[](https://docs.febucci.com/text-animator-unity/other/troubleshooting#known-issues)

Known Issues

**We are working on a fix and will update the asset as soon as possible anyways**!

Please note that we do not officially support Unity Alpha and Beta versions! We have no way to know if Unity changed APIs etc. _the day_ they published the new alpha or beta, so we use these versions to test and make sure the asset works on release/production versions. Thanks!

If you have any other issue, please feel free to contact us here! We'll fix it ASAP:

[![Logo](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fframerusercontent.com%2Fimages%2Fy1LCEnd5hyGjuX0kKaGBUorzMc.png&width=20&dpr=4&quality=100&sign=82d1be32&sv=2)Support Requests | Text Animator for Unity, Godot and Unrealwww.textanimatorforgames.com](https://www.textanimatorforgames.com/support)

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Writing Custom Actions (C#) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-actions-c

Other than using [built-in actions](https://docs.febucci.com/text-animator-unity/typewriter/wait-actions-when-typing)
, you can write your own via script (C#).

Be sure to read the [Advanced Concepts](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts)
 page as well.

* * *

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-actions-c#actions-base-class)

Different ways to create custom actions


----------------------------------------------------------------------------------------------------------------------------------------------------------------

Since Text Animator 3.0 you can create actions in many different ways, giving you even more flexibility based on your projects needs.

### 

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-actions-c#creating-actions-as-components)

Creating Actions as Components

Actions created as Components allow you to reference scene objects more easily

Copy

    [System.Serializable]
    class ExampleActionComponent : TypewriterActionScriptable
    {
        [SerializeField] float timeToWait;
        
        // main logic here, 
        
        // ...either stateless
        protected override IActionState CreateCustomState(ActionMarker marker, object typewriter)
            => new ExampleState(timeToWait);
            
        // ...or as a Coroutine
        protected override IEnumerator PerformAction(TypingInfo typingInfo)
        {
            // yield return ...
        }
    }

### 

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-actions-c#creating-actions-as-scriptable-objects)

Creating Actions as Scriptable Objects

Actions as ScriptableObjects can be reused and referenced without the need for a scene loaded

Copy

    [System.Serializable]
    [CreateAssetMenu(menuName = "Create Example Action")]
    class ExampleActionScriptable : TypewriterActionScriptable
    {
        [SerializeField] float timeToWait;
        
        // main logic here...
        
        // ...either stateless
        protected override IActionState CreateCustomState(ActionMarker marker, object typewriter)
            => new ExampleState(timeToWait);
            
        // ...or as a Coroutine
        protected override IEnumerator PerformAction(TypingInfo typingInfo)
        {
            // yield return ...
        }
    }

P.S. Don’t forget to create your action ScriptableObject in the ProjectView, and add it to an actions Database.

* * *

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-actions-c#actions-base-class-1)

Different ways to implement the actions logic


------------------------------------------------------------------------------------------------------------------------------------------------------------------------

You can decide how to write the core logic of Actions.

*   Inside Coroutines (IEnumerator), or
    
*   Via a separate "tick" method (that returns if the action should keep running or if it has finished).
    

To start, import the correct namespaces:

Copy

    using Febucci.TextAnimatorForUnity.Actions;
    using Febucci.TextAnimatorCore.Typing;
    using UnityEngine;

### 

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-actions-c#actions-base-class-2)

Creating a coroutine

Writing a coroutine is pretty straightforward!

For example, inside your TypewriterAction class (whether it's a Component or a Scriptable), just override the PerformAction method:

Copy

    [SerializeField] AudioSource source;
    
    protected override IEnumerator PerformAction(TypingInfo typingInfo)
    {
        if (source != null && source.clip != null)
        {
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
        }
    }

### 

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-actions-c#actions-base-class-3)

Creating a stateless action

Creating a Stateless action on the other hand, requires you to create a custom struct that inherits from **IActionState** and that will perform the action (in this case: waiting a few seconds before progressing the typewriter), like:

Copy

    struct ExampleState : IActionState // <--- must inherit from this
    {
        float timePassed;
        readonly float timeToWait;
        public ExampleState(float timeToWait)
        {
            timePassed = 0;
            this.timeToWait = timeToWait;
        }
        
        public ActionStatus Progress(float deltaTime, ref TypingInfo typingInfo)
        {
            // increases time passed
            timePassed += deltaTime;
            
            // tells to continue or to stop based on time
            return timePassed >= timeToWait
                ? ActionStatus.Finished
                : ActionStatus.Running;
        }
        
        public void Cancel()
        {
            // use this for modifying 
        }
    }

You can then instantiate this struct by overriding the CreateCustomState method inside your Action class (the one we saw here [Different ways to create custom actions](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-actions-c#actions-base-class)
).

Copy

    protected override IActionState CreateCustomState(ActionMarker marker, object typewriter)
            => new ExampleState(timeToWait);

### 

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-actions-c#attributes)

Attributes

*   The `marker` paramater has useful info about your tag, for example the ID or if there are any parameters that come with it (e.g. `<playSound=02>`).
    
*   The `typewriter` references the Typewriter Component or AnimatedLabel that is currently performing the action
    
*   The `typingInfo` contains information such as the current typing speed (which you can modify) and time passed inside the typewriter.
    

* * *

Done! With this simple procedure, you can add any Custom Action you want.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Writing Custom Effects (C#) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-effects-c

Other than using [built-in effects](https://docs.febucci.com/text-animator-unity/effects/built-in-effects-list)
 or [creating custom ones from the Inspector](https://docs.febucci.com/text-animator-unity/customization/create-your-own-effects)
, **you can easily program custom Effects via C#**.

P.S. Make sure you have read the [Advanced Concepts](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts)
 page!

Effects have three key parts (which can be written in the same file).

**Parameters class/struct**

Contains information about the data/values you will use in your effect (**state)**

**State** struct

Main effect class. Given the parameters and a Character, modifies it through time. Also handles [Modifiers](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers)

**Scriptable Wrapper**

Unifies the previous elements together and lets you save things on disk. A few of line of code to let us do the rest!

These names are just a convention, but **you can call them however you prefer**!

Just know that you need:

*   Something that stores effect variables
    
*   A struct responsible for modifying letters
    
*   The Scriptable that glues these two togheter and lets you save the info on disk
    

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-effects-c#writing-your-custom-script)

Writing your Custom Script


-----------------------------------------------------------------------------------------------------------------------------------------------------------

For this example, we're making an effect that makes a character go up by a variable amount.

First, make sure to import the necessary namespaces (your IDE will tell you anyways <3)

Copy

    using UnityEngine;
    
    // import Text Animator's namespaces
    using Febucci.TextAnimatorCore;
    using Febucci.TextAnimatorCore.Text;
    using Febucci.Parsing;
    using Febucci.TextAnimatorForUnity.Effects;

### 

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-effects-c#parameters)

Parameters

Create the data you will need to modify the characters (it's the same one you will see and edit in the Inspector).

Copy

    // can be either struct or class
    // the latter allows you to have default values
    [System.Serializable]
    class CustomEffectParameters
    {
        public float amount = 1.5f;
    }

### 

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-effects-c#state)

State

The "core" part of an effect. Modifies the letter given the parameters and pre-calculated Text Animator data.

*   The struct must inherit from **IEffectState**.
    

Copy

    // must be struct!
    struct CustomEffectState : IEffectState
    {
        readonly float defaultAmount;
        float amount;
    
    
        public CustomEffectState(CustomEffectParameters data)
        {
            // gets the default amount from the parameters class
            this.defaultAmount = data.amount;
            this.amount = defaultAmount;
        }
    
        public void UpdateParameters(RegionParameters parameters)
        {
            // automatically handles cases where the user wrote 
            // modifiers in the rich text tag, "a" in this case
            // (e.g. <tagID a=5> will set "amount" to 5, while 
            // a*2 will make "amount" two times defaultAmount)
            amount = parameters.ModifyFloat("a", defaultAmount);
        }
    
        public void Apply(ref CharacterData character, in ManagedEffectContext context)
        {
            // uses "amount" to move the character up
            // with a clear and easy to use API
            character.MovePosition(
                Vector3.Up * amount * context.progressionRange * context.intensity,
                context.isUpPositive
                );
            // 1. note context.progressionRange -> it's the 
            //     curve you have assigned in the editor!
            //     allowing you for a step, a sine, bounce etc. result
            // 2. note also the context.intensity, needed to have 
            //     smooth transitions between stages.
            }
    }

### 

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-effects-c#scriptable-object-wrapper)

Scriptable Object Wrapper

Creates the logic necessary to hook your custom effect into Text Animator, also saving it in the Assets folder.

Copy

    [System.Serializable] // <-- make it serializable!!
    [CreateAssetMenu(fileName = "Your Custom Effect")]
    class CustomEffectScriptable : ManagedEffectScriptable<CustomEffectState, CustomEffectParameters>
    {
        // simply creates a new State, given the Parameters (already managed by text animator)
        protected override CustomEffectState CreateState(CustomEffectParameters parameters)
            => new CustomEffectState(parameters);
    }

There is another version of "ManagedEffectScriptable" which accepts more types, as well as the "Referenced" effect implementation, but we will cover than from future versions!

These scripts are all Text Animator needs to make sure you get:

*   Auto-managed curves, playbacks, modifiers
    
*   Optimized effects without race conditions
    
*   Compatible effects in AOT platforms (without the need to use Reflection)
    
*   Our powerful preview editor
    
*   Effects that work the same on UI Toolkit and Text Mesh Pro, including dynamic scaling
    

and more! <3

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FpaXEW0rl1anhoSBUK719%252FClipboard-20251116-235502-613.gif%3Falt%3Dmedia%26token%3D72885c85-f75f-43db-969c-ab4a72c28803&width=768&dpr=4&quality=100&sign=27c111d&sv=2)

* * *

Done! **You’ve completed all the steps necessary, yay!** The more effects you add, the more this process will sound familiar and simpler.

Remember to give your effect a tag (from the inspector) and to add it to the database! Otherwise it will not be recognized. You can read more here: [Effects Database](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/effects-database)

**Have fun applying your effects!**

* * *

A guide for creating "Referenced" effects is coming soon, as we're still tinkering the UX/API part.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Changelog | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/other/changelog

**P.S. Please always back up your projects (**_**or better: use version control**_**) before updating anything, even outside Text Animator. Cheers!**

* * *

[](https://docs.febucci.com/text-animator-unity/other/changelog#latest-releases)

Latest Releases


-----------------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/other/changelog#id-3.2.0-custom-rotation-pivots-and-bugfixes-2025.12.18)

3.2.0 - Custom Rotation Pivots and Bugfixes \[2025.12.18\]

#### 

[](https://docs.febucci.com/text-animator-unity/other/changelog#added)

Added

*   Added effects with custom pivot rotations
    
*   Reimplemented pendulum effect, both for appearances, presistent and disappearance
    
*   \[API\] Exposed a character's passed time in CharacterData
    

#### 

[](https://docs.febucci.com/text-animator-unity/other/changelog#bugfix)

Bugfix

*   Fixed an annoying bug that - for some users - was showing Text Animator's window after script reloads
    
*   Fixed typewriter skipping characters if the Unity Editor had a lag spike
    
*   Fixed error when the Unity Package manager was unable to find the package
    

* * *

### 

[](https://docs.febucci.com/text-animator-unity/other/changelog#id-3.1.1-yarn-spinner-typewriter-events-and-news-panel-2025.12.03)

3.1.1 - Yarn Spinner, Typewriter events and News panel \[2025.12.03\]

#### 

[](https://docs.febucci.com/text-animator-unity/other/changelog#added-1)

Added

*   Yarn Spinner is now officially integrated! (from version 3.1)
    
*   Added events in the inspector (Typewriter Component) when the typewriter has started and finished waiting for a character
    
*   News panel directly in the About Window, to keep tracks of new updates without having to leave the Editor
    

#### 

[](https://docs.febucci.com/text-animator-unity/other/changelog#bugfix-1)

Bugfix

*   Fixed "waitforinput" action was not correctly serialized in the builtin Actions Databse
    
*   Fixed typewriter not starting correctly if parent object was disabled
    

#### 

[](https://docs.febucci.com/text-animator-unity/other/changelog#minor-changes)

Minor Changes

*   Added Documentation and License links in the package.json
    
*   Added virtual methods to do something before or after the typewriter is waiting for a character
    
*   Added custom icon for Text Animator and Typewriter components
    

* * *

### 

[](https://docs.febucci.com/text-animator-unity/other/changelog#id-3.0.0-ui-toolkit-support-a-new-animation-engine-and-much-more-2025.11.18)

3.0.0 - UI Toolkit Support, a new Animation Engine and much more! \[2025.11.18\]

#### 

[](https://docs.febucci.com/text-animator-unity/other/changelog#added-2)

Added

*   UI Toolkit is now supported from Unity 6.3
    
*   You can play effects only once, make them loop x times, start delayed and many other combinations thanks to "Playbacks" (both from the editor or through rich text tags)
    
*   The same effect can now be played in all occasions as an Appearance, Persistent (previously "Behavior") and Disappearance, increasing the number of effects available even more (e.g. a "wave" as an appearance, with different parameters than the "wave" as a persistent/behavior).
    
*   You can use rich text tags modifiers to set an effect parameter directly, multiply it or set a specific keyword
    
*   You can now set different curves to an effect, changing different transitions and their movement/influence over time (e.g. make a rotation seem laggy, increasing step by step).
    
*   Added more options to already existing effects, like the "expanding" and "sliding" directions
    
*   Typewriter Actions can be created as "Components" (other than Scriptable Objects) allowing you to reference scene objects more easily
    
*   Typewriter Actions now support both Coroutines and also a Stateless "tick" progression
    
*   A new typewriter action called "PlaySound": plays and waits for an Audio Source to finish before progressing the typewriter
    
*   _Added many other little refinements, tooltips and much more._
    

_We'll probably discover we missed to write some features here and will update this page in the next weeks - there were_ _**hundreds**_ _of commits over the last months of development!_.

#### 

[](https://docs.febucci.com/text-animator-unity/other/changelog#improved)

**Improved**

*   Rewrote the entire documentation, hopefully making it even more easy to understand and covering extra best practices, tips and frequently asked questions
    
*   Improved the License for the asset, now even more accessible for both indies and bigger teams.
    
*   Optimized effects with 0 garbage collection during, and many other optimization considerations
    
*   Fixed race conditions between effects (happening in some extreme occasions)
    
*   Fixed race conditions between actions, also allowing you to specify local actions for specific typewriters
    
*   Improved UX for the Editor, as well as APIs.
    
*   Improved Welcome screen and Setup window, now doing some extra checks
    
*   You can now share settings between multiple typewriters and text animators.
    
*   Improved API to handle rich text tags parameters, now automatically handled by Text Animator
    
*   _Many bugfixes (like the new input system warnings) and more_.
    

#### 

[](https://docs.febucci.com/text-animator-unity/other/changelog#breaking-api)

Breaking API

*   Most API is breaking (as we changed Namespaces and some core architecture, especially if you wrote custom C# effects or actions). A lot needed to change to have this new version and to prepare for all the future plans we have in mind - so we did it all at once (including the license change) so that a) it's something you only have to think about once, and b) we can work on the new updates more easily without being stuck. Please do read [Upgrading from 2.X to 3.X](https://docs.febucci.com/text-animator-unity/other/changelog/upgrading-from-2.x-to-3.x)
    . Thanks!
    

* * *

[](https://docs.febucci.com/text-animator-unity/other/changelog#known-issues)

Known Issues


-----------------------------------------------------------------------------------------------

**We are working on a fix and will update the asset as soon as possible anyways**!

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Animator Settings | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/animator-settings

The "Animator Settings" ([whether local or global](https://docs.febucci.com/text-animator-unity/quick-start/core-concepts#settings-accessibility)
) contain many options about how effects are applied and presented.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FfTe0N94riod0U2VKoRVi%252FScreenshot%25202025-11-15%2520alle%252018.39.36.png%3Falt%3Dmedia%26token%3D5e297e45-eb51-4eb9-9694-9c2028a893b8&width=768&dpr=4&quality=100&sign=dd38c32b&sv=2)

They should be self explanatory (and we're more adding tooltips from the next versions as well!), but here are some extra details about some options:

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/animator-settings#default-effects-mode)

Default Effects Mode

If you have set up at least one "Default Tag" element in any of the arrays below, the "Default Effects Mode" lets you decide how these tags are applied to letter.

*   **Fallback**: these tags will be applied if there are no other effects/tag already affecting that letter
    
*   **Constant**: these tags will be applied to _all_ text (if there are other effects, they will be stacked on top)
    

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/animator-settings#time-scale)

Time Scale

You can change effects `TimeScale` mode in the “TextAnimator” component inspector.

*   **Scaled**: effects will slow down / pause based on the game Time.timeScale ([Unity Reference](https://docs.unity3d.com/ScriptReference/Time-timeScale.html)
    )
    
*   **Unscaled**: effects will update even if the game is paused (Time.timeScale = 0), using an unscaled/independent time instead.
    

If you have the typewriter enabled, its timescale **will match the relative TextAnimator’s timescale** (which means that you can also show letters when the game is paused if you set it to “unscaled”).

If the game timescale is negative TextAnimator will act like it’s paused, but it will automatically resume once it’s greater than zero.

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/animator-settings#dynamic-scaling)

Dynamic Scaling

Text Animator achieves uniform effects result on different screen resolutions by default, and it is recommended that you keep this feature enabled.

Explanation[](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/animator-settings#explanation)

Your players will most likely have different screen sizes (their devices, from mobile to monitors etc.) which means that moving a letter of "50 pixels" might seem too much or too low, while as a designer you'll want an uniform experience/result for everyone, exactly as you intended. This is why we strongly advise to keep "Use Dynamic Scaling" enabled, and edit values based on your current computer font size (so whatever changes later, it'll keep the same uniform ratio).

*   `Reference Font Size`: represents the size where objects behave as expected. As a reference, you can pick the font size in your unity editor while you’re testing things.
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FK4qC74LIOHiJjZWbZrCl%2Ftextanimator%2520unity%2520dynamic%2520scaling.png&width=768&dpr=4&quality=100&sign=931e1049&sv=2)

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/welcome

**Welcome** to the documentation of **Text Animator for Unity 3.X**! We can't wait to have you animate your texts and get familiar with the plugin.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F74b3Q86Y180EtGnu7Jg5%252FGames%2520Using%2520Text%2520Animator.png%3Falt%3Dmedia%26token%3D9912a72f-fab2-4003-b8f7-3499fc676a33&width=768&dpr=4&quality=100&sign=91cad09e&sv=2)

We're writing a documentation that's as short and concise as possible, but that is also able to help you when you need it the most. **Please make sure to read the first and most important pages**! They take a few minutes now but will save _a lot of time_ later.

Useful links:

[Purchase](https://www.textanimatorforgames.com/unity#pricing)
 [Website](https://www.textanimatorforgames.com/unity)

#### 

[](https://docs.febucci.com/text-animator-unity/welcome#good-to-know)

Good to know

*   This documentation is available in different languages: English, Chinese, Korean, Japanese.
    
*   You can browse different versions and languages of this documentation at the top of this page.
    
*   Text Animator is also available in other engines. [Learn more here](https://www.textanimatorforgames.com/)
    .
    

And if you need any help at any time, feel free to visit the [troubleshooting page](https://docs.febucci.com/text-animator-unity/other/troubleshooting)
 (common issues and how to fix them) or the support page!

[![Logo](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fframerusercontent.com%2Fimages%2Fy1LCEnd5hyGjuX0kKaGBUorzMc.png&width=20&dpr=4&quality=100&sign=82d1be32&sv=2)Support Requests | Text Animator for Unity, Godot and Unrealwww.textanimatorforgames.com](https://www.textanimatorforgames.com/support)

#### 

[](https://docs.febucci.com/text-animator-unity/welcome#requirements)

Requirements

Please visit the [Requirements & Limitations](https://docs.febucci.com/text-animator-unity/welcome/requirements-and-limitations)
 page before purchasing or importing, and we also replied many [frequently asked questions here](https://docs.febucci.com/text-animator-unity/welcome/faq)
. Thanks!

* * *

**Have fun** and don't forget to join our [Discord](https://discord.com/invite/j4pySDa5rU)
 to join the conversation and show what you've been creating!

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Effects Database | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/effects-database

Effects are stored into Databases, which are a ScriptableObject on their own as well.

You can add and remove effects to any Database however you prefer, and you can have multiple TextAnimators share the same Databases too. By default, all TextAnimator’s will share the same “Default” Databases from the [Global Settings](https://docs.febucci.com/text-animator-unity/customization/global-settings)
 file.

**Text Animator needs an effect database to know which effects exist**, so make sure you have one!

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FVSXvT5lT5dntsMxKpb75%252FScreenshot%25202025-11-15%2520alle%252020.15.21.png%3Falt%3Dmedia%26token%3D3b2e7fdd-86fb-4193-9b33-6312916accc6&width=768&dpr=4&quality=100&sign=316e96f1&sv=2)

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/effects-database#creating-custom-databases)

Creating custom Databases

You can create new effects by right-clicking in the Project View -> Create -> Text Animator for Unity, and then choose the category and effect you want to add.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FyeZq580N8EGFfDW3tkwI%252FScreenshot%25202025-11-15%2520alle%252020.14.26.png%3Falt%3Dmedia%26token%3Df232bbae-c247-411f-ab0e-48bbc1ea1a42&width=768&dpr=4&quality=100&sign=f3f0eeec&sv=2)

Since you can also edit effect tags in each ScriptableObject, you could create different ones for different purposes, like a specific “shake” effect that applies when a dialogue has to communicate “cold”, and another when it should communicate “fear”.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Modifiers | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers

**Modifiers** **let you change the characteristics of your effects individually**, without having to create new tags or scriptables for every variation.

`“I was <wiggle>strong</wiggle>… but now I’m<wiggle a*3> three times stronger</wiggle>!!!”`

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FgsZWN78ej2eibo2lDykv%2Ftext-animator-modifier-example-ezgif.com-video-to-gif-converter.gif&width=300&dpr=4&quality=100&sign=edced766&sv=2)

You can read a list of all the available modifiers for each effect here: [Built-in effects list](https://docs.febucci.com/text-animator-unity/effects/built-in-effects-list)

* * *

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers#values)

Values

To modify effect **values** (like an amplitude or speed), write their **information inside the effect tag itself**.

#### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers#multiply)

Multiply

Pattern: `<effectId` `**parameter*value**``>`

The `*****` symbol tells the code **to** **multiply** a **float parameter by that value**. This way you can easily know how much stronger/weaker a modified effect will result compared to the base one _(for this reason, a modifier of “1” will return the same result of a base value)_.

Example[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers#example)

*   Make the `amplitude` of a "wave" effect three times stronger: `<wave a*3>`
    
*   Make a "rainbow" effect two times slower `<rainb a*0.5>`
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FaYNNPUoPShZQvpOqk37r%252FMultiply_Example_TAnim3.0-ezgif.com-video-to-gif-converter.gif%3Falt%3Dmedia%26token%3D6c9e5567-9463-4a0f-8565-f99712390eb7&width=768&dpr=4&quality=100&sign=44f54b27&sv=2)

wiggle five times stronger

The `*****` symbol is only available for numbers. For strings, please use `**=**`

#### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers#set)

Set

Pattern: `<effectId` `**parameter=value**``>`

The `**=**` symbol tells the code **to set a** parameter **value directly**. Especially useful when you need a precise movement/result in your text region, or if you are setting strings.

Example: writing "<wave a=5>" is the same as going in the inspector and setting the wave amplitude to 5! (with the benefit, of course, that "<wave a=5>" modifier uses that value only inside the text region you have set up, and will restore to default once you close the tag.)

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers#keywords)

Keywords

**Some** effects **settings** can also be modified via a **single word**, without having to write any value next to it.

Pattern: `<effectId` `**keyword**``>`

Example: playing an effect just once using [Playbacks](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/playbacks)
, writing **<wave once>**

* * *

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers#tips-and-best-practices)

Tips and Best Practices

*   You can use multiple modifiers on the same effect tag!
    

*   From the inspector, set up your effects as their "default state" / neutral tone. This way it will become easier to modify parameters when you write dialogues, without having to remember precise values for each. Once you have set up a neutral "shake", it will become easier when writing to know that "<shake a=2>" will make it as twice as stronger (e.g. useful to make someone angry!)
    

Additionally:

*   👍 You can use modifiers when declaring “[default/fallback](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects#set-default-effects-to-the-entire-text)
    ” effects as well (simply write them in the Inspector directly).
    
*   ❗ Be sure to remove spaces between the modifierID, the ‘=’ symbol and its value
    
    *   ❌ Wrong: `<wiggle f = 3>`
        
    *   ✅ Correct: `<wiggle f=3>`
        
    
*   ⚠️ If you write identical attributes in the same rich text tag, only the last one will take effect.
    
    Writing "<wiggle `**a=2**` `**a=5**`\>" is the same as writing "<wiggle `**a=5**`\>", since the first '`**a**`' parameter will be discarded / overwritten by the second.
    

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Install and Quick Start | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/quick-start

Using the asset is a matter of a few clicks (import -> add components -> press play), but to better understand everything please have a look at the following pages, so that you can start even faster and in the right direction.

[](https://docs.febucci.com/text-animator-unity/quick-start#how-to-implement-text-animator)

1\. Import Text Animator for Unity


-----------------------------------------------------------------------------------------------------------------------------------

As the very first step, you need to import Text Animator for Unity in your project.

#### 

[](https://docs.febucci.com/text-animator-unity/quick-start#compatibility-check)

Compatibility Check

**The asset works with the following UI and Unity versions**:

*   **Text Mesh Pro** (Unity 2022.3 and up)
    
*   **UI Toolkit** (Unity 6.3 and up).
    

It also supports the new Unity input system (and the legacy one, too).

#### 

[](https://docs.febucci.com/text-animator-unity/quick-start#import-the-package)

Import the Package

Once your project is set up correctly, you can import Text Animator from the Package Manager (Asset Store tab).

Make sure to include the "Samples/BuiltIn" folder, or the asset might not work.

After a succesful installation, the **welcome window** will show up and Text Animator is ready to animate your texts!

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252Fo6lFhmxUPaki6oAtVVXZ%252FScreenshot%25202025-11-15%2520alle%252017.40.31.png%3Falt%3Dmedia%26token%3D729acbd3-556d-4808-9726-7f3918afec84&width=768&dpr=4&quality=100&sign=6e6af103&sv=2)

A part of the Welcome Window, which shows after import

In case the about window doesn't show up, or if you want to seet it later, you can access it anytime from the Menu at Tools/Febucci/TextAnimator/About Window!

[](https://docs.febucci.com/text-animator-unity/quick-start#id-2.-example-scenes)

2\. Example Scenes


---------------------------------------------------------------------------------------------------------

You can learn about most Text Animator features directly from the inspector, and see how we've set up things and their direct result from the example scenes.

Start from the scene called "**00 - Welcome**", or click "Get Started" on Text Animator's welcome window.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FLgTsSXatpKk3M2Nr36VN%252FScreenshot%25202025-11-15%2520alle%252017.45.47.png%3Falt%3Dmedia%26token%3D624c13da-2e67-4653-9caa-076cc5cfa24d&width=768&dpr=4&quality=100&sign=5af94292&sv=2)

To access the example scenes, make sure you have imported them! You can safely remove/delete them once you don't need them anymore, too.

[](https://docs.febucci.com/text-animator-unity/quick-start#animating-your-first-texts)

3\. Animating your first texts


---------------------------------------------------------------------------------------------------------------------------

You can get your texts running in a few clicks!

UI Toolkit

Text Mesh Pro

_P.S. Assuming you already know_ [_how to use UI Toolkit_](https://docs.unity3d.com/Documentation/Manual/UIElements.html)
 _and what it does._

#### 

[](https://docs.febucci.com/text-animator-unity/quick-start#from-the-ui-builder)

From the UI Builder

*   Go to Library -> Project
    
*   Drag "AnimatedLabel" from "Custom Controls/Febucci/Text Animator for Unity" in your hierarchy!
    

We are working to make sure you can animate built in Labels and Buttons from UI toolkit directly! _(Unity 6.3 and up.)_ Stay updated!

Your .uxml should look like this:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZNwCUmAugxLNcVVO9oBk%252FScreenshot%25202025-11-15%2520alle%252018.02.51.png%3Falt%3Dmedia%26token%3Dced34791-d558-4883-b646-2197664dd637&width=768&dpr=4&quality=100&sign=74a39d74&sv=2)

#### 

[](https://docs.febucci.com/text-animator-unity/quick-start#via-code)

Via Code

You can create an instance of the "Febucci.TextAnimatorForUnity.AnimatedLabel" class and add it to your UI document, like this:

Copy

    using UnityEngine;
    using UnityEngine.UIElements;
    using Febucci.TextAnimatorForUnity; // <- import Text Animator's namespace
    
    public class ExampleScript : MonoBehaviour
    { 
        [SerializeField] UIDocument document;
    
        void Start()
        {
            var container = document.rootVisualElement.contentContainer;
            var animatedLabel = new AnimatedLabel(); // <- create an animated label
            container.Add(animatedLabel); // <- add it to the content container
            // [..]
            animatedLabel.SetText("<wave>hello"); // <- set the text
        }
    }

_P.S. Assuming you already know_ [_how to use Text Mesh Pro_](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html)
 _and how it works._

Add a Text Animator - Text Mesh Pro component on the same GameObject that has a TextMeshPro component (either UI or world space!):

Your inspector should look like this:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FT3h66pIPFdakGOCfToEY%252FScreenshot%25202025-11-15%2520alle%252017.59.18.png%3Falt%3Dmedia%26token%3D26196c49-f0f5-457b-85dd-da358f43c823&width=768&dpr=4&quality=100&sign=38546e2c&sv=2)

You can read [Setting up texts](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts)
 for more details and suggestions!

### 

[](https://docs.febucci.com/text-animator-unity/quick-start#id-1-writing-effects-in-your-text)

Writing effects in your text

One way to adding effects in your text is using rich text tags, like this: “`I'm <shake>freezing</shake>`”, where "shake" is an ID for a built-in effect.

*   Try writing a text by experimenting with the following tags: `<wiggle>` `<shake>` `<wave>` `<bounce>`, like “`<wiggle>I'm joking</wiggle> hehe now <shake>I'm scared</shake>`”, then enter Unity’s Play mode.
    

Your text is animating letters based on the effects you’ve written!

* * *

Have fun animating your texts! You can proceed to the next page for a more in-depth look on all the asset's features.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Setting up texts | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/effects

You can set texts to Text Animator from two different UI systems:

*   [UI Toolkit](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#ui-toolkit)
    
*   [Text Mesh Pro](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#text-mesh-pro)
    

This page contains some information already present in [Install and Quick Start](https://docs.febucci.com/text-animator-unity/quick-start/install-and-quick-start)
, but also other details and suggestions for each system and in general. Make sure to read the [Best Practices](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts#best-practices)
 section!

* * *

[](https://docs.febucci.com/text-animator-unity/effects#ui-toolkit)

UI Toolkit


-----------------------------------------------------------------------------------

_P.S. Assuming you already know_ [_how to use UI Toolkit_](https://docs.unity3d.com/Documentation/Manual/UIElements.html)
 _and what it does._

#### 

[](https://docs.febucci.com/text-animator-unity/effects#from-the-ui-builder)

From the UI Builder

*   Go to Library -> Project
    
*   Drag "AnimatedLabel" from "Custom Controls/Febucci/Text Animator for Unity" in your hierarchy!
    

We are working to make sure you can animate built in Labels and Buttons from UI toolkit directly! _(Unity 6.3 and up.)_ Stay updated!

Your .uxml should look like this:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZNwCUmAugxLNcVVO9oBk%252FScreenshot%25202025-11-15%2520alle%252018.02.51.png%3Falt%3Dmedia%26token%3Dced34791-d558-4883-b646-2197664dd637&width=768&dpr=4&quality=100&sign=74a39d74&sv=2)

#### 

[](https://docs.febucci.com/text-animator-unity/effects#via-code)

Via Code

You can create an instance of the "Febucci.TextAnimatorForUnity.AnimatedLabel" class and add it to your UI document, like this:

Copy

    using UnityEngine;
    using UnityEngine.UIElements;
    using Febucci.TextAnimatorForUnity; // <- import Text Animator's namespace
    
    public class ExampleScript : MonoBehaviour
    { 
        [SerializeField] UIDocument document;
    
        void Start()
        {
            var container = document.rootVisualElement.contentContainer;
            var animatedLabel = new AnimatedLabel(); // <- create an animated label
            container.Add(animatedLabel); // <- add it to the content container
            // [..]
            animatedLabel.SetText("<wave>hello"); // <- set the text
        }
    }

That's all!! You are ready for [How to add effects](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects)

* * *

[](https://docs.febucci.com/text-animator-unity/effects#text-mesh-pro)

Text Mesh Pro


-----------------------------------------------------------------------------------------

_P.S. Assuming you already know_ [_how to use Text Mesh Pro_](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html)
 _and how it works._

Add a Text Animator - Text Mesh Pro component on the same GameObject that has a TextMeshPro component (either UI or world space!):

Your inspector should look like this:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FT3h66pIPFdakGOCfToEY%252FScreenshot%25202025-11-15%2520alle%252017.59.18.png%3Falt%3Dmedia%26token%3D26196c49-f0f5-457b-85dd-da358f43c823&width=768&dpr=4&quality=100&sign=38546e2c&sv=2)

That's all!! You are ready for [How to add effects](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects)

If you're seeing empty texts (but have set them in the component), make sure that you have clicked at least once on a TextMeshPro component and imported the "Essentials" (once their window will pop up and ask you to do so).

#### 

[](https://docs.febucci.com/text-animator-unity/effects#best-practices-for-setting-text-via-code)

Best Practices for setting text via code

To set the text to your TextMeshPro object via code, please reference Text Animator's script instead of TMPro, like the following:

Copy

    using UnityEngine;
    using TMPro; 
    using Febucci.TextAnimatorForUnity.TextMeshPro; // <- import Text Animator's namespace
    
    public class ExampleScript : MonoBehaviour
    {
        [SerializeField] TMP_Text textMeshPro;
        [SerializeField] TextAnimator_TMP textAnimator;
    
        void Start()
        {
            // 🚫 Don't: set text through TMPro
            textMeshPro.SetText("<wave>hello");
    
            // ✅ Do: set text through Text Animator directly
            textAnimator.SetText("<wave>hello");
        }
    
    }

P.S. Referencing TMPro will work anyways, but setting the text with TextAnimator is better integrated as we have more control on the text.

* * *

[](https://docs.febucci.com/text-animator-unity/effects#best-practices)

Best Practices


-------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/effects#set-the-entire-text-dialogue-only-once)

Set the entire text/dialogue only once

Please try to set text just once, and use the typewriter / visibility methods to control how it appears.

If you really need to append text later in time, you can use the "textAnimator.AppendText" method.

Example[](https://docs.febucci.com/text-animator-unity/effects#example)

If you have a character that says "Helloooo how are you doing?", and you want to display it letter by letter, simply do: `typewriter.ShowText("Hellooooo how are you doing?");` and that's it! [Show and hide letters dynamically](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically)

* * *

If you're building a dynamic string, you can still do that before setting its value to the typewriter/animator.

Copy

    int apples = 5; //later taken from the game state
    string playerName = "Bob";
    
    // build the entire dialogue line first
    string dialogue = $"Hello {playerName}, you've got {apples} apples";
    
    // then set the text once
    typewriter.ShowText(dialogue);

(If you're using a Dialogue System, they'll do this for you - no worries ! [Integrations](https://docs.febucci.com/text-animator-unity/integrations/integrated-plugins-and-dialogues-systems)
)

Why should I set the entire text once, instead of character by character?[](https://docs.febucci.com/text-animator-unity/effects#why-should-i-set-the-entire-text-once-instead-of-character-by-character)

Performance! (Even if you didn't have Text Animator.)

Every time you set the text, TextMeshPro or UI toolkit need to calculate its mesh, positioning etc., and Text Animator has then to re-calculate character durations and more. This means that if you change it multiple times per second (e.g. adding more letters), you're doing these calculations every time.

To display characters one by one, you can simply set the full text once, and then start the typewriter: [Show and hide letters dynamically](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically)

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Playbacks | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/playbacks

**Playbacks decide how an effect gets applied through time** (for example, playing an effect only once).

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/playbacks#built-in-playbacks)

Built In Playbacks

You can use the following built-in playbacks to modify your effects, either [assigning them from the inspector](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects)
 or setting them as [modifier keywords:](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers#keywords)

For example, if you already have a "wave" effect that loops infinitely, but in one occasion you want to show it only once, you can write "<wave **once**\>", where "once" is the ID of the playback.

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/playbacks#create-custom-playbacks)

Create Custom Playbacks

There are currently three different types of playback classes that you can instantiate:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252Fzupt163LqdAGyPlxMi76%252FScreenshot%25202025-11-15%2520alle%252019.55.30.png%3Falt%3Dmedia%26token%3Dda819d95-9fd3-4d59-aa59-33d5a98b9717&width=768&dpr=4&quality=100&sign=63eb2b86&sv=2)

Each playback has different parameters that you can modify (e.g. duration).

If any parameter is less or equal to 0, then it will be ignored and the animation engine will jump to the "next" important/related one.

Appearances and disapperances need at least one value to be greater than 0, or their duration will be invalid and they will be skipped

#### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/playbacks#simple)

Simple

Parameter

Description

Delay before start

How much time the animation engine waits before starting to show the effect

Fade duration

How much time it takes for an effect to go from 0 to 1

Still duration

How much time an effect is displayed on screen.

#### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/playbacks#weighted)

Weighted

Parameter

Description

Intensity01

Lets you control externally how much intense should an effect be (e.g. 1 when near a game objective, 0 when too far)

#### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/playbacks#cycle)

Cycle

Parameter

Description

Delay before start

How much time the animation engine waits before starting to show the effect

Fade in duration

How much time it takes for an effect to go from 0 to 1

Still duration

How much time an effect is displayed on screen.

Fade out duration

How much time it takes for an effect to go from 1 to 0

Cycles

How many times this loop is repeated

Delay Between Cycles

How much time to wait before starting a new cycle

* * *

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/playbacks#playbacks-databases)

Playbacks Databases

As always, you can store playbacks inside a **database** and assign it to the [Global Settings](https://docs.febucci.com/text-animator-unity/customization/global-settings)
 (p.s. there is already one built-in and already set up), like the following:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FGjKdZv4dnZ0IOL9ZOFW2%252FScreenshot%25202025-11-15%2520alle%252019.59.17.png%3Falt%3Dmedia%26token%3Dc03fbe88-b9cc-4d41-8d98-dd8c4ee8e92b&width=768&dpr=4&quality=100&sign=a41b83ab&sv=2)

This way you can access all playbacks from all different Text Animator components, and modify your effects individually through [Modifiers](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers)
 (example "`<wave once>`".

If you are creating a new playback, make sure that it is stored inside that main/global database

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Phases | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/phases

A "**phase**" decribes how the effect varies between letters.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252Fe0rVNhfYkoaST18lc2so%252FClipboard-20251116-152040-561.gif%3Falt%3Dmedia%26token%3Dae40450e-cf37-4859-9e27-7b05a986a44d&width=768&dpr=4&quality=100&sign=3601a642&sv=2)

You can modify an effect phase either through the inspector, or via rich text tags [modifiers](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers)
.

**Char Offset**

Time variation between letters

ModifierID

i

**Word Offset**

Time variation between words

ModifierID

w

**Speed**

Effect speed (also affects [Playbacks](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/playbacks)
)

ModifierID

s

Example for rich text tags:

*   Make an effect twice as fast: "<wave s=2\>"
    
*   Modify offsets: "<wave i=.1 w=.3\>" (will set char offset as 0.1, and word as 0.3)
    

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/phases#extra-notes-about-offsets)

Extra notes about offsets

*   An offset of 0 or of 1 means that the effect is the same on all character
    
*   If you go from 0 to 0.5, the effect is shifted in one direction, while from 1 to 0.5 is shifted in the opposite direction (where 0.5 is higher)
    
*   An offset of 0.5 means that one character is in one direction, and another is in the opposite one
    

* * *

Last updated 1 month ago

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Curves | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/curves

Effects modify a letter states (position, rotation, scale, ...) based on a "state **curve"**, which you can assign in the inspector.

As always, **curves** are a Scriptable Object and clicking on them will show you their preview in the inspector.

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/curves#built-in-curves)

Built-in Curves

**Sine**

Follows the Sine curve (and eases in during appearances)

**Linear**

Goes linearly from 0 to 1

**Hold**

Stays at 1, always

**Square**

Either 1 or -1

**Step**

Goes from 0 to 1 in four different steps

**Bounce**

Bounces from 0 to 1

### 

[](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/curves#creating-custom-curves-from-the-inspector)

Creating Custom Curves from the inspector

To create a custom curve from the inspector go to Project->Create->Text Animator for Unity and then select "**Custom**".

You will find an inspector with two curves, both editable at the bottom of the panel.

*   **Curve01** goes from 0 to 1 and decides how Appearances and Disappearances behave
    
*   **CurveRange** goes from -1 to 1 (but ends where it started so that it forms a smooth/seamless loop) and affects Persistent effects
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZv0T9uTeTrdB1zcBiDNj%252FScreenshot%25202025-11-16%2520alle%252017.04.29.png%3Falt%3Dmedia%26token%3Dd2fc3da7-4456-4cd3-b724-ecf9910219a4&width=768&dpr=4&quality=100&sign=7544b967&sv=2)

* * *

A way to set curves via [Modifiers](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers)
 (similar to [Playbacks](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/playbacks)
) is coming in future releases!

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Direct Effects | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/customization/create-your-own-effects/direct-effects

**Direct effects modify a visual property of a letter**, whether it's their position, color, scale and more.

**Color**

Modifies the color of a letter, allowing you to decide if it should affect only the alpha, the RGB or both.

**Continuous Rotation**

Modifies the rotation of a character, from oscillating back and forth to doing a full loop

**Scale**

Multiplies the scale of a character. A scale of 1 will do nothing!

**Position**

Changes the position of a character over time. Also allows for 3D (Z pos)

**Shear**

Distorts (or "skews") the character from different pivots.

**Expand**

Expands the character sides from different directions.

**Random Position**

Moves the character towards a random direction, generated at runtime.

**Rainbow**

Changes the character's color to a rainbow effect, cycling over time.

The [built in effects](https://docs.febucci.com/text-animator-unity/effects/built-in-effects-list)
 you find in the asset are a mix of these direct effects! We decided that a "random position" effect could be called "**wiggle**" and put it in the default folder, as well as the "shake" one (which is a wiggle with different [Curves](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/curves)
) , but at the end of the day it's up to you! Have fun!! 🎉

Last updated 1 month ago

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Show and hide letters dynamically | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/typewriter

**You can use a Typewriter to show and hide letters dynamically**, choosing different pauses for any kind of characters (punctuation, letters, \[…\]), trigger events and more.

* * *

[](https://docs.febucci.com/text-animator-unity/typewriter#showing-text)

Showing Text


------------------------------------------------------------------------------------------

The typewriter contains general settings and events listeners, and allows for different pauses/timing modes:

*   **By Character**: shows one letter after the other.
    
*   **By Word**: progresses text word after word.
    

This new architecture (from 3.0) allows you to change typewriter timings during development (for whatever reason) while keeping event references and settings intact! <3

**Your typewriter should look like this:**

Text Mesh Pro

UI Toolkit

From the TypewriterComponent in the Inspector:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F4QBGWiDjjQq4LXVbhNfc%252FScreenshot%25202025-11-16%2520alle%252018.15.08.png%3Falt%3Dmedia%26token%3Daeb76665-1ea4-498e-9181-091ddf322063&width=768&dpr=4&quality=100&sign=3832d3ba&sv=2)

From the AnimatedLabel in the the UI Builder:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FB0i06unOYgu5XhHcdZN8%252FScreenshot%25202025-11-16%2520alle%252018.21.21.png%3Falt%3Dmedia%26token%3Dcf1193d6-cac5-47d4-93bf-b89a121f0046&width=768&dpr=4&quality=100&sign=5c5442b9&sv=2)

Make sure to assign the timings scriptable object, or the typewriter will show the entire text instantly!

* * *

You can start the typewriter in two main ways:

### 

[](https://docs.febucci.com/text-animator-unity/typewriter#a-via-code-recommended)

A) Via Code (Recommended)

If you want to use the typewriter, **it is recommended that you set the text directly to that component via code.**

Text Mesh Pro

UI Toolkit

If you are using TextMeshPro, please replace scripts that reference TMPro or Text Animator ([Setting up texts](https://docs.febucci.com/text-animator-unity/effects/setting-up-texts)
) and reference `Febucci.TextAnimatorForUnity.TypewriterComponent` instead.

*   ❌ Don't: “`tmproText.text = textValue;`” , or "`textAnimator.SetText(textValue);`"
    
*   ✅ Do: `typewriter.ShowText(textValue);`
    

Via UI Toolkit, the `AnimatedLabel` already has a "`Typewriter`" value that you can interact with! You don't need to do anything else, except for making sure you have assigned typewriter delays.

### 

[](https://docs.febucci.com/text-animator-unity/typewriter#b-via-the-easy-integration)

B) Automatic recognition

In case you haven't followed the step above, TextAnimator will still try to start the typewriter automatically if you have added a Typewriter component or have set up "Timings" through the AnimatedLabel in UI Toolkit.

Easy Integration might happen one frame behind (as it has to discover that something has changed first, which usually was done on the previous frame, and then start the typewriter). If this is an issue, either follow the step [A) Via Code (Recommended)](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#a-via-code-recommended)
, or see [When I set the text, I see the previous one for one frame before showing the new one](https://docs.febucci.com/text-animator-unity/other/troubleshooting#when-i-set-the-text-i-see-the-previous-one-for-one-frame-before-showing-the-new-one)

* * *

[](https://docs.febucci.com/text-animator-unity/typewriter#controlling-letters)

Controlling Letters


--------------------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/typewriter#start-and-stop-typing)

Start and Stop Typing

Inside the component’s Inspector you’ll find some options to control how the typewriter start should be triggered:

*   `Start Typewriter Mode`: tells the typewriter when to start showing letters.
    

Value

Explanation

**From Script Only**

The typerwiter can only be started by invoking [TextAnimatorPlayer.StartShowingText()](https://www.api.febucci.com/tools/text-animator-unity/api/Febucci.UI.Core.TypewriterCore.html#Febucci_UI_Core_TypewriterCore_ShowText_System_String_)

**OnEnable**

The typewriter starts every time the gameObject is set active

**OnShowText**

The typewriter starts as soon as a new text is set ([as explained in the “Showing Text” section](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically#showing-text)
)

**Automatically From All Events**

All of the above

*   `Reset Typing Speed At Startup`: true if you want the typewriter’s speed to reset back to 1 every time a new text is show, otherwise it will save the last one used.
    

You can pause the typewriter at any time by invoking `typewriter.StopShowingText()`, and you can start/resume it by invoking `typewriter.StartShowingText()`.

### 

[](https://docs.febucci.com/text-animator-unity/typewriter#skip)

Skip the Entire Text

To Skip the entire typewriter, you can invoke the `typewriter.SkipTypewriter()` method.

You can also find a few options to control how it behaves:

*   `Hide Appearances On Skip`: true if you want to prevent appearance effects from playing whenever the typewriter skips (meaning that the text will be shown instantly).
    
*   `Trigger Events On Skip`: true if you want to trigger all remaining events once the typewriter skips (be careful with that if you’re running some game logic with them, as everything will be run at once). Read more about events here: [Trigger Events when typing](https://docs.febucci.com/text-animator-unity/typewriter/trigger-events-when-typing)
    

### 

[](https://docs.febucci.com/text-animator-unity/typewriter#skip-specific-parts-of-the-text)

Skip Specific Parts of the Text

This feature is under testing for 3.0 and will be restored from the next version very soon! Thanks for your understanding!

### 

[](https://docs.febucci.com/text-animator-unity/typewriter#hiding-text)

Hiding Text

You can hide letters dynamically via script, by invoking `typewriter.StartDisappearingText()`, and you can also stop it at any time by invoking `typewriter.StopDisappearingText()`.

* * *

You can create your own timing waits (read [here](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-typing-waits-c)
 how via C#) or you can use the built-in ones.

[](https://docs.febucci.com/text-animator-unity/typewriter#options)

Options


--------------------------------------------------------------------------------

Typewriters might share the same settings and also have specific ones, so be sure to hover the mouse cursor above its fields in the Inspector to show the tooltips for each one.

Here is a quick overview of the most important/common ones:

### 

[](https://docs.febucci.com/text-animator-unity/typewriter#callbacks-unity-events)

Callbacks (Unity Events)

You can use Unity Events that will be triggered based on the typewriter activity (example: when it just ended showing text).

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FWHU9EqhOj3uN5AI3PURA%252FScreenshot%25202025-11-16%2520alle%252018.34.38.png%3Falt%3Dmedia%26token%3D7757f0b7-300e-4637-8495-542fab1b0fe5&width=768&dpr=4&quality=100&sign=a793fbd3&sv=2)

Event

Explanation

`OnTextShowed`

Event called after the entire text has been shown (_if you’ve set “Use Typewriter” to true, it will wait until all letters are shown_)

`OnTextDisappeared`

Called as soon as the script starts hiding the last letter

The following below only work if the “**use typewriter**” is set to **true**:

Event

Explanation

`OnTypewriterStart`

Called right before the typewriter starts showing its first letter. It doesn’t work if the typewriter is off, since it would coincide with the “OnTextShowed” event _(in this case you can use that one instead)_

`OnCharacterVisible(Char)`

Called each time a character became visible

`OnMessage(EventMarker)`

Invoked every time the typewriter meets a message/event in text. Read more about events [here](https://docs.febucci.com/text-animator-unity/typewriter/trigger-events-when-typing)

A typewriter uses its linked Text Animator **Time Scale** to progress time (you can read more here: [Time Scale](https://docs.febucci.com/text-animator-unity/effects/how-to-add-effects/animator-settings#time-scale)
), meaning that if the time is set to "Unscaled", then the typewriter will progress even if your game is paused.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Integrated Plugins & Dialogues Systems | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/integrations

We are re-writing all the integration pages to make sure Text Animator 3.0 is up to date with all the previous 3rd party assets:

*   Dialogue System for Unity
    
*   Ink
    
*   Game Creator 2
    
*   Unity Localization Package
    
*   Unity Visual Scripting
    
*   Playmaker
    
*   Naninovel
    

We are also working to integrate more packages as well, for example:

*   Adventure Creator
    

### 

[](https://docs.febucci.com/text-animator-unity/integrations#easy-integration)

Officially Supported Third Parties

**Yarn Spinner**

### 

[](https://docs.febucci.com/text-animator-unity/integrations#easy-integration-1)

Easy Integration

Most assets should work through an _easy integration_, meaning that the asset should be able to pick text changes from Text Mesh Pro and start the typewriter from that. BUT official integrations are coming soon!

Invisible tags appended to your text

_If you're using Easy Integration, TextAnimator will add two invisible tags in appendix to your text in order to work. No worries, the text placement/layout will be left unchanged and it will act like if the tags are not written at all._

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Curve Effects | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/customization/create-your-own-effects/curve-effects

**Curve effects let you control many transform properties and let you choose their animation over time.**

* * *

You can create a custom curve effect from the "Special" Effect submenu.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3857371675-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FmuVE9taoTg1C5htCyrOt%252FScreenshot%25202025-11-16%2520alle%252018.57.13.png%3Falt%3Dmedia%26token%3Df99c5af4-aae5-4af1-aa2e-2381f803c31c&width=768&dpr=4&quality=100&sign=ef327f5d&sv=2)

You have the same [Phases](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/phases)
settings, and in addition it's important to notice the "**Weight**" curve (which goes from 0 to 1 and should loop if you want a seamless effect).

We are working to add new properties from the next versions as well!

Thanks to Text Animator 3.0 and the new Core Library, effects _data_ is separated from _implementation_, meaning that we can improve the backend/structure without altering your data! (or provide better porting steps/auto-fixes anyways)!

Last updated 1 month ago

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Styles | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/customization

Styles quickly replace parts of the text with something else, for example to create a combo of effects, typewriter actions and events, which would otherwise require you a lot of typing for recurring tags.

If you're using TMPro, please use Text Animator Styles and not TMPro ones for this, as the latter (TMPro's) can't recognize Text Animator tags and will result in them being added to the text.

* * *

Simply open the stylesheet scriptable object of your choice (you can create one in the Project Folder, via the Create menu -> Text Animator-> StyleSheet) and start adding/editing tags.

You can have a Global stylesheet ( [Global Settings](https://docs.febucci.com/text-animator-unity/customization/global-settings)
 ) and also a local one.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FbEgcnrQ9RzsRjy1jCb7m%2Ftextanimator%2520settings%2520stylesheet%2520example.png&width=768&dpr=4&quality=100&sign=f266ed25&sv=2)

From the example above, whenever you write the style tag “`<style1>`” in the text, it will be replaced with “`<wave><play=5><rainb><shake>`” - and closing it with “`</style1>`” will be replaced with “`</wave></rainb></shake><?ended>`”.

Styles tags are case insensitive (writing "<style1>" and "<Style1>" will produce the same result).

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Yarn Spinner | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/integrations/integrated-plugins-and-dialogues-systems/yarn-spinner

Yarn Spinner is a powerful tool that lets you write dialouges like this:

Copy

    -> What's going on? <<once>>
        Guard: The kingdom is under seige!
    -> Where can I park my horse? <<once if $has_horse>>
        Guard: Over by the tavern.
    -> Lovely day today!
        Guard: Uh huh.
    -> I should go.
        Guard: Please do.

And it also has a visual debugger, works directly in Unity and many other great features.

[Yarn Spinnerwww.yarnspinner.dev](https://www.yarnspinner.dev/)

* * *

### 

[](https://docs.febucci.com/text-animator-unity/integrations/integrated-plugins-and-dialogues-systems/yarn-spinner#integration-steps)

Integration Steps

To integrate Yarn Spinner 3, you need to replace Text Animator's Typewriter Component with their one, made specifically for this integration.

You will be able to use all Text Animator features (including the typewriter wait times etc.) while _**also**_ having arbitrary waits and other specific features of Yarn Spinner.

You can read more here:

[![Logo](https://docs.yarnspinner.dev/~gitbook/image?url=https%3A%2F%2F133540031-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fcollections%252FKwtKEQTliyPminHTczxw%252Ficon%252Fv3hX5YL7Z6ThxSO48Uvd%252FYarnSpinner-GitBook-Icon.png%3Falt%3Dmedia%26token%3D4567e3aa-6559-4522-a9d2-627155c77d22&width=48&height=48&sign=f0865bf9&sv=2)Text Animator | Yarn Spinnerdocs.yarnspinner.dev](https://docs.yarnspinner.dev/3.1/yarn-spinner-for-unity/unity-add-ons/text-animator)

Please let us know if it's working how you expected and/or if you'd like any new feature or improvement!!

* * *

If you are on older version of Yarn Spinner, please refer to their documentation to how the integration works! (Yarn Spinner supports both TAnim 2.X and 3.X, yay!)

Last updated 1 month ago

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Advanced Concepts | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/writing-custom-classes

Behind the scenes, Text Animator is doing a lot of work and optimization to make sure:

*   There is 0 garbage collection during animations _(there is still some when the text is set, as does TMPro and also Text Animator 2.0, but we're working on it!)_
    
*   The asset is compatible with different Unity versions, systems and platforms
    
*   There is an API that is as simple as possible for you _(putting the pain on us, but that's the whole point!)_
    
*   Things work even if there is a wrong setup with null references (as humanly possible)
    

That said, there are some key concepts inside Text Animator for Unity that are important to know when you start writing custom scripts:

*   [Core Library](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts#core-library)
    
*   [Stateless vs Referenced elements](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts#stateless-vs-referenced-elements)
    

* * *

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes#core-library)

Core Library


------------------------------------------------------------------------------------------------------

Text Animator is divided in two main namespaces:

*   The "`Febucci.TextAnimatorCore`" is our **core library,** a runtime DLL shipped inside the package and that is foundamental to make things work.
    
*   The "`Febucci.TextAnimatorUnity`" is the **Unity implementation**, from Scriptable Objects to Monobehaviors and more.
    

You will find how to set up scripts as intended in the next pages/guides, but please be careful about what you inherit, modify or re-implement!

I'll keep updating the core library to implement new features or reorganize the structure, and it's impossible to know any kind of variation and use case people might do in C# (especially if not intended) - so please follow the guides! I'll mark things internal as much as possible anyways and I'll keep the Unity implementation as backwards compatible as possible between versions (as I always did in the past years, also including an updating guide where applicable) - but if you want to do some not-planned modification do it at your own risk!

If you do upgrade Unity version mid-project, please remove the asset and re-download it from the package manager (it will download the package built for that Unity version, behind the scenes!)

[I upgraded Unity version (2022.3->Unity 6.3) and there are some errors with Text Animator](https://docs.febucci.com/text-animator-unity/other/troubleshooting#i-upgraded-unity-version-2022.3-greater-than-unity-6.3-and-there-are-some-errors-with-text-animator)

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes#stateless-vs-referenced-elements)

Stateless vs Referenced elements


----------------------------------------------------------------------------------------------------------------------------------------------

Most Text Animator elements, from effects, actions, playbacks and curves, are implemented in two ways. One is independent from Unity and GameObjects/ScriptableObjects in general, and the other keeps references from the game state / files and classes.

Type

Pros

Cons

Stateless

*   Better optimized (also prepared for Burst in the future, TBD)
    
*   No race conditions between elements
    

*   Some code wrappers, BUT mitigated through the asset's custom classes!
    
*   Can't modify animations/typewriters based on the game state
    

Referenced

*   Can access the game state and make things happen differently based on it
    

*   Possible race conditions if not implemented correctly (e.g. two typewriters accessing the same action, which has a timer or makes things happen, at the same time)
    
*   Can't be optimized through Burst (but should be negligible in most occasions, as built-in ones do the heavy part)
    

We are also investigating for a way to give you _**Direct**_ elements, which mean: remove all or own implementations and just let you hook things how you want (which should accomodate like the 1% of the users, given all the other tools available, but still an important option in our opinion).

*   **Pros**: Do it yourself.
    
*   **Cons**: Do it yourself.
    

It's up to you to decide how to customize your elements.

*   Opt for stateless types when you are in performance-critical context (e.g. having many letters)
    

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Upgrading from 2.X to 3.X | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/other/changelog/upgrading-from-2.x-to-3.x

Hi there! This post contains some useful info about upgrading from Text Animator 2.X to 3.0. If you have any other question, please feel free to [contact us via support](https://www.textanimatorforgames.com/support)
!

[](https://docs.febucci.com/text-animator-unity/other/changelog/upgrading-from-2.x-to-3.x#how-to-get-3.x)

How to get 3.X


-----------------------------------------------------------------------------------------------------------------------------

In order to get access to version 3.X:

*   **If you have purchased Text Animator 2.X in the last 12 months** (November 2024 onwards), then you can claim 3.X for free! Simply go to to the new asset store page with the same account that has 2.X, and you will see the "Free" option unlocked. Be sure to **do it now**, even if you plan using 3.X later.
    
*   Otherwise, **if you have purchased Text Animator before November 2024**, then you can update at a very discounted price (we have updated the asset for free in the past 5 years! and to keep things accessible for everyone, also given the huge work on version 3.X, we do need your support with bigger releases).
    

👉 **Please notice** that Text Animator 3.X has a different license! It is way more affordable for both indies and also bigger teams, you can [read more here](https://www.textanimatorforgames.com/unity#pricing)
.

[](https://docs.febucci.com/text-animator-unity/other/changelog/upgrading-from-2.x-to-3.x#update-only-on-new-projects)

Update only on new projects


-------------------------------------------------------------------------------------------------------------------------------------------------------

**We strongly recommend that you start using 3.X only on new projects**, given all the [giant changes](https://docs.febucci.com/text-animator-unity/other/changelog)
 and new improvements. Text Animator 2.X is now under Long Term Support, so that you can keep working on your games with additional bug fixes and support from us. A lot needed to change to have this new version and to prepare for all the future plans we have in mind - so we did it all at once (including the license change) so that a) it's something you only have to think about once, and b) we can work on the new updates more easily without being stuck

**If you still want to update from 2.X to 3.0 inside a live project**, the difficulty will vary based on how much you have customized 2.X:

*   **If you have only hooked up Text Animator components**, without modifying other scripts, you'll probably have some errors about namespaces (or obsolete fields) but then you should be good to go and re-create effects in the inspector and similar.
    
*   **If you wrote a lot of custom effects actions and more,** then you'll probably need to spend some extra time porting everything (even if the API has some parallels, the new [Core Concepts](https://docs.febucci.com/text-animator-unity/quick-start/core-concepts)
     and [Advanced Concepts](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts)
     change the formula quite a bit). Head over [Writing Custom Classes](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts)
     for more.
    

In any case, you will have to re-edit effect values, databases and hook up components again. We are planning for an auto-updater in the future (these things are _a ton_ of work) - but please go into 3.0 knowing that it's not there yet!

[](https://docs.febucci.com/text-animator-unity/other/changelog/upgrading-from-2.x-to-3.x#main-differences)

Main differences


---------------------------------------------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/other/changelog/upgrading-from-2.x-to-3.x#core-concepts)

Core Concepts

Other than the [Changelog](https://docs.febucci.com/text-animator-unity/other/changelog)
, here are some core concept changes that help you quick start with the new version, if you have used 2.X in the past already.

*   There is only **one** typewriter component (instead of two, per character and per word). Its timing values (per character and per word) are now scriptable objects that you can reference and switch at any time. Read more here [Show and hide letters dynamically](https://docs.febucci.com/text-animator-unity/typewriter/show-and-hide-letters-dynamically)
    
*   The main settings are now moved to [Global Settings](https://docs.febucci.com/text-animator-unity/customization/global-settings)
    
*   Some effect might have different tags, like "slide" became "slideh" and "slidev". Simply click on the effects database and change the tag for whatever you need!
    

Please be sure to read [Core Concepts](https://docs.febucci.com/text-animator-unity/quick-start/core-concepts)
and the documentation in general, of course, to discover what's new and how to do things!

### 

[](https://docs.febucci.com/text-animator-unity/other/changelog/upgrading-from-2.x-to-3.x#api)

API

For simple references to Text Animator elements:

*   The `Febucci.UI` namespace is now `Febucci.TextAnimatorForUnity`
    
*   `TypewriterCore` has been replaced with `TypewriterComponent`
    

For more advanced changes:

*   Please have a look at the [Advanced Concepts](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts)
    
*   Have a look at each [Writing Custom Classes](https://docs.febucci.com/text-animator-unity/writing-custom-classes/advanced-concepts)
     page to discover how to re-implement the custom classes.
    

### 

[](https://docs.febucci.com/text-animator-unity/other/changelog/upgrading-from-2.x-to-3.x#missing-elements-to-be-reimplemented-later)

Missing elements to be reimplemented later

*   The "notype" tag, which allowed you to skip typewriter entirely. **Workaround** (if you have used it in your texts)**:** set a style with tagID "notype" and set a super high speed. We will update it anyways in the next weeks!
    

We are also working to update the [Integrations](https://docs.febucci.com/text-animator-unity/integrations/integrated-plugins-and-dialogues-systems)
 (even if most should work right away). Read more here [Integrated Plugins & Dialogues Systems](https://docs.febucci.com/text-animator-unity/integrations/integrated-plugins-and-dialogues-systems)
.

Last updated 22 days ago

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Changelog | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/other

**P.S. Please always back up your projects (**_**or better: use version control**_**) before updating anything, even outside Text Animator. Cheers!**

* * *

[](https://docs.febucci.com/text-animator-unity/other#latest-releases)

Latest Releases


-------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/other#id-3.2.0-custom-rotation-pivots-and-bugfixes-2025.12.18)

3.2.0 - Custom Rotation Pivots and Bugfixes \[2025.12.18\]

#### 

[](https://docs.febucci.com/text-animator-unity/other#added)

Added

*   Added effects with custom pivot rotations
    
*   Reimplemented pendulum effect, both for appearances, presistent and disappearance
    
*   \[API\] Exposed a character's passed time in CharacterData
    

#### 

[](https://docs.febucci.com/text-animator-unity/other#bugfix)

Bugfix

*   Fixed an annoying bug that - for some users - was showing Text Animator's window after script reloads
    
*   Fixed typewriter skipping characters if the Unity Editor had a lag spike
    
*   Fixed error when the Unity Package manager was unable to find the package
    

* * *

### 

[](https://docs.febucci.com/text-animator-unity/other#id-3.1.1-yarn-spinner-typewriter-events-and-news-panel-2025.12.03)

3.1.1 - Yarn Spinner, Typewriter events and News panel \[2025.12.03\]

#### 

[](https://docs.febucci.com/text-animator-unity/other#added-1)

Added

*   Yarn Spinner is now officially integrated! (from version 3.1)
    
*   Added events in the inspector (Typewriter Component) when the typewriter has started and finished waiting for a character
    
*   News panel directly in the About Window, to keep tracks of new updates without having to leave the Editor
    

#### 

[](https://docs.febucci.com/text-animator-unity/other#bugfix-1)

Bugfix

*   Fixed "waitforinput" action was not correctly serialized in the builtin Actions Databse
    
*   Fixed typewriter not starting correctly if parent object was disabled
    

#### 

[](https://docs.febucci.com/text-animator-unity/other#minor-changes)

Minor Changes

*   Added Documentation and License links in the package.json
    
*   Added virtual methods to do something before or after the typewriter is waiting for a character
    
*   Added custom icon for Text Animator and Typewriter components
    

* * *

### 

[](https://docs.febucci.com/text-animator-unity/other#id-3.0.0-ui-toolkit-support-a-new-animation-engine-and-much-more-2025.11.18)

3.0.0 - UI Toolkit Support, a new Animation Engine and much more! \[2025.11.18\]

#### 

[](https://docs.febucci.com/text-animator-unity/other#added-2)

Added

*   UI Toolkit is now supported from Unity 6.3
    
*   You can play effects only once, make them loop x times, start delayed and many other combinations thanks to "Playbacks" (both from the editor or through rich text tags)
    
*   The same effect can now be played in all occasions as an Appearance, Persistent (previously "Behavior") and Disappearance, increasing the number of effects available even more (e.g. a "wave" as an appearance, with different parameters than the "wave" as a persistent/behavior).
    
*   You can use rich text tags modifiers to set an effect parameter directly, multiply it or set a specific keyword
    
*   You can now set different curves to an effect, changing different transitions and their movement/influence over time (e.g. make a rotation seem laggy, increasing step by step).
    
*   Added more options to already existing effects, like the "expanding" and "sliding" directions
    
*   Typewriter Actions can be created as "Components" (other than Scriptable Objects) allowing you to reference scene objects more easily
    
*   Typewriter Actions now support both Coroutines and also a Stateless "tick" progression
    
*   A new typewriter action called "PlaySound": plays and waits for an Audio Source to finish before progressing the typewriter
    
*   _Added many other little refinements, tooltips and much more._
    

_We'll probably discover we missed to write some features here and will update this page in the next weeks - there were_ _**hundreds**_ _of commits over the last months of development!_.

#### 

[](https://docs.febucci.com/text-animator-unity/other#improved)

**Improved**

*   Rewrote the entire documentation, hopefully making it even more easy to understand and covering extra best practices, tips and frequently asked questions
    
*   Improved the License for the asset, now even more accessible for both indies and bigger teams.
    
*   Optimized effects with 0 garbage collection during, and many other optimization considerations
    
*   Fixed race conditions between effects (happening in some extreme occasions)
    
*   Fixed race conditions between actions, also allowing you to specify local actions for specific typewriters
    
*   Improved UX for the Editor, as well as APIs.
    
*   Improved Welcome screen and Setup window, now doing some extra checks
    
*   You can now share settings between multiple typewriters and text animators.
    
*   Improved API to handle rich text tags parameters, now automatically handled by Text Animator
    
*   _Many bugfixes (like the new input system warnings) and more_.
    

#### 

[](https://docs.febucci.com/text-animator-unity/other#breaking-api)

Breaking API

*   Most API is breaking (as we changed Namespaces and some core architecture, especially if you wrote custom C# effects or actions). A lot needed to change to have this new version and to prepare for all the future plans we have in mind - so we did it all at once (including the license change) so that a) it's something you only have to think about once, and b) we can work on the new updates more easily without being stuck. Please do read [Upgrading from 2.X to 3.X](https://docs.febucci.com/text-animator-unity/other/changelog/upgrading-from-2.x-to-3.x)
    . Thanks!
    

* * *

[](https://docs.febucci.com/text-animator-unity/other#known-issues)

Known Issues


-------------------------------------------------------------------------------------

**We are working on a fix and will update the asset as soon as possible anyways**!

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Accessing parameters | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-effects-c/accessing-parameters

It can be very useful to access the values and parameters within a tag via code. This is easily achieved using the `RegionParameters` structure in the `UpdateParameters` function, which provides access to each region of your texts.

Copy

    public void UpdateParameters(RegionParameters parameters)
    {
        // ...
        value = parameters.ModifiyFloat("a", fallbackValue);
    }

### 

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-effects-c/accessing-parameters#keywords)

Keywords

As seen in [Keywords](https://docs.febucci.com/text-animator-unity/effects/how-to-edit-effects/modifiers#keywords)
, keywords are plain words (without an equal sign) inside your tag (e.g., `<mytag keyword1 keyword2 ...>`). To access the list of these keywords, you can use:

Copy

    var keywords = parameters.keywords

*   The effect's name is a keyword itself (e.g., if I have `<mytag key1>`, the hashset will contain `[mytag, key1]`);
    
*   Modifiers are ignored in this list (e.g., if I have `<mytag myMod=10.0>`, the hashset will contain `[mytag]`);
    
*   Duplicate keywords are ignored (since we are using a HashSet).
    

### 

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-effects-c/accessing-parameters#float-values)

Float values

To access float value you can use:

Copy

    // Returns true/false wheter the tag contains the modifier
    parameters.HasFloat("modName");
    
    // Returns the modifier value if exists otherwise the fallback value
    parameters.ModifiyFloat("modName", fallbackValue); 

### 

[](https://docs.febucci.com/text-animator-unity/writing-custom-classes/writing-custom-effects-c/accessing-parameters#string-values)

String values

You can also access string modifiers:

Copy

    // Returns true/false wheter the tag contains the modifier
    parameters.HasString("modName"); 
    
    // returns the modifier value if exists otherwise the fallback value
    parameters.GetStringValueOrDefault("modName", fallbackValue); 

Last updated 4 days ago

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 常见问题 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#ben-di-hua)

本地化

Text Animator 支持多语言吗？[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#text-animator-zhi-chi-duo-yu-yan-ma)

简短回答： **是的，** _**但这不依赖于 Text Animator**_.

*   关于已翻译的文本： 是的，但本地化不是由 Text Animator 处理的。本地化由外部脚本处理（例如你自己的本地化管理器、对话系统等）。 换句话说，Text Animator 不是一个本地化插件。 如果你的文本包含富文本标签，翻译后的语言中必须保持相同的布局（例如 “hello <shake> how are you?” 应翻译为 “ciao <shake> come stai?”）。 然后，你可以简单地调用 "textAnimatorComponent.ShowText(translatedText);"。 （此本地化流程也适用于任何其他游戏/项目 \[...\])
    
*   关于不同字体： 是的，但这不是由 Text Animator 处理的（而是由 Text Mesh Pro 处理）。 如果 TMPro 支持某种语言，Text Animator 也能正常工作。这是因为 Text Animator 仅对由 TextMeshPro 生成的字母进行动画处理。
    

是否支持从右到左的文本？(RTL)[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#shi-fou-zhi-chi-cong-you-dao-zuo-de-wen-ben-rtl)

支持！在幕后 TextAnimator 仅移动网格，但生成网格的是 TextMeshPro。TMPro 支持 RTL 文本（可在组件的检查器中启用），因此 TextAnimator 也支持。

_请_ 注意，像 “RTLTMPro” 这样的外部包可能并不完全受支持，因为我们严格指的是 TMPro，请查看 [集成的插件与对话系统](https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng/ji-cheng-de-cha-jian-yu-dui-hua-xi-tong)
 来代替。

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#ji-cheng-yu-ban-ben)

集成与版本

支持哪些 Unity 版本？[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#zhi-chi-na-xie-unity-ban-ben)

你可以通过在此处阅读来了解支持哪些 Unity 版本： [要求与限制](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi)

我的 \[在此插入对话系统\] 支持吗？[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#wo-de-zai-ci-cha-ru-dui-hua-xi-tong-zhi-chi-ma)

你可以通过在此处阅读来了解哪些第三方插件已与 Text Animator 集成： [集成的插件与对话系统](https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng/ji-cheng-de-cha-jian-yu-dui-hua-xi-tong)

支持 UIToolkit 吗？[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#zhi-chi-uitoolkit-ma)

支持！从 Unity 6.3 及以上版本。

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#xiao-guo-yu-jie-xi)

效果与解析

我可以更改富文本解析的符号吗？（例如使用 "\[shake\]" 而不是 "<shake>"）[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#wo-ke-yi-geng-gai-fu-wen-ben-jie-xi-de-fu-hao-ma-li-ru-shi-yong-shake-er-bu-shi-shake)

可以！从 [全局设置](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/quan-ju-she-zhi)
 文件。

效果何时应用？打开标签时还是关闭标签之后？[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#xiao-guo-he-shi-ying-yong-da-kai-biao-qian-shi-hai-shi-guan-bi-biao-qian-zhi-hou)

效果从你打开其标签的那一刻起就会被应用。

"<shake>hello在你设置第一个 “hello” 字符时，单词 “\>” 就已经开始抖动了。

TextAnimator 会在编辑模式下预览效果吗？[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#textanimator-hui-zai-bian-ji-mo-shi-xia-yu-lan-xiao-guo-ma)

会的！只需单击某个效果的 Scriptable Object 即可查看其预览。 [如何编辑特效](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao)

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#qi-ta)

其他

我有一些关于许可的问题[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#wo-you-yi-xie-guan-yu-xu-ke-de-wen-ti)

你可以阅读 [有关许可的信息在此处](https://www.textanimatorforgames.com/unity#faq)
.

我可以在网页构建中使用 Text Animator 吗？[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#wo-ke-yi-zai-wang-ye-gou-jian-zhong-shi-yong-text-animator-ma)

可以！

我可以删除插件的 “Example” 文件夹吗？[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#wo-ke-yi-shan-chu-cha-jian-de-example-wen-jian-jia-ma)

当然，如果你不需要插件的示例文件夹，可以将其删除。

_谁很棒？_[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#shui-hen-bang)

你很棒！

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq#xiang-wo-men-ti-wen)

向我们提问

如果你有其他问题，欢迎随时联系我们！

[![Logo](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fframerusercontent.com%2Fimages%2Fy1LCEnd5hyGjuX0kKaGBUorzMc.png&width=20&dpr=4&quality=100&sign=82d1be32&sv=2)Support Requests | Text Animator for Unity, Godot and Unrealwww.textanimatorforgames.com](https://www.textanimatorforgames.com/support)

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 Unity 文本动画器 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh

**欢迎** 来到文档： **适用于 Unity 3.X 的文本动画器**！我们迫不及待想让你为文本添加动画并熟悉此插件。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F74b3Q86Y180EtGnu7Jg5%252FGames%2520Using%2520Text%2520Animator.png%3Falt%3Dmedia%26token%3D9912a72f-fab2-4003-b8f7-3499fc676a33&width=768&dpr=4&quality=100&sign=f97de3a1&sv=2)

我们正在编写尽可能简短且简明的文档，同时在你最需要时也能提供帮助。 **请务必阅读第一部分和最重要的页面**！现在花几分钟时间阅读，但将会节省 _大量时间_ 以后。

有用的链接：

[购买](https://www.textanimatorforgames.com/unity#pricing)
 [官网](https://www.textanimatorforgames.com/unity)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh#zhi-de-liao-jie-de-shi-xiang)

值得了解的事项

*   本文档提供多种语言版本: 英语、中文、韩语、日语。
    
*   你可以查看 此文档的不同版本和语言 在本页顶端。
    
*   文本动画器也可用于其他引擎. [在此了解更多](https://www.textanimatorforgames.com/)
    .
    

如果你随时需要帮助，欢迎访问 [故障排除页面](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu)
 （常见问题及其解决方法）或支持页面！

[![Logo](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fframerusercontent.com%2Fimages%2Fy1LCEnd5hyGjuX0kKaGBUorzMc.png&width=20&dpr=4&quality=100&sign=82d1be32&sv=2)Support Requests | Text Animator for Unity, Godot and Unrealwww.textanimatorforgames.com](https://www.textanimatorforgames.com/support)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh#xu-qiu)

需求

请在购买或导入前访问 [要求与限制](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi)
 页面，我们也在此回答了许多 [常见问题](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq)
。谢谢！

* * *

**玩得开心** 并别忘了加入我们的 [Discord](https://discord.com/invite/j4pySDa5rU)
 参与讨论并展示你的创作！

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 安装与快速上手 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/an-zhuang-yu-kuai-su-shang-shou

使用此资源只需几次点击（导入 -> 添加组件 -> 按播放），但为更好地理解所有内容，请查看以下页面，这样你可以更快且以正确的方向开始。

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/an-zhuang-yu-kuai-su-shang-shou#how-to-implement-text-animator)

1\. 在 Unity 中导入 Text Animator


-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

第一步，你需要在项目中导入 Text Animator for Unity。

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/an-zhuang-yu-kuai-su-shang-shou#jian-rong-xing-jian-cha)

兼容性检查

**该资源兼容以下用户界面和 Unity 版本**:

*   **Text Mesh Pro** （Unity 2022.3 及更高版本）
    
*   **UI 工具包** (Unity 6.3 及更高版本).
    

它同样支持新的 Unity 输入系统（也支持旧版输入系统）。

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/an-zhuang-yu-kuai-su-shang-shou#dao-ru-bao)

导入包

一旦项目正确设置，你可以从包管理器（Asset Store 选项卡）导入 Text Animator。

确保包含 "Samples/BuiltIn" 文件夹，否则该资源可能无法正常工作。

安装成功后， **欢迎窗口** 将会弹出，Text Animator 已准备好为你的文本添加动画！

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252Fo6lFhmxUPaki6oAtVVXZ%252FScreenshot%25202025-11-15%2520alle%252017.40.31.png%3Falt%3Dmedia%26token%3D729acbd3-556d-4808-9726-7f3918afec84&width=768&dpr=4&quality=100&sign=5f8c0ce8&sv=2)

导入后显示的欢迎窗口的一部分

如果关于窗口未显示，或你想稍后查看，可以随时从菜单的 Tools/Febucci/TextAnimator/About Window 访问它！

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/an-zhuang-yu-kuai-su-shang-shou#id-2.-shi-li-chang-jing)

2\. 示例场景


---------------------------------------------------------------------------------------------------------------------------------------------

你可以直接从检视面板了解大多数 Text Animator 功能，并从示例场景中查看我们如何设置以及它们的直接效果。

从名为“**00 - Welcome**”的场景开始，或在 Text Animator 的欢迎窗口中点击“Get Started”。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FLgTsSXatpKk3M2Nr36VN%252FScreenshot%25202025-11-15%2520alle%252017.45.47.png%3Falt%3Dmedia%26token%3D624c13da-2e67-4653-9caa-076cc5cfa24d&width=768&dpr=4&quality=100&sign=2f467ea5&sv=2)

要访问示例场景，请确保你已经导入它们！当你不再需要时，也可以安全地移除/删除它们。

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/an-zhuang-yu-kuai-su-shang-shou#animating-your-first-texts)

3\. 为你的首批文本添加动画


-------------------------------------------------------------------------------------------------------------------------------------------------------

你可以在几次点击内让文本运行起来！

UI Toolkit

Text Mesh Pro

_附注。假设你已经知道_ [_如何使用 UI Toolkit_](https://docs.unity3d.com/Documentation/Manual/UIElements.html)
 _以及它的功能。_

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/an-zhuang-yu-kuai-su-shang-shou#cong-ui-builder)

从 UI Builder

*   转到 库 -> 项目
    
*   拖动 "AnimatedLabel" 从你的层级视图中的 "Custom Controls/Febucci/Text Animator for Unity"！
    

我们正在努力确保你可以直接从 UI Toolkit 为内置的 Label 和 Button 添加动画！ _（Unity 6.3 及更高版本。）_ 保持更新！

你的 .uxml 应该看起来像这样：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZNwCUmAugxLNcVVO9oBk%252FScreenshot%25202025-11-15%2520alle%252018.02.51.png%3Falt%3Dmedia%26token%3Dced34791-d558-4883-b646-2197664dd637&width=768&dpr=4&quality=100&sign=c9df04c7&sv=2)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/an-zhuang-yu-kuai-su-shang-shou#tong-guo-dai-ma)

通过代码

你可以创建一个 "Febucci.TextAnimatorForUnity.AnimatedLabel" 类的实例并将其添加到你的 UI 文档，像这样：

复制

    using UnityEngine;
    using UnityEngine.UIElements;
    using Febucci.TextAnimatorForUnity; // <- 导入 Text Animator 的命名空间
    
    public class ExampleScript : MonoBehaviour
    { 
        [SerializeField] UIDocument document;
    
        void Start()
        {
            var container = document.rootVisualElement.contentContainer;
            var animatedLabel = new AnimatedLabel(); // <- 创建一个动画标签
            container.Add(animatedLabel); // <- 将其添加到内容容器中
            // [..]
            animatedLabel.SetText("<wave>hello"); // <- 设置文本
        }
    }

_附注。假设你已经知道_ [_如何使用 Text Mesh Pro_](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html)
 _以及它如何工作。_

添加一个 Text Animator - Text Mesh Pro 组件到同一个具有 TextMeshPro 组件（无论是 UI 还是世界空间！）：

你的检查器应如下所示：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FT3h66pIPFdakGOCfToEY%252FScreenshot%25202025-11-15%2520alle%252017.59.18.png%3Falt%3Dmedia%26token%3D26196c49-f0f5-457b-85dd-da358f43c823&width=768&dpr=4&quality=100&sign=c0c53f3b&sv=2)

你可以阅读 [设置文本](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben)
 以获取更多细节和建议！

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/an-zhuang-yu-kuai-su-shang-shou#id-1-writing-effects-in-your-text)

在文本中书写效果

在文本中添加效果的一种方式是使用富文本标签，例如：“`I'm <shake>freezing</shake>`”，其中 “shake” 是内置效果的 ID。

*   尝试通过实验以下标签来书写文本： `<wiggle>` `<shake>` `<wave>` `<bounce>`，例如“`<wiggle>I'm joking</wiggle> hehe now <shake>I'm scared</shake>`”，然后进入 Unity 的播放模式（Play）。
    

你的文本会根据你写的效果对字母进行动画处理！

* * *

祝你玩得开心！你可以继续下一页，深入了解该资源的所有功能。

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 要求与限制 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi

Text Animator 是一个功能非常强大的工具，对要求和限制很少。 **购买前请在此阅读！**

*   [要求](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#requirements)
    
*   [限制](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#limitations)
    

你可能也会感兴趣：

*   [集成](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#integrating-third-party-dialogue-systems-and-plugins)
    
*   [要求与限制](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#frequently-asked-questions)
    
*   [要求与限制](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#known-issues)
    

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#requirements)

要求


----------------------------------------------------------------------------------------------------------

**该资源兼容以下用户界面和 Unity 版本**:

*   **Text Mesh Pro** （Unity 2022.3 及更高版本）
    
*   **UI 工具包** (Unity 6.3 及更高版本).
    

它同样支持新的 Unity 输入系统（也支持旧版输入系统）。

请注意，我们不正式支持 Unity Alpha 和 Beta 版本！我们无法得知 Unity 是否更改了 API 等。 _这一天_ 他们发布新的 alpha 或 beta 的那天，所以我们会使用这些版本进行测试并确保该资源在正式/生产版本中可用。谢谢！

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#integrating-third-party-dialogue-systems-and-plugins)

集成


--------------------------------------------------------------------------------------------------------------------------------------------------

集成第三方对话系统和插件：

我们将在接下来的几周内移植所有第三方集成！在此阅读更多信息 [集成的插件与对话系统](https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng/ji-cheng-de-cha-jian-yu-dui-hua-xi-tong)

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#limitations)

限制


---------------------------------------------------------------------------------------------------------

这是该资源无法 _（当前）_ 实现的。

“横线/条”不进行动画（出于选择）[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#heng-xian-tiao-bu-jin-xing-dong-hua-chu-yu-xuan-ze)

文本中的“横线/条”（`删除线` **和** `下划线`）出于选择不进行动画。

（这是有动画的横线/条的效果。由于它们并不是很好看，因此选择让它们保持静态。）

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2Fj1zQb4UQUOp9BRiaMwTh%2Fbarsnotanimated.gif&width=300&dpr=4&quality=100&sign=1b0918ec&sv=2)

卸载包时移除标签[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#xie-zai-bao-shi-yi-chu-biao-qian)

如众所周知（例如 TMPro），如果你卸载此包，你必须手动从对话中移除所有该插件的标签。

👍🏻 如果你因此感到担心 _出于任何原因，_ **你可以将插件设置为仅使用“回退效果”** （这些效果会被应用 **到整段文本** 而不需要任何标签），并且在你移除插件的情况下所有内容将保持不变。太棒了！

使用 \\r 和 \\b[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#shi-yong-r-he-b)

你不能在中途删除或替换 _部分_ 文本。

❌ 退格（例如， `\b` ）当前不受支持

✔️ 你可以在中途删除/更改/替换 **整段** 文本，和/或隐藏特定部分的文本。

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#chang-jian-wen-ti-jie-da)

常见问题解答


--------------------------------------------------------------------------------------------------------------------------

请同时阅读 [常见问题](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq)
以了解常见问题及其解决方法。谢谢！

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi#yi-zhi-wen-ti)

已知问题


-------------------------------------------------------------------------------------------------------------

**我们正在着手修复，并且无论如何会尽快更新该资源。**!

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 如何添加特效 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao

你可以通过以下方式为文本添加效果：

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao#set-effects-to-specific-parts-of-the-text)

将效果设置为文本的特定部分

你可以通过使用以下方式为文本的特定部分添加效果： **富文本标签。**

效果标签看起来像这样：

*   **持续**: `<tagID>` 用于打开， `</tagID>` 用于关闭
    
*   **出现**: `{tagID}` 用于打开， `{/tagID}` 用于关闭
    
*   **消失**: `{#tagID}` 用于打开， `{/#tagID}` 用于关闭 _（基本上是一个带有_ `_#_` _在前面的外观标签，用来简单提醒你消失效果是外观效果的反向）_.
    

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao#extra-notes-about-rich-text-formatting)

关于富文本格式的额外说明

使用 Unity 的 TextAnimator：

*   你可以将多个效果叠加在一起（例如：“`<shake><size>`”）。 （也请查看 [样式](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/yang-shi)
    )
    
*   你可以用一个单一的 ‘ **所有** ’ 字符来关闭当前打开的效果，例如：`/`’ 字符，比如：
    
    *   ”`</>`” 用于持久效果
        
    *   ”`{/}`” 用于出现效果
        
    *   ”`{/#}`” 用于消失效果。
        
    
*   如果你在文本结尾处，则无需关闭标签，因为 Text Animator 从你打开标签的那一刻就开始应用效果。（例如 "`<shake>hello`" 将导致 hello 已经在动画中）。
    

你可以更改不同的

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao#set-default-effects-to-the-entire-text)

将默认效果设置为整个文本

你可以决定默认会应用于所有字母的效果， **而无需在文本中编写效果标签** 多亏了 [动画器设置](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/dong-hua-qi-she-zhi)
.

UI 工具包

Text Mesh Pro

AnimatedLabel 的设置通过不同的可脚本化对象处理（在本例中是下图中高亮的那个）。在此处阅读更多关于 [如何创建一个](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/dong-hua-qi-she-zhi)
.

如果你没有设置一个，则会使用 [全局设置](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/quan-ju-she-zhi)
 中的那个！

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FagcdeSBrmD3NCQLoVswt%252FScreenshot%25202025-11-15%2520alle%252018.33.00.png%3Falt%3Dmedia%26token%3D6d57fa83-5f8f-475d-940f-280151ce67d5&width=768&dpr=4&quality=100&sign=b9d61f70&sv=2)

对于 Text Mesh Pro，设置可以是“本地”的（绑定到组件），也可以是“共享”的（在其他 Text Animator 实例之间）。

*   要修改 **本地的** 设置，只需前往“TextAnimator - Text Mesh Pro”组件检查器并调整其值。
    
*   要修改 **共享** 设置，请分配相应的 ScriptableObject 实例。 [在此阅读更多内容](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/dong-hua-qi-she-zhi)
    .
    

在设置内：

1.  访问“默认标签（Default Tags）”部分
    
2.  展开你想编辑的效果类别
    
3.  添加你想包含的任何效果标签，例如：
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FMGbfDEQeK1CRnktW6aue%252FScreenshot%25202025-11-15%2520alle%252018.48.23.png%3Falt%3Dmedia%26token%3D2a7db44e-c31a-48ae-a317-871ca6006070&width=768&dpr=4&quality=100&sign=ffe45a7f&sv=2)

如果你不想默认应用任何效果，只需将效果数量设置为零。

你也可以更改“**默认标签模式（Default Tags Mode）**” 为 “**常量**” 如果你希望效果始终被应用，并覆盖一切。

你可以为每个数组元素添加修饰符，例如“shake a=5”，在这里阅读更多： [修饰符](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu)

示例：回退（Fallbacks）[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao#shi-li-hui-tui-fallbacks)

假设我们有一个默认效果（“size”），但我们想对文本的特定部分应用“fade”效果。 我们可以通过这样写来实现该结果："default default \`{fade}\` fade fade fade \`{/fade}\` default default"

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FfkwPOWUP3UA38XjdRWRQ%2Ftext-animator-override-appearances-example-ezgif.com-video-to-gif-converter.gif&width=300&dpr=4&quality=100&sign=a2f2d030&sv=2)

如你所见，位于“fade”标签外的字母将应用默认效果，而位于“{fade}”和“{/fade}”之间的部分将仅具有“fade”效果。

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 Unity 文本动画器 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying

**欢迎** 来到文档： **适用于 Unity 3.X 的文本动画器**！我们迫不及待想让你为文本添加动画并熟悉此插件。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F74b3Q86Y180EtGnu7Jg5%252FGames%2520Using%2520Text%2520Animator.png%3Falt%3Dmedia%26token%3D9912a72f-fab2-4003-b8f7-3499fc676a33&width=768&dpr=4&quality=100&sign=f97de3a1&sv=2)

我们正在编写尽可能简短且简明的文档，同时在你最需要时也能提供帮助。 **请务必阅读第一部分和最重要的页面**！现在花几分钟时间阅读，但将会节省 _大量时间_ 以后。

有用的链接：

[购买](https://www.textanimatorforgames.com/unity#pricing)
 [官网](https://www.textanimatorforgames.com/unity)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying#zhi-de-liao-jie-de-shi-xiang)

值得了解的事项

*   本文档提供多种语言版本: 英语、中文、韩语、日语。
    
*   你可以查看 此文档的不同版本和语言 在本页顶端。
    
*   文本动画器也可用于其他引擎. [在此了解更多](https://www.textanimatorforgames.com/)
    .
    

如果你随时需要帮助，欢迎访问 [故障排除页面](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu)
 （常见问题及其解决方法）或支持页面！

[![Logo](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fframerusercontent.com%2Fimages%2Fy1LCEnd5hyGjuX0kKaGBUorzMc.png&width=20&dpr=4&quality=100&sign=82d1be32&sv=2)Support Requests | Text Animator for Unity, Godot and Unrealwww.textanimatorforgames.com](https://www.textanimatorforgames.com/support)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying#xu-qiu)

需求

请在购买或导入前访问 [要求与限制](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/yao-qiu-yu-xian-zhi)
 页面，我们也在此回答了许多 [常见问题](https://docs.febucci.com/text-animator-unity/3.x-zh/huan-ying/faq)
。谢谢！

* * *

**玩得开心** 并别忘了加入我们的 [Discord](https://discord.com/invite/j4pySDa5rU)
 参与讨论并展示你的创作！

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 核心概念 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/he-xin-gai-nian

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/he-xin-gai-nian#xiao-guo)

效果

你可以在字符“生命”的不同阶段应用效果：

**出现**

![An example of the Appearance Effect {vertexp}](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FT7U4C8xOWPU5tjtdhxHT%2Fverticalexpandpreview.gif&width=300&dpr=4&quality=100&sign=2d90d0dc&sv=2)

用于仅在字母出现在屏幕上时对其进行动画处理。 _（更多…__)_

**持续**

![An example of the Behavior Effect <wiggle>](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FkXQFZNbm8mSv67m9nubS%2Fwigglepreviewfebucci.gif&width=300&dpr=4&quality=100&sign=1ff9ee43&sv=2)

用于在字符可见期间持续随时间对字母进行动画处理。

**消失**

![An example of the Disappearance Effect {#size}](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FsHBEkEs6y1POC6EOORwf%2Fdecreasing%2520size%2520text%2520animator%2520unity4.gif&width=300&dpr=4&quality=100&sign=89a11fab&sv=2)

用于当字母刚变为不可见时对其进行动画处理。

自 Text Animator 3.0 起，任何效果都可以在字母的任何阶段播放！（出现、持续或消失）

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/he-xin-gai-nian#hun-he-he-pi-pei-shu-zhi)

混合和匹配数值

即使你有“默认”效果和数值，也可以通过检查器或文本随时修改它们。

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/he-xin-gai-nian#she-zhi-ke-fang-wen-xing)

设置可访问性

Text Animator 使用许多不同的设置，从动画到打字机等。

在大多数情况下，你可以在三个不同的层级应用这些不同的设置：

*   **本地：** 设置绑定到该组件
    
*   **共享：** 设置存储在 ScriptableObject 中，并将在引用该 ScriptableObject 的其他实例之间共享。
    
*   **全局：** 这些设置要么会叠加应用于其他设置之上（例如在识别效果的情况下），要么将被用于 _仅_ 在未指定其他设置时（作为“回退”，例如曲线的情况）。
    

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/he-xin-gai-nian#shu-ju-ku)

数据库

Text Animator 使用 ScriptableObject 来存储有关 _存在什么_ 以及可被使用的内容，以及动画和打字机的构建模块（效果、等待时间、曲线等）。

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/he-xin-gai-nian#bian-ji-qi-ti-shi)

编辑器提示

你可以将鼠标悬停在检查器中的许多选项和字段上以显示一些提示和额外信息！

最后更新于1个月前

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 设置文本 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben

你可以通过两种不同的 UI 系统将文本设置到 Text Animator：

*   [设置文本](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#ui-toolkit)
    
*   [Text Mesh Pro](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#text-mesh-pro)
    

此页面包含一些已经出现在 [安装与快速上手](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/an-zhuang-yu-kuai-su-shang-shou)
中的信息，但也包含针对每个系统和一般情况的其他细节和建议。务必阅读 [设置文本](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#best-practices)
 一节！

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#ui-gong-ju-bao)

UI 工具包


-----------------------------------------------------------------------------------------------------------

_附注。假设你已经知道_ [_如何使用 UI Toolkit_](https://docs.unity3d.com/Documentation/Manual/UIElements.html)
 _以及它的功能。_

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#cong-ui-builder)

从 UI Builder

*   转到 库 -> 项目
    
*   拖动 "AnimatedLabel" 从你的层级视图中的 "Custom Controls/Febucci/Text Animator for Unity"！
    

我们正在努力确保你可以直接从 UI Toolkit 为内置的 Label 和 Button 添加动画！ _（Unity 6.3 及更高版本。）_ 保持更新！

你的 .uxml 应该看起来像这样：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZNwCUmAugxLNcVVO9oBk%252FScreenshot%25202025-11-15%2520alle%252018.02.51.png%3Falt%3Dmedia%26token%3Dced34791-d558-4883-b646-2197664dd637&width=768&dpr=4&quality=100&sign=c9df04c7&sv=2)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#tong-guo-dai-ma)

通过代码

你可以创建一个 "Febucci.TextAnimatorForUnity.AnimatedLabel" 类的实例并将其添加到你的 UI 文档，像这样：

复制

    using UnityEngine;
    using UnityEngine.UIElements;
    using Febucci.TextAnimatorForUnity; // <- 导入 Text Animator 的命名空间
    
    public class ExampleScript : MonoBehaviour
    { 
        [SerializeField] UIDocument document;
    
        void Start()
        {
            var container = document.rootVisualElement.contentContainer;
            var animatedLabel = new AnimatedLabel(); // <- 创建一个动画标签
            container.Add(animatedLabel); // <- 将其添加到内容容器中
            // [..]
            animatedLabel.SetText("<wave>hello"); // <- 设置文本
        }
    }

就是这些！！你已准备好进行 [如何添加特效](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao)

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#text-mesh-pro)

Text Mesh Pro


-----------------------------------------------------------------------------------------------------------------

_附注。假设你已经知道_ [_如何使用 Text Mesh Pro_](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html)
 _以及它如何工作。_

添加一个 Text Animator - Text Mesh Pro 组件到同一个具有 TextMeshPro 组件（无论是 UI 还是世界空间！）：

你的检查器应如下所示：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FT3h66pIPFdakGOCfToEY%252FScreenshot%25202025-11-15%2520alle%252017.59.18.png%3Falt%3Dmedia%26token%3D26196c49-f0f5-457b-85dd-da358f43c823&width=768&dpr=4&quality=100&sign=c0c53f3b&sv=2)

就是这些！！你已准备好进行 [如何添加特效](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao)

如果你看到空文本（但已在组件中设置），请确保至少点击过一次 TextMeshPro 组件并导入“Essentials”（当它们的窗口弹出并要求你这样做时）。

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#tong-guo-dai-ma-she-zhi-wen-ben-de-zui-jia-shi-jian)

通过代码设置文本的最佳实践

若要通过代码将文本设置到你的 TextMeshPro 对象，请引用 Text Animator 的脚本而不是 TMPro，例如：

复制

    using UnityEngine;
    using TMPro; 
    using Febucci.TextAnimatorForUnity.TextMeshPro; // <- 导入 Text Animator 的命名空间
    
    public class ExampleScript : MonoBehaviour
    {
        [SerializeField] TMP_Text textMeshPro;
        [SerializeField] TextAnimator_TMP textAnimator;
    
        void Start()
        {
            // 🚫 不要：通过 TMPro 设置文本
            textMeshPro.SetText("<wave>hello");
    
            // ✅ 应当：直接通过 Text Animator 设置文本
            textAnimator.SetText("<wave>hello");
        }
    
    }

附注：引用 TMPro 仍然可以工作，但使用 TextAnimator 设置文本集成得更好，因为我们对文本有更多控制。

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#zui-jia-shi-jian)

最佳实践


-----------------------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#zhi-she-zhi-zheng-ge-wen-ben-dui-hua-yi-ci)

只设置整个文本/对话一次

请尽量只设置文本一次，并使用打字机/可见性方法来控制其显示方式。

如果你确实需要在之后追加文本，可以使用 "textAnimator.AppendText" 方法。

示例[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#shi-li)

如果有一个角色说“Helloooo how are you doing?”，并且你想逐字显示，只需： `typewriter.ShowText("Hellooooo how are you doing?");` 就是这样！ [动态显示与隐藏字母](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu)

* * *

如果你在构建动态字符串，你仍然可以在将其值设置给打字机/动画器之前完成构建。

复制

    int apples = 5; // 稍后从游戏状态获取
    string playerName = "Bob";
    
    // 先构建整行对话
    string dialogue = $"Hello {playerName}, you've got {apples} apples";
    
    // 然后只设置一次文本
    typewriter.ShowText(dialogue);

（如果你使用对话系统，他们会为你处理这些 —— 不用担心！ [集成](https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng/ji-cheng-de-cha-jian-yu-dui-hua-xi-tong)
)

为什么我应该一次性设置整个文本，而不是逐字符设置？[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#wei-shen-me-wo-ying-gai-yi-ci-xing-she-zhi-zheng-ge-wen-ben-er-bu-shi-zhu-zi-fu-she-zhi)

性能！（即使你没有使用 Text Animator。）

每次设置文本时，TextMeshPro 或 UI 工具包都需要计算其网格、定位等，Text Animator 随后还必须重新计算字符持续时间等。这意味着如果你每秒多次更改它（例如不断添加字母），这些计算会每次都发生。

要逐个显示字符，你可以简单地先将完整文本设置一次，然后启动打字机： [动态显示与隐藏字母](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu)

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 安装与快速上手 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi

使用此资源只需几次点击（导入 -> 添加组件 -> 按播放），但为更好地理解所有内容，请查看以下页面，这样你可以更快且以正确的方向开始。

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi#how-to-implement-text-animator)

1\. 在 Unity 中导入 Text Animator


-----------------------------------------------------------------------------------------------------------------------------------------

第一步，你需要在项目中导入 Text Animator for Unity。

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi#jian-rong-xing-jian-cha)

兼容性检查

**该资源兼容以下用户界面和 Unity 版本**:

*   **Text Mesh Pro** （Unity 2022.3 及更高版本）
    
*   **UI 工具包** (Unity 6.3 及更高版本).
    

它同样支持新的 Unity 输入系统（也支持旧版输入系统）。

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi#dao-ru-bao)

导入包

一旦项目正确设置，你可以从包管理器（Asset Store 选项卡）导入 Text Animator。

确保包含 "Samples/BuiltIn" 文件夹，否则该资源可能无法正常工作。

安装成功后， **欢迎窗口** 将会弹出，Text Animator 已准备好为你的文本添加动画！

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252Fo6lFhmxUPaki6oAtVVXZ%252FScreenshot%25202025-11-15%2520alle%252017.40.31.png%3Falt%3Dmedia%26token%3D729acbd3-556d-4808-9726-7f3918afec84&width=768&dpr=4&quality=100&sign=5f8c0ce8&sv=2)

导入后显示的欢迎窗口的一部分

如果关于窗口未显示，或你想稍后查看，可以随时从菜单的 Tools/Febucci/TextAnimator/About Window 访问它！

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi#id-2.-shi-li-chang-jing)

2\. 示例场景


-------------------------------------------------------------------------------------------------------------

你可以直接从检视面板了解大多数 Text Animator 功能，并从示例场景中查看我们如何设置以及它们的直接效果。

从名为“**00 - Welcome**”的场景开始，或在 Text Animator 的欢迎窗口中点击“Get Started”。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FLgTsSXatpKk3M2Nr36VN%252FScreenshot%25202025-11-15%2520alle%252017.45.47.png%3Falt%3Dmedia%26token%3D624c13da-2e67-4653-9caa-076cc5cfa24d&width=768&dpr=4&quality=100&sign=2f467ea5&sv=2)

要访问示例场景，请确保你已经导入它们！当你不再需要时，也可以安全地移除/删除它们。

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi#animating-your-first-texts)

3\. 为你的首批文本添加动画


-----------------------------------------------------------------------------------------------------------------------

你可以在几次点击内让文本运行起来！

UI Toolkit

Text Mesh Pro

_附注。假设你已经知道_ [_如何使用 UI Toolkit_](https://docs.unity3d.com/Documentation/Manual/UIElements.html)
 _以及它的功能。_

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi#cong-ui-builder)

从 UI Builder

*   转到 库 -> 项目
    
*   拖动 "AnimatedLabel" 从你的层级视图中的 "Custom Controls/Febucci/Text Animator for Unity"！
    

我们正在努力确保你可以直接从 UI Toolkit 为内置的 Label 和 Button 添加动画！ _（Unity 6.3 及更高版本。）_ 保持更新！

你的 .uxml 应该看起来像这样：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZNwCUmAugxLNcVVO9oBk%252FScreenshot%25202025-11-15%2520alle%252018.02.51.png%3Falt%3Dmedia%26token%3Dced34791-d558-4883-b646-2197664dd637&width=768&dpr=4&quality=100&sign=c9df04c7&sv=2)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi#tong-guo-dai-ma)

通过代码

你可以创建一个 "Febucci.TextAnimatorForUnity.AnimatedLabel" 类的实例并将其添加到你的 UI 文档，像这样：

复制

    using UnityEngine;
    using UnityEngine.UIElements;
    using Febucci.TextAnimatorForUnity; // <- 导入 Text Animator 的命名空间
    
    public class ExampleScript : MonoBehaviour
    { 
        [SerializeField] UIDocument document;
    
        void Start()
        {
            var container = document.rootVisualElement.contentContainer;
            var animatedLabel = new AnimatedLabel(); // <- 创建一个动画标签
            container.Add(animatedLabel); // <- 将其添加到内容容器中
            // [..]
            animatedLabel.SetText("<wave>hello"); // <- 设置文本
        }
    }

_附注。假设你已经知道_ [_如何使用 Text Mesh Pro_](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html)
 _以及它如何工作。_

添加一个 Text Animator - Text Mesh Pro 组件到同一个具有 TextMeshPro 组件（无论是 UI 还是世界空间！）：

你的检查器应如下所示：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FT3h66pIPFdakGOCfToEY%252FScreenshot%25202025-11-15%2520alle%252017.59.18.png%3Falt%3Dmedia%26token%3D26196c49-f0f5-457b-85dd-da358f43c823&width=768&dpr=4&quality=100&sign=c0c53f3b&sv=2)

你可以阅读 [设置文本](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben)
 以获取更多细节和建议！

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi#id-1-writing-effects-in-your-text)

在文本中书写效果

在文本中添加效果的一种方式是使用富文本标签，例如：“`I'm <shake>freezing</shake>`”，其中 “shake” 是内置效果的 ID。

*   尝试通过实验以下标签来书写文本： `<wiggle>` `<shake>` `<wave>` `<bounce>`，例如“`<wiggle>I'm joking</wiggle> hehe now <shake>I'm scared</shake>`”，然后进入 Unity 的播放模式（Play）。
    

你的文本会根据你写的效果对字母进行动画处理！

* * *

祝你玩得开心！你可以继续下一页，深入了解该资源的所有功能。

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 动画器设置 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/dong-hua-qi-she-zhi

“动画器设置”（[无论是本地还是全局](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/he-xin-gai-nian#settings-accessibility)
）包含了关于效果如何应用和呈现的许多选项。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FfTe0N94riod0U2VKoRVi%252FScreenshot%25202025-11-15%2520alle%252018.39.36.png%3Falt%3Dmedia%26token%3D5e297e45-eb51-4eb9-9694-9c2028a893b8&width=768&dpr=4&quality=100&sign=4f128b8c&sv=2)

它们应该不言自明（我们在接下来的版本中也会添加更多工具提示！），但这里对某些选项提供一些额外说明：

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/dong-hua-qi-she-zhi#mo-ren-xiao-guo-mo-shi)

默认效果模式

如果你在下面的任一数组中至少设置了一个“默认标签”元素，“默认效果模式”可以让你决定这些标签如何应用到字母上。

*   **回退**：如果没有其它效果/标签已经影响该字母，这些标签将被应用
    
*   **常量**：这些标签将被应用到 _所有_ 文本（如果存在其它效果，它们会叠加在上面）
    

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/dong-hua-qi-she-zhi#shi-jian-suo-fang)

时间缩放

你可以在“文本动画器”组件检查器中更改效果 `时间缩放` 模式。

*   **缩放**：效果会根据游戏的 Time.timeScale 放慢/暂停（[Unity 参考](https://docs.unity3d.com/ScriptReference/Time-timeScale.html)
    )
    
*   **无缩放**：即使游戏暂停（Time.timeScale = 0），效果也会使用无缩放/独立时间继续更新。
    

如果你启用了打字机，其时间缩放 **将匹配相对的文本动画器的时间缩放** （这意味着如果你将其设置为“无缩放”，在游戏暂停时你也可以显示字母）。

如果游戏的时间缩放为负，文本动画器将表现得像已暂停，但一旦其大于零将自动恢复。

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/dong-hua-qi-she-zhi#dong-tai-suo-fang)

动态缩放

文本动画器默认在不同屏幕分辨率上实现一致的效果结果，建议你保持此功能启用。

说明[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/dong-hua-qi-she-zhi#shuo-ming)

你的玩家很可能有不同的屏幕尺寸（从移动设备到显示器等），这意味着将字母移动“50 像素”在某些设备上可能显得过多或过少，而作为设计者你希望每个人都能获得与你预期一致的统一体验/结果。因此我们强烈建议保持“使用动态缩放”启用，并基于你当前电脑的字体大小编辑数值（这样无论之后发生什么变化，它都会保持相同的统一比例）。

*   `参考字体大小`：表示对象按预期表现的尺寸。作为参考，你可以在测试时在 Unity 编辑器中选择字体大小。
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FK4qC74LIOHiJjZWbZrCl%2Ftextanimator%2520unity%2520dynamic%2520scaling.png&width=768&dpr=4&quality=100&sign=931e1049&sv=2)

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 特效数据库 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/te-xiao-shu-ju-ku

特效存储在数据库中，这些数据库本身也是 ScriptableObject。

你可以按任何你喜欢的方式向任意数据库添加或移除特效，并且可以让多个 TextAnimator 共享相同的数据库。默认情况下，所有 TextAnimator 将共享来自 [全局设置](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/quan-ju-she-zhi)
 文件的“默认”数据库。

**Text Animator 需要一个特效数据库来知道有哪些特效存在**, 所以请确保你有一个!

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FVSXvT5lT5dntsMxKpb75%252FScreenshot%25202025-11-15%2520alle%252020.15.21.png%3Falt%3Dmedia%26token%3D3b2e7fdd-86fb-4193-9b33-6312916accc6&width=768&dpr=4&quality=100&sign=e2c0f702&sv=2)

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/te-xiao-shu-ju-ku#chuang-jian-zi-ding-yi-shu-ju-ku)

创建自定义数据库

你可以在项目视图中右键 -> 创建 -> Text Animator for Unity，然后选择你想要添加的类别和特效，来创建新特效。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FyeZq580N8EGFfDW3tkwI%252FScreenshot%25202025-11-15%2520alle%252020.14.26.png%3Falt%3Dmedia%26token%3Df232bbae-c247-411f-ab0e-48bbc1ea1a42&width=768&dpr=4&quality=100&sign=6961bac3&sv=2)

由于你也可以在每个 ScriptableObject 中编辑特效标签，你可以为不同用途创建不同的特效，例如为需要传达“寒冷”的对话创建一个特定的“震动”特效，而为需要传达“恐惧”的情况创建另一个。

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 如何编辑特效 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao

您可以通过在项目窗口中单击其可脚本化对象来编辑任何效果。您将在编辑模式中（从 Unity 6.3 开始，其他版本将尽快支持）看到实时预览，显示效果如何应用于字母的不同阶段（出现、消失和持续）。

您也可以通过富文本标签来修改效果，使用 [修饰符](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu)
 （例如， **<wave s=2>** 使其速度加倍）。

* * *

重要的是您始终设置 **标签 ID**，否则该效果在数据库中将无法识别！

在检查器中您还会找到附加参数，可用于进一步修改效果，例如：

*   **烘焙曲线**：请保持开启！它会优化您的效果，尤其是在关键环境下（如果您有大量字母并且叠加了许多效果）。
    
*   **覆盖** [全局设置](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/quan-ju-she-zhi)
     使用自定义 [曲线](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/qu-xian)
     或 [回放](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/hui-fang)
     而非默认
    

同步持续时间仍在开发中！请告诉我们您的反馈！

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F6JMmtL11b32xG7FmgEv7%252FScreenshot%25202025-11-16%2520alle%252017.18.36.png%3Falt%3Dmedia%26token%3Db4a87c65-eb10-44be-864a-c27ceba45445&width=768&dpr=4&quality=100&sign=d0fae0f1&sv=2)

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 设置文本 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo

你可以通过两种不同的 UI 系统将文本设置到 Text Animator：

*   [设置文本](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#ui-toolkit)
    
*   [Text Mesh Pro](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#text-mesh-pro)
    

此页面包含一些已经出现在 [安装与快速上手](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/an-zhuang-yu-kuai-su-shang-shou)
中的信息，但也包含针对每个系统和一般情况的其他细节和建议。务必阅读 [设置文本](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben#best-practices)
 一节！

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo#ui-gong-ju-bao)

UI 工具包


-------------------------------------------------------------------------------------------

_附注。假设你已经知道_ [_如何使用 UI Toolkit_](https://docs.unity3d.com/Documentation/Manual/UIElements.html)
 _以及它的功能。_

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo#cong-ui-builder)

从 UI Builder

*   转到 库 -> 项目
    
*   拖动 "AnimatedLabel" 从你的层级视图中的 "Custom Controls/Febucci/Text Animator for Unity"！
    

我们正在努力确保你可以直接从 UI Toolkit 为内置的 Label 和 Button 添加动画！ _（Unity 6.3 及更高版本。）_ 保持更新！

你的 .uxml 应该看起来像这样：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZNwCUmAugxLNcVVO9oBk%252FScreenshot%25202025-11-15%2520alle%252018.02.51.png%3Falt%3Dmedia%26token%3Dced34791-d558-4883-b646-2197664dd637&width=768&dpr=4&quality=100&sign=c9df04c7&sv=2)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo#tong-guo-dai-ma)

通过代码

你可以创建一个 "Febucci.TextAnimatorForUnity.AnimatedLabel" 类的实例并将其添加到你的 UI 文档，像这样：

复制

    using UnityEngine;
    using UnityEngine.UIElements;
    using Febucci.TextAnimatorForUnity; // <- 导入 Text Animator 的命名空间
    
    public class ExampleScript : MonoBehaviour
    { 
        [SerializeField] UIDocument document;
    
        void Start()
        {
            var container = document.rootVisualElement.contentContainer;
            var animatedLabel = new AnimatedLabel(); // <- 创建一个动画标签
            container.Add(animatedLabel); // <- 将其添加到内容容器中
            // [..]
            animatedLabel.SetText("<wave>hello"); // <- 设置文本
        }
    }

就是这些！！你已准备好进行 [如何添加特效](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao)

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo#text-mesh-pro)

Text Mesh Pro


-------------------------------------------------------------------------------------------------

_附注。假设你已经知道_ [_如何使用 Text Mesh Pro_](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html)
 _以及它如何工作。_

添加一个 Text Animator - Text Mesh Pro 组件到同一个具有 TextMeshPro 组件（无论是 UI 还是世界空间！）：

你的检查器应如下所示：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FT3h66pIPFdakGOCfToEY%252FScreenshot%25202025-11-15%2520alle%252017.59.18.png%3Falt%3Dmedia%26token%3D26196c49-f0f5-457b-85dd-da358f43c823&width=768&dpr=4&quality=100&sign=c0c53f3b&sv=2)

就是这些！！你已准备好进行 [如何添加特效](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao)

如果你看到空文本（但已在组件中设置），请确保至少点击过一次 TextMeshPro 组件并导入“Essentials”（当它们的窗口弹出并要求你这样做时）。

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo#tong-guo-dai-ma-she-zhi-wen-ben-de-zui-jia-shi-jian)

通过代码设置文本的最佳实践

若要通过代码将文本设置到你的 TextMeshPro 对象，请引用 Text Animator 的脚本而不是 TMPro，例如：

复制

    using UnityEngine;
    using TMPro; 
    using Febucci.TextAnimatorForUnity.TextMeshPro; // <- 导入 Text Animator 的命名空间
    
    public class ExampleScript : MonoBehaviour
    {
        [SerializeField] TMP_Text textMeshPro;
        [SerializeField] TextAnimator_TMP textAnimator;
    
        void Start()
        {
            // 🚫 不要：通过 TMPro 设置文本
            textMeshPro.SetText("<wave>hello");
    
            // ✅ 应当：直接通过 Text Animator 设置文本
            textAnimator.SetText("<wave>hello");
        }
    
    }

附注：引用 TMPro 仍然可以工作，但使用 TextAnimator 设置文本集成得更好，因为我们对文本有更多控制。

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo#zui-jia-shi-jian)

最佳实践


-------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo#zhi-she-zhi-zheng-ge-wen-ben-dui-hua-yi-ci)

只设置整个文本/对话一次

请尽量只设置文本一次，并使用打字机/可见性方法来控制其显示方式。

如果你确实需要在之后追加文本，可以使用 "textAnimator.AppendText" 方法。

示例[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo#shi-li)

如果有一个角色说“Helloooo how are you doing?”，并且你想逐字显示，只需： `typewriter.ShowText("Hellooooo how are you doing?");` 就是这样！ [动态显示与隐藏字母](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu)

* * *

如果你在构建动态字符串，你仍然可以在将其值设置给打字机/动画器之前完成构建。

复制

    int apples = 5; // 稍后从游戏状态获取
    string playerName = "Bob";
    
    // 先构建整行对话
    string dialogue = $"Hello {playerName}, you've got {apples} apples";
    
    // 然后只设置一次文本
    typewriter.ShowText(dialogue);

（如果你使用对话系统，他们会为你处理这些 —— 不用担心！ [集成](https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng/ji-cheng-de-cha-jian-yu-dui-hua-xi-tong)
)

为什么我应该一次性设置整个文本，而不是逐字符设置？[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo#wei-shen-me-wo-ying-gai-yi-ci-xing-she-zhi-zheng-ge-wen-ben-er-bu-shi-zhu-zi-fu-she-zhi)

性能！（即使你没有使用 Text Animator。）

每次设置文本时，TextMeshPro 或 UI 工具包都需要计算其网格、定位等，Text Animator 随后还必须重新计算字符持续时间等。这意味着如果你每秒多次更改它（例如不断添加字母），这些计算会每次都发生。

要逐个显示字符，你可以简单地先将完整文本设置一次，然后启动打字机： [动态显示与隐藏字母](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu)

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 修饰符 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu

**修饰符** **让你单独更改效果的特性**，而无需为每个变体创建新的标签或可脚本对象。

`“我曾经<wiggle>强大</wiggle>……但现在我<wiggle a*3>强大了三倍</wiggle>!!!”`

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FgsZWN78ej2eibo2lDykv%2Ftext-animator-modifier-example-ezgif.com-video-to-gif-converter.gif&width=300&dpr=4&quality=100&sign=edced766&sv=2)

你可以在这里阅读每个效果可用修饰符的列表： [内置特效列表](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/nei-zhi-te-xiao-lie-biao)

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu#shu-zhi)

数值

要修改效果 **的数值** （例如振幅或速度），请在其 **信息写在效果标签本身内部**.

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu#cheng-yi)

乘以

格式： `<effectId` `**parameter*value**``>`

符号会告诉代码 `*****` 将 **乘以** **一个** 浮点参数与该值相乘 **通过这种方式，你可以轻松知道被修改的效果相比基础效果会强/弱多少**. （出于这个原因，值为“1”的修饰符将返回与基础值相同的结果） _示例_.

使[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu#shi)

*   一个“波动”效果的 `振幅` 变为三倍强： `<wave a*3>`
    
*   使“彩虹”效果变为两倍慢 `<rainb a*0.5>`
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FaYNNPUoPShZQvpOqk37r%252FMultiply_Example_TAnim3.0-ezgif.com-video-to-gif-converter.gif%3Falt%3Dmedia%26token%3D6c9e5567-9463-4a0f-8565-f99712390eb7&width=768&dpr=4&quality=100&sign=8e7a1f62&sv=2)

将摇摆增强五倍

符号会告诉代码 `*****` 符号仅适用于数字。对于字符串，请使用 `**=**`

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu#she-zhi)

设置

格式： `<effectId` `**parameter=value**``>`

符号会告诉代码 `**=**` 将 **来直接设置一个** 参数 **值**. 当你需要在文字区域获得精确的运动/结果，或设置字符串时特别有用。

示例：写入“<wave a=5>”等同于在检查器中将波动振幅设置为5！（当然好处是“<wave a=5>”修饰符只在你设置的文本区域内使用该值，关闭标签后会恢复为默认值。）

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu#guan-jian-ci)

关键词

**某些** 效果 **设置** 也可以通过一个 **单词**来修改，而无需在旁边写任何数值。

格式： `<effectId` `**关键字**``>`

示例：使用 [回放](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/hui-fang)
播放效果仅一次，写入 **<wave once>**

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu#ti-shi-yu-zui-jia-shi-jian)

提示与最佳实践

*   你可以在同一个效果标签上使用多个修饰符！
    

*   在检查器中，将你的效果设置为它们的“默认状态”/中性音调。 这样在你撰写对话时修改参数会更容易，而无需记住每个参数的精确数值。 一旦你设置好了一个中性的“抖动”，在写作时就会更容易知道“<shake a=2>”会使其变为两倍强（例如：用于让某人生气！）
    

另外：

*   👍 你也可以在声明“[默认/回退](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao#set-default-effects-to-the-entire-text)
    ”效果时使用修饰符（只需直接在检查器中写入）。
    
*   ❗ 确保在修饰符ID、“=”符号及其值之间不要留空格
    
    *   ❌ 错误： `<wiggle f = 3>`
        
    *   ✅ 正确： `<wiggle f=3>`
        
    
*   ⚠️ 如果在同一个富文本标签中写入相同的属性，只有最后一个会生效。
    
    写入“<wiggle `**a=2**` `**a=5**`\>”等同于写入“<wiggle `**a=5**`\>”，因为第一个“`**浮点参数与该值相乘**`”参数将被第二个丢弃/覆盖。
    

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 阶段 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/jie-duan

A "**phase**" 描述效果在字母之间如何变化。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252Fe0rVNhfYkoaST18lc2so%252FClipboard-20251116-152040-561.gif%3Falt%3Dmedia%26token%3Dae40450e-cf37-4859-9e27-7b05a986a44d&width=768&dpr=4&quality=100&sign=dde0be4d&sv=2)

你可以通过检视器或富文本标签来修改效果阶段 [modifiers](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu)
.

**字符偏移**

字母之间的时间变化

ModifierID

i

**单词偏移**

单词之间的时间变化

ModifierID

w

**速度**

效果速度（也影响 [回放](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/hui-fang)
)

ModifierID

s

富文本标签示例：

*   将效果加速两倍："<wave s=2\>"
    
*   修改偏移："<wave i=.1 w=.3\>"（将字符偏移设置为 0.1，单词偏移设置为 0.3）
    

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/jie-duan#guan-yu-pian-yi-deewai-shuo-ming)

关于偏移的额外说明

*   偏移为 0 或 1 表示所有字符上的效果相同
    
*   如果你从 0 变到 0.5，效果会向一个方向移动；而从 1 变到 0.5 则向相反方向移动（其中 0.5 更高）
    
*   偏移为 0.5 表示一个字符朝一个方向，另一个字符朝相反方向
    

* * *

最后更新于1个月前

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 曲线 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/qu-xian

效果根据“状态 **曲线”**，你可以在检视器中分配它们。

一如既往， **曲线** 是一个可脚本化对象，点击它们会在检视器中显示其预览。

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/qu-xian#nei-zhi-qu-xian)

内置曲线

**正弦**

遵循正弦曲线（并在出现时逐渐缓入）

**线性**

线性地从0到1变化

**保持**

始终保持在1

**方波**

要么是1要么是-1

**步进**

以四个不同的步骤从0变到1

**弹跳**

从0弹跳到1

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/qu-xian#cong-jian-shi-qi-chuang-jian-zi-ding-yi-qu-xian)

从检视器创建自定义曲线

要从检视器创建自定义曲线，请转到 Project->Create->Text Animator for Unity 然后选择“**自定义**".

你会在检视器中看到两个曲线，它们都可以在面板底部编辑。

*   **Curve01** 从0到1并决定出现和消失的行为
    
*   **CurveRange** 从-1到1（但在起点结束以形成平滑/无缝循环）并影响持久性效果
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZv0T9uTeTrdB1zcBiDNj%252FScreenshot%25202025-11-16%2520alle%252017.04.29.png%3Falt%3Dmedia%26token%3Dd2fc3da7-4456-4cd3-b724-ecf9910219a4&width=768&dpr=4&quality=100&sign=a5caded4&sv=2)

* * *

通过以下方式设置曲线的一种方法 [修饰符](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu)
 （类似于 [回放](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/hui-fang)
）将在未来版本中推出！

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 内置特效列表 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/nei-zhi-te-xiao-lie-biao

这是我们创建的默认/内置数据库，已经可用（导入“Samples”文件夹！）并包含许多可在游戏中使用的效果。

你也可以随时创建自己的效果！

*   [创建您自己的特效](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/chuang-jian-nin-zi-ji-de-te-xiao)
    
*   [编写自定义特效（C#）](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-te-xiao-c)
    

自 Text Animator for Unity 3.0 起，任何效果都可以作为出现、持续和消失来播放，并且你也可以让它们只播放一次或基于其他条件播放！

你也可以使用 [修饰符](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu)
，允许你单独更改行为效果的特性。

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2F577I8LcLJl1quOreidHC%2Fpendulumpreview.gif&width=490&dpr=4&quality=100&sign=fa9422c7&sv=2)

**钟摆**

标签

pend

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FewfXieMBJaRjEcihXyeT%2Fdanglepreview.gif&width=490&dpr=4&quality=100&sign=d00e4c63&sv=2)

**摆动（下垂）**

标签

dangle

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2Fd0wCTrvN7t49jUBGNqI0%2Ffadepreview.gif&width=490&dpr=4&quality=100&sign=4a33090f&sv=2)

**淡入淡出**

标签

fade

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FCbIcUivK6TUlvvPHQx9l%2Frainbowpreviewfebucci.gif&width=490&dpr=4&quality=100&sign=fa7368ab&sv=2)

**彩虹**

标签

rainb

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FONRSbf0b6oeC6tUYL7Ef%2Frotatingpreviewfebucci.gif&width=490&dpr=4&quality=100&sign=2bfdc2cd&sv=2)

**旋转**

标签

rot

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2Fbhm0HLqRADQj3RCVHUN2%2Fbouncepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=ba59014d&sv=2)

**反弹**

标签

bounce

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FImNDiBy3MuZpT1fB0UxF%2Fslidepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=5c1b22c2&sv=2)

**滑动**

标签

slideh

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2F9zSq1hqy61sKFcWpOxNI%2Fswingpreviewfebucci.gif&width=490&dpr=4&quality=100&sign=dec9d5f5&sv=2)

**摆动**

标签

swing

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FaZftI1kdTYBEZedse6qJ%2Fwavepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=9cb0fc71&sv=2)

**波动**

标签

wave

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FT3x704G3ZSzv4Hi4h4jA%2Fsizepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=8e27b570&sv=2)

**放大**

标签

incr

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2F21sLOk7GG8dv7I0XaGMO%2Fshakepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=13725beb&sv=2)

**抖动**

标签

shake

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2Fcwposy2qWMvqTWq81T5e%2Fwigglepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=b01cd84b&sv=2)

**摆动（微幅）**

标签

wiggle

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/nei-zhi-te-xiao-lie-biao#shu-yu-biao)

术语表

修饰符 ID

修饰符数值

名称

换句话说

a

浮点数，例如：3

振幅

效果强度

s

浮点数，例如：3

速度

速度

*   `标签`：代表效果标签，在其类别中是唯一的（例如 <shake>）
    

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 回放 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/hui-fang

**播放方式决定效果如何随时间应用** （例如，仅播放一次效果）。

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/hui-fang#nei-zhi-bo-fang-fang-shi)

内置播放方式

您可以使用以下内置播放方式来修改您的效果，或者 [从检查器分配它们](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao)
 或将它们设置为 [修饰符关键字：](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu#keywords)

例如，如果您已经有一个无限循环的“wave”效果，但在某种情况下您只想显示一次，您可以写“<wave **一次**\>”，其中“once”是该播放方式的 ID。

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/hui-fang#chuang-jian-zi-ding-yi-bo-fang-fang-shi)

创建自定义播放方式

目前有三种不同类型的播放类可以实例化：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252Fzupt163LqdAGyPlxMi76%252FScreenshot%25202025-11-15%2520alle%252019.55.30.png%3Falt%3Dmedia%26token%3Dda819d95-9fd3-4d59-aa59-33d5a98b9717&width=768&dpr=4&quality=100&sign=6b1d3e31&sv=2)

每种播放方式都有不同的参数可供修改（例如持续时间）。

如果任何参数小于或等于 0，则该参数将被忽略，动画引擎将跳到“下一个”重要/相关参数。

出现和消失至少需要一个值大于 0，否则它们的持续时间将无效并被跳过

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/hui-fang#jian-dan)

简单

参数

描述

开始前延迟

动画引擎在开始显示效果前等待的时间

淡入/淡出时长

效果从 0 到 1 所需的时间

静止时长

效果在屏幕上显示的时间。

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/hui-fang#jia-quan)

加权

参数

描述

强度01

允许您从外部控制效果应有的强度（例如靠近游戏目标时为 1，太远时为 0）

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/hui-fang#xun-huan)

循环

参数

描述

开始前延迟

动画引擎在开始显示效果前等待的时间

淡入时长

效果从 0 到 1 所需的时间

静止时长

效果在屏幕上显示的时间。

淡出时长

效果从 1 到 0 所需的时间

循环次数

此循环重复的次数

循环间延迟

开始新循环前要等待的时间

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/hui-fang#bo-fang-fang-shi-shu-ju-ku)

播放方式数据库

像往常一样，您可以将播放方式存储在一个 **数据库** 并将其分配给 [全局设置](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/quan-ju-she-zhi)
 （顺便说一下，已经有一个内置并已设置），例如如下：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FGjKdZv4dnZ0IOL9ZOFW2%252FScreenshot%25202025-11-15%2520alle%252019.59.17.png%3Falt%3Dmedia%26token%3Dc03fbe88-b9cc-4d41-8d98-dd8c4ee8e92b&width=768&dpr=4&quality=100&sign=ba4fe8b4&sv=2)

这样您就可以从所有不同的文本动画组件访问所有播放方式，并通过单独修改来调整您的效果 [修饰符](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu)
 （例如“`<wave once>`".

如果您正在创建新的播放方式，请确保将其存储在该主/全局数据库中

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 动态显示与隐藏字母 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu

**你可以使用打字机动态显示和隐藏字母**, 为不同类型的字符（标点符号、字母、\[…\]）选择不同的停顿时间、触发事件等。

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#showing-text)

显示文本


---------------------------------------------------------------------------------------------------------------------------

该 打字机包含通用设置和事件监听器，并允许不同的停顿/计时模式：

*   **按字符**：一个字母接着一个字母显示。
    
*   **按单词**：按单词逐步显示文本。
    

这种新架构（从 3.0 开始）允许你在开发过程中更改打字机计时（无论出于何种原因），同时保持事件引用和设置不变！<3

**你的打字机应该看起来像这样：**

Text Mesh Pro

UI 工具包

在检视器中的 TypewriterComponent：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F4QBGWiDjjQq4LXVbhNfc%252FScreenshot%25202025-11-16%2520alle%252018.15.08.png%3Falt%3Dmedia%26token%3Daeb76665-1ea4-498e-9181-091ddf322063&width=768&dpr=4&quality=100&sign=11c67ddd&sv=2)

在 UI Builder 中的 AnimatedLabel：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FB0i06unOYgu5XhHcdZN8%252FScreenshot%25202025-11-16%2520alle%252018.21.21.png%3Falt%3Dmedia%26token%3Dcf1193d6-cac5-47d4-93bf-b89a121f0046&width=768&dpr=4&quality=100&sign=be88101a&sv=2)

确保分配了计时的 Scriptable Object，否则打字机会立即显示整个文本！

* * *

你可以通过两种主要方式启动打字机：

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#a-via-code-recommended)

A) 通过代码（推荐）

如果你想使用打字机， **建议通过代码将文本直接设置到该组件上。**

Text Mesh Pro

UI 工具包

如果你使用 TextMeshPro，请替换引用 TMPro 或 Text Animator 的脚本（[设置文本](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben)
）并引用 `Febucci.TextAnimatorForUnity.TypewriterComponent` 来代替。

*   ❌ 不要：“`tmproText.text = textValue;`” ，或 "`textAnimator.SetText(textValue);`"
    
*   ✅ 请使用： `typewriter.ShowText(textValue);`
    

通过 UI Toolkit， `AnimatedLabel` 已经有一个 “`Typewriter`” 值可以与之交互！ 你不需要做其他事情，只要确保你已分配了打字机的延迟即可。

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#b-via-the-easy-integration)

B) 自动识别

如果你没有遵循上述步骤，当你添加了 Typewriter 组件或通过 UI Toolkit 的 AnimatedLabel 设置了 “Timings” 时，TextAnimator 仍会尝试自动启动打字机。

简单集成可能会滞后一帧（因为它必须先发现某些内容已更改，这通常在上一帧完成），然后再启动打字机）。如果这是个问题，请遵循步骤 [A) 通过代码（推荐）](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#a-via-code-recommended)
，或参见 [故障排除](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#when-i-set-the-text-i-see-the-previous-one-for-one-frame-before-showing-the-new-one)

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#kong-zhi-zi-mu)

控制字母


-----------------------------------------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#start-and-stop-typing)

开始与停止打字

在组件的检视器内你会找到一些选项来控制打字机何时开始触发：

*   `启动打字机模式`：告诉打字机何时开始显示字母。
    

值

说明

**仅通过脚本**

打字机只能通过调用来启动 [TextAnimatorPlayer.StartShowingText()](https://www.api.febucci.com/tools/text-animator-unity/api/Febucci.UI.Core.TypewriterCore.html#Febucci_UI_Core_TypewriterCore_ShowText_System_String_)

**OnEnable**

每当 gameObject 被设置为激活时，打字机会启动

**OnShowText**

一旦设置了新文本，打字机就会开始（[如“显示文本”部分所述](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#showing-text)
)

**自动来自所有事件**

以上所有

*   `在启动时重置打字速度`：如果为 true，则每次显示新文本时打字机速度将重置为 1，否则会保存上次使用的速度。
    

你可以随时通过调用来暂停打字机 `typewriter.StopShowingText()`，并且可以通过调用来开始/继续它 `typewriter.StartShowingText()`.

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#skip)

跳过整个文本

要跳过整个打字效果，你可以调用 `typewriter.SkipTypewriter()` 方法。

你还可以找到一些选项来控制其行为：

*   `在跳过时隐藏出现效果`：如果为 true，则在打字机跳过时会阻止出现效果播放（即文本将立即显示）。
    
*   `在跳过时触发事件`：如果为 true，则一旦打字机跳过将触发所有剩余事件（如果你使用这些事件运行某些游戏逻辑要小心，因为所有事件会同时执行）。在此处阅读有关事件的更多信息： [在打字时触发事件](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-chu-fa-shi-jian)
    

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#tiao-guo-wen-ben-de-te-ding-bu-fen)

跳过文本的特定部分

该功能正在为 3.0 版测试，并将在下一个版本很快恢复！感谢你的理解！

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#hiding-text)

隐藏文本

你可以通过脚本动态隐藏字母，调用 `typewriter.StartDisappearingText()`，并且你也可以随时通过调用来停止它 `typewriter.StopDisappearingText()`.

* * *

你可以创建你自己的计时等待（阅读 [这里](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-da-zi-deng-dai-c)
 通过 C# 的方法）或者你可以使用内置的等待。

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#options)

选项


--------------------------------------------------------------------------------------------------------------------

打字机可能共用相同的设置也可能有各自特定的设置，因此请确保在检视器中将鼠标悬停在其字段上以显示每个字段的工具提示。

下面是最重要/常见设置的快速概述：

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#callbacks-unity-events)

回调（Unity 事件）

你可以使用基于打字机活动触发的 Unity 事件（例如：当它刚结束显示文本时）。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FWHU9EqhOj3uN5AI3PURA%252FScreenshot%25202025-11-16%2520alle%252018.34.38.png%3Falt%3Dmedia%26token%3D7757f0b7-300e-4637-8495-542fab1b0fe5&width=768&dpr=4&quality=100&sign=8573985c&sv=2)

事件

说明

`OnTextShowed`

在整个文本显示完后调用的事件（_如果你将“使用打字机”设置为 true，它将等到所有字母都显示完毕_)

`OnTextDisappeared`

一旦脚本开始隐藏最后一个字母即调用

下面的功能只有在“**使用打字机**” 设置为 **true**:

事件

说明

`OnTypewriterStart`

在打字机开始显示第一个字母之前立即调用。 如果打字机关闭则不会起作用，因为那种情况下它会与 “OnTextShowed” 事件同时发生 _（在这种情况下你可以改用那个事件）_

`OnCharacterVisible(Char)`

每当一个字符变为可见时调用

`OnMessage(EventMarker)`

每当打字机在文本中遇到消息/事件时调用。阅读有关事件的更多内容 [这里](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-chu-fa-shi-jian)

打字机使用其链接的文本动画器 **时间缩放** 来推进时间（你可以在此处阅读更多： [动画器设置](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/dong-hua-qi-she-zhi#time-scale)
），这意味着如果时间设置为“未缩放”，即使你的游戏暂停，打字机也会继续进行。

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 在打字时等待动作 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-deng-dai-dong-zuo

**当打字机到达文本中的特定位置时，你可以执行动作**. _因此，只有在打字机启用时动作才会生效。_

示例：等待 X 秒或等待玩家输入。

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-deng-dai-dong-zuo#how-to-add-actions-in-your-text)

如何在文本中添加动作


------------------------------------------------------------------------------------------------------------------------------------------------

你可以通过使用富文本标签在文本中添加动作。

动作的格式遵循此格式：“`<actionID>`” 或 “`<actionID=attribute1,attribute2,...>`” 用于可选的参数/属性（就像事件/消息一样）。

动作标签不区分大小写， `<waitfor>` 和 `<waitFor>` 将产生相同的结果。

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-deng-dai-dong-zuo#parameters)

参数

动作支持多个参数，在 ‘`=`’ 符号之后并由 `逗号`.

示例： `<waitfor=1.5>` 或 `<playaudio=tada,laugh,dub>`

*   ⚠️ 浮点数必须使用 `句点`, 不能使用 `逗号`.
    
    *   ✔️ <speed=0.5>
        
    *   ❌ <speed=0,5>
        
    

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-deng-dai-dong-zuo#databases)

数据库


-------------------------------------------------------------------------------------------------------------------

与效果类似，你会在数据库中找到动作。你可以根据需要添加或移除任意多个，创建特定动作并且也可以 [通过 C# 编程自定义你自己的](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-dong-zuo-c)
.

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-deng-dai-dong-zuo#built-in-actions)

内置的一个


----------------------------------------------------------------------------------------------------------------------------

你可以在文本中使用以下内置动作。

**等待秒数**

在继续显示文本之前等待 X 秒

标签

waitfor

属性

浮点数（等待时长）

示例

<waitfor=3>

**等待输入**

等待玩家输入

标签

waitinput

属性

不适用

示例

<waitinput>

**速度**

乘以打字机速度

标签

speed

属性

浮点数（速度倍数）

示例

<speed =2>

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-deng-dai-dong-zuo#zu-jian-dong-zuo)

组件动作


---------------------------------------------------------------------------------------------------------------------------

某些动作仅在场景中存在时可用（你需要将它们作为组件创建）。

**播放声音**

播放（检查器中引用的）音频源并等待其播放完毕

标签

psound

属性

不适用

示例

<psound>

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-deng-dai-dong-zuo#ben-di-dong-zuo)

本地动作

你可以将动作设为 _本地的_, 这意味着只有当你在打字机组件旁创建它们时才会被识别。（仅适用于 **TextMeshPro**)

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FclEP11Pk4aO6mj1dRttg%252FScreenshot%25202025-11-17%2520alle%252015.33.37.png%3Falt%3Dmedia%26token%3D9370c9b0-eb9c-4408-8d7c-da316d4a77c1&width=768&dpr=4&quality=100&sign=2472d944&sv=2)

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-deng-dai-dong-zuo#quan-ju-dong-zuo)

全局动作

只要你将 “使全局可用” 设置为开启，全局动作就可以被场景中任何正在打字的打字机访问。

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 在字母显示时播放声音 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-zi-mu-xian-shi-shi-bo-fang-sheng-yin

要在游戏中实现打字机音效，你可以订阅 Typewriter 的 “`OnCharacterVisible`” 事件并根据该事件播放音效。

该事件会传递一个 “char” 作为参数，因此你也可以根据不同的字母播放不同的音效。

_附注：该事件在遇到空格时也会触发，因此请根据你偏好的字符类型来决定是否播放音效。_

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-zi-mu-xian-shi-shi-bo-fang-sheng-yin#example-package)

示例包

例如，你可以安装位于 “Extra” 文件夹中的 “TypeWriter Sounds” 包并查看其实现。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FLld5xGmtqLsHHY6U2x1g%2FUntitled.png&width=768&dpr=4&quality=100&sign=6f3e25a8&sv=2)

变量

说明

`来源`

播放音效的主要音频源

`MinSoundDelay`

在播放下一个音效之前必须经过的最短时间

`中断之前的音效`

如果为真，之前的音频将被停止

`随机序列`

如果为真，下一个要播放的音频剪辑将从 “Sounds” 数组中随机选择。如果为假，音效将按顺序播放

`音效`

要播放的打字机音效

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 在打字时触发事件 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-chu-fa-shi-jian

事件是特殊标签，允许你在打字机到达文本的特定部分时将消息（字符串）发送到任何侦听脚本。 _（因此，事件仅在打字机启用时生效）_

![textanimatorgif2febucci](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2F3UxVpaMvfQpqNMeoWA2v%2Ftextanimatorgif2febucci.gif&width=768&dpr=4&quality=100&sign=b35a2221&sv=2)

场景“示例 3 - 事件”

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-chu-fa-shi-jian#overview)

概述


---------------------------------------------------------------------------------------------------------------

你可以通过使用富文本标签在文本中编写事件。

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-chu-fa-shi-jian#formatting)

格式

事件的消息以问号开头，如下所示： `<?eventMessage>`.

**示例：** 要调用名为“shakeCamera”的事件，写： `<?shakeCamera>`

*   👍🏻 事件可以使用任何类型的标签，包括内置效果的标签。
    
*   ⚠️ 事件区分大小写。写下 `<?camshake>` 与写下 `<?camShake>`是不一样的。小心！（或者在你的脚本中使用 `string.ToLower()` 方法来处理这个问题。）
    

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-chu-fa-shi-jian#parameters)

参数

事件可以有一个或多个参数（第一个参数之前使用 `=` 符号，然后用逗号分隔其他参数 `,`），以便你向脚本发送多个数据。

*   一个参数： `<?eventID=parameter1>`，将产生消息“eventID”和一个参数“parameter1”。
    
*   多个参数： `<?eventID=p1,p2>`，将产生消息“eventID”和参数“p1”和“p2”。
    

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-chu-fa-shi-jian#listening-to-events)

监听事件


----------------------------------------------------------------------------------------------------------------------------

想要监听事件/消息的脚本必须订阅 `onMessage` 回调到 `Typewriter` 类中。（[脚本 API](https://www.api.febucci.com/tools/text-animator-unity/api/Febucci.UI.Core.TypewriterCore.html#Febucci_UI_Core_TypewriterCore_onMessage)
).

示例：

复制

    // 在你的脚本内
    [SerializeField] TypewriterComponent typewriter;
    
    // 添加和移除回调
    void OnEnable() => typewriter.onMessage.AddListener(OnMessage);
    void OnDisable() => typewriter.onMessage.RemoveListener(OnMessage);
    
    // 根据接收到的标记执行操作
    void OnMessage(EventMarker marker)
    {
        switch (marker.name)
        {
            // 一旦打字机遇到 "<?something>" 标签
            
            case "something":
                // 做某事
                break;
        }
    }

👍🏻 注意“message”字符串不包含‘<’、‘?’ 和 ‘>’ 字符，只包含消息本身。

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 样式 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/yang-shi

样式可快速将文本的部分内容替换为其他内容，例如用于创建效果组合、打字机动作和事件，否则这些会因重复标签而需要大量输入。

如果你使用的是 TMPro，请使用 Text Animator Styles 而不是 TMPro 的样式，因为后者（TMPro 的）无法识别 Text Animator 标签，会导致这些标签被添加到文本中。

* * *

只需打开你选择的样式表脚本对象（你可以在项目文件夹通过 创建 菜单 -> Text Animator -> StyleSheet 创建一个），然后开始添加/编辑标签。

你可以拥有一个全局样式表（ [全局设置](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/quan-ju-she-zhi)
 ）也可以有一个本地样式表。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FbEgcnrQ9RzsRjy1jCb7m%2Ftextanimator%2520settings%2520stylesheet%2520example.png&width=768&dpr=4&quality=100&sign=f266ed25&sv=2)

从上面的示例来看，每当你在文本中写入样式标签“`<style1>`”时，它将被替换为“`<wave><play=5><rainb><shake>`”——并用“`</style1>`”来关闭时，会被替换为“`</wave></rainb></shake><?ended>`”。

样式标签不区分大小写（写 "<style1>" 和 "<Style1>" 会产生相同的结果）。

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 直接特效 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/chuang-jian-nin-zi-ji-de-te-xiao/zhi-jie-te-xiao

**直接效果会修改字母的可视属性**，无论是它们的位置、颜色、缩放等。

**颜色**

修改字母的颜色，允许你决定是否仅影响透明度（alpha）、RGB 或两者都影响。

**连续旋转**

修改字符的旋转，从来回摆动到进行完整的循环旋转

**缩放**

乘以字符的缩放。缩放为1将不会有任何变化！

**位置**

随时间改变字符的位置。也允许使用三维（Z 轴位置）

**剪切**

从不同的枢轴点扭曲（或“倾斜”）字符。

**扩展**

从不同方向扩展字符的侧面。

**随机位置**

将字符向运行时生成的随机方向移动。

**彩虹**

将字符的颜色更改为彩虹效果，随时间循环变化。

该 [内置效果](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/nei-zhi-te-xiao-lie-biao)
 你在资源中发现的这些是这些直接效果的混合！我们决定将“随机位置”效果称为“**摇摆**" ”并将其放在默认文件夹中，以及“抖动”效果（它是具有不同 [曲线](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/qu-xian)
）的摇摆，但归根结底由你决定！玩得开心！！ 🎉

最后更新于1个月前

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 创建您自己的特效 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/chuang-jian-nin-zi-ji-de-te-xiao

在 Unity 的 TextAnimator 中，你可以通过多种不同方式创建自定义效果。

*   [创建您自己的特效](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/chuang-jian-nin-zi-ji-de-te-xiao#creating-effects-from-the-inspector)
    
*   [编写自定义特效（C#）](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-te-xiao-c)
    

随意选择最适合你的方法！

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/chuang-jian-nin-zi-ji-de-te-xiao#recommendations)

建议

正如你会随着深入使用 Text Animator 所发现的，借助自定义效果、曲线和回放，你能够创造出相当强大的组合和结果！话虽如此（就像任何强大的东西一样） **由你来明智地使用它们**！理论上你可以在每个效果内创建无限引用的动画，导致堆栈溢出，或者创建对目标硬件要求过高的顶点级动画，如果屏幕上有太多文本/动画就会导致帧率下降，因此请注意不要过度使用！

话虽如此……玩得开心！

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/chuang-jian-nin-zi-ji-de-te-xiao#cong-jian-shi-qi-chuang-jian-xiao-guo)

从检视器创建效果


-------------------------------------------------------------------------------------------------------------------------------------------------------

除了已有的内置效果， **你可以直接从检视器创建自己的效果（无需编写任何代码）**.

附注：如果你 _确实_ 想通过 C# 编写自定义效果，请查看 [编写自定义特效（C#）](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-te-xiao-c)

像往常一样，要创建自定义效果，请前往 Project 窗口 -> Create -> Text Animator for Unity，然后从“Effects”菜单中选择任意项。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F0e1f9LNQxJvGr7X9eaKJ%252FScreenshot%25202025-11-16%2520alle%252018.45.04.png%3Falt%3Dmedia%26token%3Db64409f1-f23d-4242-b7ca-2b74890cdf6e&width=768&dpr=4&quality=100&sign=82d141cd&sv=2)

你可以从检视器创建两种不同类型的效果：

*   [直接特效](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/chuang-jian-nin-zi-ji-de-te-xiao/zhi-jie-te-xiao)
    
*   [曲线特效](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/chuang-jian-nin-zi-ji-de-te-xiao/qu-xian-te-xiao)
    

最后更新于1个月前

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 动态显示与隐藏字母 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji

**你可以使用打字机动态显示和隐藏字母**, 为不同类型的字符（标点符号、字母、\[…\]）选择不同的停顿时间、触发事件等。

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji#showing-text)

显示文本


---------------------------------------------------------------------------------------

该 打字机包含通用设置和事件监听器，并允许不同的停顿/计时模式：

*   **按字符**：一个字母接着一个字母显示。
    
*   **按单词**：按单词逐步显示文本。
    

这种新架构（从 3.0 开始）允许你在开发过程中更改打字机计时（无论出于何种原因），同时保持事件引用和设置不变！<3

**你的打字机应该看起来像这样：**

Text Mesh Pro

UI 工具包

在检视器中的 TypewriterComponent：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F4QBGWiDjjQq4LXVbhNfc%252FScreenshot%25202025-11-16%2520alle%252018.15.08.png%3Falt%3Dmedia%26token%3Daeb76665-1ea4-498e-9181-091ddf322063&width=768&dpr=4&quality=100&sign=11c67ddd&sv=2)

在 UI Builder 中的 AnimatedLabel：

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FB0i06unOYgu5XhHcdZN8%252FScreenshot%25202025-11-16%2520alle%252018.21.21.png%3Falt%3Dmedia%26token%3Dcf1193d6-cac5-47d4-93bf-b89a121f0046&width=768&dpr=4&quality=100&sign=be88101a&sv=2)

确保分配了计时的 Scriptable Object，否则打字机会立即显示整个文本！

* * *

你可以通过两种主要方式启动打字机：

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji#a-via-code-recommended)

A) 通过代码（推荐）

如果你想使用打字机， **建议通过代码将文本直接设置到该组件上。**

Text Mesh Pro

UI 工具包

如果你使用 TextMeshPro，请替换引用 TMPro 或 Text Animator 的脚本（[设置文本](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben)
）并引用 `Febucci.TextAnimatorForUnity.TypewriterComponent` 来代替。

*   ❌ 不要：“`tmproText.text = textValue;`” ，或 "`textAnimator.SetText(textValue);`"
    
*   ✅ 请使用： `typewriter.ShowText(textValue);`
    

通过 UI Toolkit， `AnimatedLabel` 已经有一个 “`Typewriter`” 值可以与之交互！ 你不需要做其他事情，只要确保你已分配了打字机的延迟即可。

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji#b-via-the-easy-integration)

B) 自动识别

如果你没有遵循上述步骤，当你添加了 Typewriter 组件或通过 UI Toolkit 的 AnimatedLabel 设置了 “Timings” 时，TextAnimator 仍会尝试自动启动打字机。

简单集成可能会滞后一帧（因为它必须先发现某些内容已更改，这通常在上一帧完成），然后再启动打字机）。如果这是个问题，请遵循步骤 [A) 通过代码（推荐）](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#a-via-code-recommended)
，或参见 [故障排除](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#when-i-set-the-text-i-see-the-previous-one-for-one-frame-before-showing-the-new-one)

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji#kong-zhi-zi-mu)

控制字母


-----------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji#start-and-stop-typing)

开始与停止打字

在组件的检视器内你会找到一些选项来控制打字机何时开始触发：

*   `启动打字机模式`：告诉打字机何时开始显示字母。
    

值

说明

**仅通过脚本**

打字机只能通过调用来启动 [TextAnimatorPlayer.StartShowingText()](https://www.api.febucci.com/tools/text-animator-unity/api/Febucci.UI.Core.TypewriterCore.html#Febucci_UI_Core_TypewriterCore_ShowText_System_String_)

**OnEnable**

每当 gameObject 被设置为激活时，打字机会启动

**OnShowText**

一旦设置了新文本，打字机就会开始（[如“显示文本”部分所述](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu#showing-text)
)

**自动来自所有事件**

以上所有

*   `在启动时重置打字速度`：如果为 true，则每次显示新文本时打字机速度将重置为 1，否则会保存上次使用的速度。
    

你可以随时通过调用来暂停打字机 `typewriter.StopShowingText()`，并且可以通过调用来开始/继续它 `typewriter.StartShowingText()`.

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji#skip)

跳过整个文本

要跳过整个打字效果，你可以调用 `typewriter.SkipTypewriter()` 方法。

你还可以找到一些选项来控制其行为：

*   `在跳过时隐藏出现效果`：如果为 true，则在打字机跳过时会阻止出现效果播放（即文本将立即显示）。
    
*   `在跳过时触发事件`：如果为 true，则一旦打字机跳过将触发所有剩余事件（如果你使用这些事件运行某些游戏逻辑要小心，因为所有事件会同时执行）。在此处阅读有关事件的更多信息： [在打字时触发事件](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-chu-fa-shi-jian)
    

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji#tiao-guo-wen-ben-de-te-ding-bu-fen)

跳过文本的特定部分

该功能正在为 3.0 版测试，并将在下一个版本很快恢复！感谢你的理解！

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji#hiding-text)

隐藏文本

你可以通过脚本动态隐藏字母，调用 `typewriter.StartDisappearingText()`，并且你也可以随时通过调用来停止它 `typewriter.StopDisappearingText()`.

* * *

你可以创建你自己的计时等待（阅读 [这里](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-da-zi-deng-dai-c)
 通过 C# 的方法）或者你可以使用内置的等待。

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji#options)

选项


--------------------------------------------------------------------------------

打字机可能共用相同的设置也可能有各自特定的设置，因此请确保在检视器中将鼠标悬停在其字段上以显示每个字段的工具提示。

下面是最重要/常见设置的快速概述：

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji#callbacks-unity-events)

回调（Unity 事件）

你可以使用基于打字机活动触发的 Unity 事件（例如：当它刚结束显示文本时）。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FWHU9EqhOj3uN5AI3PURA%252FScreenshot%25202025-11-16%2520alle%252018.34.38.png%3Falt%3Dmedia%26token%3D7757f0b7-300e-4637-8495-542fab1b0fe5&width=768&dpr=4&quality=100&sign=8573985c&sv=2)

事件

说明

`OnTextShowed`

在整个文本显示完后调用的事件（_如果你将“使用打字机”设置为 true，它将等到所有字母都显示完毕_)

`OnTextDisappeared`

一旦脚本开始隐藏最后一个字母即调用

下面的功能只有在“**使用打字机**” 设置为 **true**:

事件

说明

`OnTypewriterStart`

在打字机开始显示第一个字母之前立即调用。 如果打字机关闭则不会起作用，因为那种情况下它会与 “OnTextShowed” 事件同时发生 _（在这种情况下你可以改用那个事件）_

`OnCharacterVisible(Char)`

每当一个字符变为可见时调用

`OnMessage(EventMarker)`

每当打字机在文本中遇到消息/事件时调用。阅读有关事件的更多内容 [这里](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/zai-da-zi-shi-chu-fa-shi-jian)

打字机使用其链接的文本动画器 **时间缩放** 来推进时间（你可以在此处阅读更多： [动画器设置](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/dong-hua-qi-she-zhi#time-scale)
），这意味着如果时间设置为“未缩放”，即使你的游戏暂停，打字机也会继续进行。

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 曲线特效 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/chuang-jian-nin-zi-ji-de-te-xiao/qu-xian-te-xiao

**曲线效果让您控制许多变换属性，并允许您选择它们随时间的动画。**

* * *

您可以从“特殊”效果子菜单创建自定义曲线效果。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FmuVE9taoTg1C5htCyrOt%252FScreenshot%25202025-11-16%2520alle%252018.57.13.png%3Falt%3Dmedia%26token%3Df99c5af4-aae5-4af1-aa2e-2381f803c31c&width=768&dpr=4&quality=100&sign=bc0ec466&sv=2)

您拥有相同的 [阶段](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/jie-duan)
设置，此外需要注意“**权重**”曲线（从0到1，如果您希望无缝效果应当循环）。

我们正在努力在后续版本中添加更多属性！

多亏了 Text Animator 3.0 和新的核心库，效果 _数据_ 与 _实现_是分离的，这意味着我们可以在不更改您的数据的情况下改进后端/结构！（或无论如何提供更好的迁移步骤/自动修复）！

最后更新于1个月前

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 样式 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi

样式可快速将文本的部分内容替换为其他内容，例如用于创建效果组合、打字机动作和事件，否则这些会因重复标签而需要大量输入。

如果你使用的是 TMPro，请使用 Text Animator Styles 而不是 TMPro 的样式，因为后者（TMPro 的）无法识别 Text Animator 标签，会导致这些标签被添加到文本中。

* * *

只需打开你选择的样式表脚本对象（你可以在项目文件夹通过 创建 菜单 -> Text Animator -> StyleSheet 创建一个），然后开始添加/编辑标签。

你可以拥有一个全局样式表（ [全局设置](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/quan-ju-she-zhi)
 ）也可以有一个本地样式表。

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FbEgcnrQ9RzsRjy1jCb7m%2Ftextanimator%2520settings%2520stylesheet%2520example.png&width=768&dpr=4&quality=100&sign=f266ed25&sv=2)

从上面的示例来看，每当你在文本中写入样式标签“`<style1>`”时，它将被替换为“`<wave><play=5><rainb><shake>`”——并用“`</style1>`”来关闭时，会被替换为“`</wave></rainb></shake><?ended>`”。

样式标签不区分大小写（写 "<style1>" 和 "<Style1>" 会产生相同的结果）。

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 全局设置 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/quan-ju-she-zhi

你可以使用全局设置来管理许多资源功能。

需要将一个 "TextAnimatorSettings" 类型的 ScriptableObject 放在 Resources 文件夹中。导入插件时它应该会自动为你创建，如果找不到它也应该会自动修复！

主要选项有：

*   用于启用或禁用动画类别的切换项 **全局地** （针对所有文本动画器）
    
*   设置不同的数据库以便自动识别用于
    
*   更改解析符号（例如使用 "\[\]" 方括号而不是 "<>" 用于持久效果）
    
*   设置在组件中未设置选项时将使用的“回退”
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F0AZYkreB0l3zafPMLFNT%252FScreenshot%25202025-11-17%2520alle%252014.14.40.png%3Falt%3Dmedia%26token%3Dc1205e8b-c0dc-420c-91bb-5f16383b7489&width=768&dpr=4&quality=100&sign=f47ad928&sv=2)

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 集成的插件与对话系统 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng/ji-cheng-de-cha-jian-yu-dui-hua-xi-tong

我们正在重写所有集成页面，以确保 Text Animator 3.0 与之前的所有第三方资源保持最新：

*   Unity 的 Dialogue System
    
*   Ink
    
*   Game Creator 2
    
*   Unity 本地化包
    
*   Unity 可视化脚本
    
*   Playmaker
    
*   Naninovel
    

我们也在努力集成更多的包，例如：

*   Adventure Creator
    

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng/ji-cheng-de-cha-jian-yu-dui-hua-xi-tong#easy-integration)

官方支持的第三方

**Yarn Spinner**

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng/ji-cheng-de-cha-jian-yu-dui-hua-xi-tong#easy-integration-1)

简单集成

大多数资源应该可以通过 _简单集成_，这意味着该资源应该能够从 Text Mesh Pro 获取文本更改并从那里启动打字机效果。但官方集成即将推出！

附加到文本的不可见标签

_如果你使用简单集成，TextAnimator 将在文本末尾添加两个不可见标签以便工作。别担心，文本的放置/布局将保持不变，并且它的行为就像这些标签根本未被写入一样。_

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 高级概念 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian

在幕后，Text Animator 正在做大量工作和优化以确保：

*   动画期间没有垃圾回收 _（在设置文本时仍然会有一些垃圾回收，TMPro 和 Text Animator 2.0 也是如此，但我们正在努力解决！）_
    
*   该资源与不同的 Unity 版本、系统和平台兼容
    
*   为你提供尽可能简单的 API _（把痛苦留给我们，但这正是目的！）_
    
*   即使在存在错误设置或空引用的情况下，功能也能尽可能正常工作
    

也就是说，当你开始编写自定义脚本时，Text Animator for Unity 内部有一些关键概念需要了解：

*   [高级概念](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian#core-library)
    
*   [高级概念](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian#stateless-vs-referenced-elements)
    

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian#he-xin-ku)

核心库


------------------------------------------------------------------------------------------------------------------

Text Animator 被分为两个主要命名空间：

*   “`Febucci.TextAnimatorCore`” 是我们的 **核心库，** 这是随包一起提供的运行时 DLL，是使一切正常工作的基础。
    
*   “`Febucci.TextAnimatorUnity`” 是 **Unity 实现，**从可脚本化对象到 MonoBehaviours 等。
    

你将在接下来的页面/指南中找到如何按预期设置脚本，但请注意你继承、修改或重新实现的内容！

我会持续更新核心库以实现新功能或重组结构，而不可能预见人们在 C# 中可能做出的各种变体和用例（尤其是非预期的情况）——所以请遵循指南！无论如何我会尽可能将内容标记为 internal，并尽量保持 Unity 实现跨版本向后兼容（像我过去这些年一直做的那样，必要时也会包含更新指南）——但如果你要做一些未计划的修改，请自担风险！

如果你在项目进行中升级 Unity 版本，请从包管理器中移除该资源并重新下载（它会在幕后下载为该 Unity 版本构建的包！）

[故障排除](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#i-upgraded-unity-version-2022.3-greater-than-unity-6.3-and-there-are-some-errors-with-text-animator)

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian#wu-zhuang-tai-vs-you-yin-yong-de-yuan-su)

无状态 vs 有引用的元素


-----------------------------------------------------------------------------------------------------------------------------------------------------------

大多数 Text Animator 元素（包括效果、动作、播放器和曲线）有两种实现方式。一种独立于 Unity 和 GameObject/ScriptableObject，另一种则保留来自游戏状态/文件和类的引用。

类型

优点

缺点

无状态

*   更佳的优化（将来也为 Burst 做好准备，待定）
    
*   元素之间不存在竞争条件
    

*   有一些代码封装，但通过资源的自定义类进行了缓解！
    
*   无法基于游戏状态修改动画/打字效果
    

有引用的

*   可以访问游戏状态并根据其不同地触发行为
    

*   如果实现不当可能出现竞争条件（例如两个打字器同时访问同一个具有计时器或触发行为的动作）
    
*   不能通过 Burst 优化（但大多数情况下应可忽略，因为内置部分承担了主要开销）
    

我们也在研究一种方式，为你提供 _**直接**_ 元素，意思是：移除我们所有的实现，只让你以你想要的方式挂接（鉴于现有的其他工具，这应只适用于大约 1% 的用户，但在我们看来仍然是一个重要选项）。

*   **优点**: 自己动手。
    
*   **缺点**: 自己动手。
    

如何自定义你的元素由你决定。

*   在性能关键的场景下（例如存在大量字母时）选择无状态类型
    

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 高级概念 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei

在幕后，Text Animator 正在做大量工作和优化以确保：

*   动画期间没有垃圾回收 _（在设置文本时仍然会有一些垃圾回收，TMPro 和 Text Animator 2.0 也是如此，但我们正在努力解决！）_
    
*   该资源与不同的 Unity 版本、系统和平台兼容
    
*   为你提供尽可能简单的 API _（把痛苦留给我们，但这正是目的！）_
    
*   即使在存在错误设置或空引用的情况下，功能也能尽可能正常工作
    

也就是说，当你开始编写自定义脚本时，Text Animator for Unity 内部有一些关键概念需要了解：

*   [高级概念](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian#core-library)
    
*   [高级概念](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian#stateless-vs-referenced-elements)
    

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei#he-xin-ku)

核心库


--------------------------------------------------------------------------------------------------

Text Animator 被分为两个主要命名空间：

*   “`Febucci.TextAnimatorCore`” 是我们的 **核心库，** 这是随包一起提供的运行时 DLL，是使一切正常工作的基础。
    
*   “`Febucci.TextAnimatorUnity`” 是 **Unity 实现，**从可脚本化对象到 MonoBehaviours 等。
    

你将在接下来的页面/指南中找到如何按预期设置脚本，但请注意你继承、修改或重新实现的内容！

我会持续更新核心库以实现新功能或重组结构，而不可能预见人们在 C# 中可能做出的各种变体和用例（尤其是非预期的情况）——所以请遵循指南！无论如何我会尽可能将内容标记为 internal，并尽量保持 Unity 实现跨版本向后兼容（像我过去这些年一直做的那样，必要时也会包含更新指南）——但如果你要做一些未计划的修改，请自担风险！

如果你在项目进行中升级 Unity 版本，请从包管理器中移除该资源并重新下载（它会在幕后下载为该 Unity 版本构建的包！）

[故障排除](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#i-upgraded-unity-version-2022.3-greater-than-unity-6.3-and-there-are-some-errors-with-text-animator)

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei#wu-zhuang-tai-vs-you-yin-yong-de-yuan-su)

无状态 vs 有引用的元素


-------------------------------------------------------------------------------------------------------------------------------------------

大多数 Text Animator 元素（包括效果、动作、播放器和曲线）有两种实现方式。一种独立于 Unity 和 GameObject/ScriptableObject，另一种则保留来自游戏状态/文件和类的引用。

类型

优点

缺点

无状态

*   更佳的优化（将来也为 Burst 做好准备，待定）
    
*   元素之间不存在竞争条件
    

*   有一些代码封装，但通过资源的自定义类进行了缓解！
    
*   无法基于游戏状态修改动画/打字效果
    

有引用的

*   可以访问游戏状态并根据其不同地触发行为
    

*   如果实现不当可能出现竞争条件（例如两个打字器同时访问同一个具有计时器或触发行为的动作）
    
*   不能通过 Burst 优化（但大多数情况下应可忽略，因为内置部分承担了主要开销）
    

我们也在研究一种方式，为你提供 _**直接**_ 元素，意思是：移除我们所有的实现，只让你以你想要的方式挂接（鉴于现有的其他工具，这应只适用于大约 1% 的用户，但在我们看来仍然是一个重要选项）。

*   **优点**: 自己动手。
    
*   **缺点**: 自己动手。
    

如何自定义你的元素由你决定。

*   在性能关键的场景下（例如存在大量字母时）选择无状态类型
    

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 Yarn Spinner | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng/ji-cheng-de-cha-jian-yu-dui-hua-xi-tong/yarn-spinner

Yarn Spinner 是一个强大的工具，可让你像这样编写对话：

复制

    -> 发生什么事了？ <<once>>
        守卫：王国正遭受围攻！
    -> 我可以把马停在哪里？ <<once if $has_horse>>
        守卫：在酒馆那边。
    -> 今天天气真好！
        守卫：嗯哼。
    -> 我该走了。
        守卫：请走吧。

它还有一个可视化调试器，可直接在 Unity 中使用，并包含许多其他优秀功能。

[Yarn Spinnerwww.yarnspinner.dev](https://www.yarnspinner.dev/)

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng/ji-cheng-de-cha-jian-yu-dui-hua-xi-tong/yarn-spinner#ji-cheng-bu-zhou)

集成步骤

要集成 Yarn Spinner 3，你需要将 Text Animator 的打字机组件替换为他们为此集成专门制作的组件。

你将能够使用所有 Text Animator 的功能（包括打字机等待时间等），同时 _**也**_ 拥有任意的等待以及 Yarn Spinner 的其他特定功能。

你可以在这里阅读更多：

[![Logo](https://docs.yarnspinner.dev/~gitbook/image?url=https%3A%2F%2F133540031-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fcollections%252FKwtKEQTliyPminHTczxw%252Ficon%252Fv3hX5YL7Z6ThxSO48Uvd%252FYarnSpinner-GitBook-Icon.png%3Falt%3Dmedia%26token%3D4567e3aa-6559-4522-a9d2-627155c77d22&width=48&height=48&sign=f0865bf9&sv=2)Text Animator | Yarn Spinnerdocs.yarnspinner.dev](https://docs.yarnspinner.dev/3.1/yarn-spinner-for-unity/unity-add-ons/text-animator)

如果它的工作方式符合你的预期，或你希望添加任何新功能或改进，请告诉我们！！

* * *

如果你使用的是较旧版本的 Yarn Spinner，请参考他们的文档了解集成如何工作！（Yarn Spinner 同时支持 TAnim 2.X 和 3.X，太棒了！）

最后更新于1个月前

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 访问参数 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-te-xiao-c/fang-wen-can-shu

通过代码访问标签内的值和参数非常有用。这可以通过使用 `RegionParameters` 结构在 `UpdateParameters` 函数中轻松实现，该函数提供对文本每个区域的访问。

复制

    public void UpdateParameters(RegionParameters parameters)
    {
        // ...
        value = parameters.ModifiyFloat("a", fallbackValue);
    }

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-te-xiao-c/fang-wen-can-shu#guan-jian-zi)

关键字

如在 [修饰符](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu#keywords)
所见，关键字是在标签内的普通词（不带等号）（例如， `<mytag keyword1 keyword2 ...>`）。要访问这些关键字的列表，可以使用：

复制

    var keywords = parameters.keywords

*   效果的名称本身就是一个关键字（例如，如果我有 `<mytag key1>`，哈希集合将包含 `[mytag, key1]`);
    
*   修饰符在此列表中被忽略（例如，如果我有 `<mytag myMod=10.0>`，哈希集合将包含 `[mytag]`);
    
*   重复的关键字会被忽略（因为我们使用的是 HashSet）。
    

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-te-xiao-c/fang-wen-can-shu#fu-dian-zhi)

浮点值

要访问浮点值可以使用：

复制

    // 返回标签是否包含该修饰符（真/假）
    parameters.HasFloat("modName");
    
    // 如果存在则返回修饰符值，否则返回备用值
    parameters.ModifiyFloat("modName", fallbackValue); 

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-te-xiao-c/fang-wen-can-shu#zi-fu-chuan-zhi)

字符串值

你也可以访问字符串修饰符：

复制

    // 返回标签是否包含该修饰符（真/假）
    parameters.HasString("modName"); 
    
    // 如果存在则返回修饰符值，否则返回备用值
    parameters.GetStringValueOrDefault("modName", fallbackValue); 

最后更新于4天前

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 编写自定义特效（C#） | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-te-xiao-c

除了使用 [内置效果](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/nei-zhi-te-xiao-lie-biao)
 或 [从检查器创建自定义效果](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/chuang-jian-nin-zi-ji-de-te-xiao)
, **你也可以通过 C# 轻松编写自定义效果**.

附注：确保你已阅读 [高级概念](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian)
 页面！

效果有三个关键部分（可以写在同一个文件中）。

**参数类/结构体**

包含有关你将在效果中使用的数据/值的信息（**状态）**

**状态** 结构体

主效果类。根据参数和字符，随时间修改它。同时处理 [修饰符](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-bian-ji-te-xiao/xiu-shi-fu)

**可脚本化封装**

将前面的元素统一在一起并让你将内容保存到磁盘。只需几行代码让我们完成其余工作！

这些名称只是约定，但 **你可以按自己喜欢的方式命名它们**!

只要知道你需要：

*   用于存储效果变量的东西
    
*   负责修改字母的结构体
    
*   将这两者黏合并允许你将信息保存到磁盘的可脚本化对象
    

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-te-xiao-c#bian-xie-ni-de-zi-ding-yi-jiao-ben)

编写你的自定义脚本


---------------------------------------------------------------------------------------------------------------------------------------------------------------

在本示例中，我们制作了一个使字符按可变量上升的效果。

首先，确保导入必要的命名空间（你的 IDE 无论如何都会提示你 <3）

复制

    using UnityEngine;
    
    // 导入 Text Animator 的命名空间
    using Febucci.TextAnimatorCore;
    using Febucci.TextAnimatorCore.Text;
    using Febucci.Parsing;
    using Febucci.TextAnimatorForUnity.Effects;

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-te-xiao-c#can-shu)

参数

创建你将用来修改字符的数据（这就是你将在检查器中看到并编辑的内容）。

复制

    // 可以是 struct 或 class
    // 后者允许你拥有默认值
    [System.Serializable]
    class CustomEffectParameters
    {
        public float amount = 1.5f;
    }

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-te-xiao-c#zhuang-tai)

状态

效果的“核心”部分。根据参数和预先计算的 Text Animator 数据修改字母。

*   该结构体必须继承自 **IEffectState**.
    

复制

    // 必须是 struct！
    struct CustomEffectState : IEffectState
    {
        readonly float defaultAmount;
        float amount;
    
    
        public CustomEffectState(CustomEffectParameters data)
        {
            // 从参数类获取默认量
            this.defaultAmount = data.amount;
            this.amount = defaultAmount;
        }
    
        public void UpdateParameters(RegionParameters parameters)
        {
            // 自动处理用户在富文本标签中写入的情况， 
            // 在此例中为 "a"
            //（例如 <tagID a=5> 会将 "amount" 设为 5，而 
            // a*2 会使 "amount" 成为 defaultAmount 的两倍）
            amount = parameters.ModifyFloat("a", defaultAmount);
        }
    
        public void Apply(ref CharacterData character, in ManagedEffectContext context)
        {
            // 使用 "amount" 将字符向上移动
            // 使用清晰且易用的 API
            character.MovePosition(
                Vector3.Up * amount * context.progressionRange * context.intensity,
                context.isUpPositive
                );
            // 1. 注意 context.progressionRange -> 它是 
            //     你在编辑器中分配的曲线！
            //     允许你得到阶跃、正弦、弹跳等效果
            // 2. 还要注意 context.intensity，需用于实现 
            //     阶段之间的平滑过渡。
            }
    }

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-te-xiao-c#ke-jiao-ben-hua-dui-xiang-feng-zhuang)

可脚本化对象封装

创建将你的自定义效果挂接到 Text Animator 所需的逻辑，并将其保存在 Assets 文件夹中。

复制

    [System.Serializable] // <-- 使其可序列化！！
    [CreateAssetMenu(fileName = "Your Custom Effect")]
    class CustomEffectScriptable : ManagedEffectScriptable<CustomEffectState, CustomEffectParameters>
    {
        // 简单地根据参数（已由 text animator 管理）创建一个新状态
        protected override CustomEffectState CreateState(CustomEffectParameters parameters)
            => new CustomEffectState(parameters);
    }

还有另一个接受更多类型的 "ManagedEffectScriptable" 版本，以及 "Referenced" 效果的实现，但我们将在未来的版本中介绍！

这些脚本是 Text Animator 确保你获得以下内容所需的全部：

*   自动管理的曲线、播放、修饰符
    
*   无竞态条件的优化效果
    
*   兼容 AOT 平台的效果（无需使用反射）
    
*   我们强大的预览编辑器
    
*   在 UI Toolkit 和 Text Mesh Pro 上表现一致的效果，包括动态缩放
    

还有更多！<3

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F1326131491-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FpaXEW0rl1anhoSBUK719%252FClipboard-20251116-235502-613.gif%3Falt%3Dmedia%26token%3D72885c85-f75f-43db-969c-ab4a72c28803&width=768&dpr=4&quality=100&sign=b078f4b6&sv=2)

* * *

完成！ **你已完成所有必要步骤，耶！** 你添加的效果越多，这个过程就越熟悉、越简单。

记得为你的效果在检查器中设置标签并将其添加到数据库！否则它将无法被识别。你可以在这里阅读更多： [特效数据库](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/ru-he-tian-jia-te-xiao/te-xiao-shu-ju-ku)

**祝你在应用效果时玩得开心！**

* * *

关于创建 “Referenced” 效果的指南即将推出，因为我们仍在调整 UX/API 部分。

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 编写自定义打字等待（C#） | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-da-zi-deng-dai-c

通过使用“Text Animator for Unity”，你可以创建你自己的 **自定义打字机等待**，在字母之间设置不同类型的延迟以及更多功能。

如果你想了解默认的打字机， [请在此阅读](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu)

请务必已阅读 [高级概念](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian)
 页面。

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-da-zi-deng-dai-c#adding-custom-typewriters)

添加自定义打字机等待


--------------------------------------------------------------------------------------------------------------------------------------------------------------

为了创建自定义打字机等待，你需要创建一个继承自的可脚本化对象类 `Febucci.TextAnimatorForUnity.TypingsTimingsScriptableBase`

下面是一个简单的示例脚本：

复制

    // 导入必要的 Febucci 命名空间
    using Febucci.TextAnimatorCore;
    using Febucci.TextAnimatorCore.Text;
    using Febucci.TextAnimatorForUnity;
    
    using UnityEngine;
    
    [System.Serializable] // <--- 记得序列化你的 Scriptable！
    [CreateAssetMenu(fileName = "Custom Typewriter Waits")]
    class CustomTypingWaits : TypingsTimingsScriptableBase
    {
        // --- 像平常一样在此处放置你的属性
        [SerializeField] float delay = .1f;
        
        // 显示文本时的自定义等待
        public override float GetWaitAppearanceTimeOf(CharacterData character, TextAnimator animator)
        {
            // 示例：跳过空格
            if (char.IsWhiteSpace(character.info.character))
                return 0;
    
            return delay;
        }
    
        // 文本消失时的自定义等待
        public override float GetWaitDisappearanceTimeOf(CharacterData character, TextAnimator animator)
        {
            // 在这种情况下，它与显示时相同
            return GetWaitAppearanceTimeOf(character, animator);
        }
    }

* * *

就是这样！

别忘了在你的资源文件夹中创建该可脚本化对象，并将其分配给你的 Typewriter 组件。更多内容请阅读： [动态显示与隐藏字母](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu)

祝你在实现你自己的打字机时玩得开心 <3

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 集成的插件与对话系统 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng

我们正在重写所有集成页面，以确保 Text Animator 3.0 与之前的所有第三方资源保持最新：

*   Unity 的 Dialogue System
    
*   Ink
    
*   Game Creator 2
    
*   Unity 本地化包
    
*   Unity 可视化脚本
    
*   Playmaker
    
*   Naninovel
    

我们也在努力集成更多的包，例如：

*   Adventure Creator
    

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng#easy-integration)

官方支持的第三方

**Yarn Spinner**

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng#easy-integration-1)

简单集成

大多数资源应该可以通过 _简单集成_，这意味着该资源应该能够从 Text Mesh Pro 获取文本更改并从那里启动打字机效果。但官方集成即将推出！

附加到文本的不可见标签

_如果你使用简单集成，TextAnimator 将在文本末尾添加两个不可见标签以便工作。别担心，文本的放置/布局将保持不变，并且它的行为就像这些标签根本未被写入一样。_

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 更新日志 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi

**附言：请务必始终备份你的项目（**_**更好的是：使用版本控制**_**）在更新任何内容之前，即使是在 Text Animator 之外。干杯！**

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#zui-xin-fa-bu)

最新发布


-----------------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#id-3.2.0-zi-ding-yi-xuan-zhuan-shu-zhou-he-cuo-wu-xiu-fu-2025.12.18)

3.2.0 - 自定义旋转枢轴和错误修复 \[2025.12.18\]

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#xin-zeng)

新增

*   添加了具有自定义枢轴旋转的效果
    
*   重新实现了钟摆效果，适用于出现、持续和消失
    
*   \[API\] 在 CharacterData 中公开了字符的已过时间
    

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#cuo-wu-xiu-fu)

错误修复

*   修复了一个恼人的错误，该错误会在某些用户的脚本重载后显示 Text Animator 窗口
    
*   修复了当 Unity 编辑器出现延迟峰值时打字机跳过字符的问题
    
*   修复了当 Unity 包管理器无法找到该包时的错误
    

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#id-3.1.1-yarn-spinner-da-zi-ji-shi-jian-he-xin-wen-mian-ban-2025.12.03)

3.1.1 - Yarn Spinner、打字机事件和新闻面板 \[2025.12.03\]

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#xin-zeng-1)

新增

*   Yarn Spinner 现在已正式集成！(从 3.1 版开始)
    
*   在检查器（Typewriter 组件）中添加了事件，用于在打字机开始等待字符和完成等待字符时触发
    
*   在关于窗口中直接添加了新闻面板，以便在不离开编辑器的情况下跟踪新更新
    

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#cuo-wu-xiu-fu-1)

错误修复

*   修复了内置动作数据库中 "waitforinput" 操作未正确序列化的问题
    
*   修复了当父对象被禁用时打字机未正确启动的问题
    

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#xiao-gai-dong)

小改动

*   在 package.json 中添加了文档和许可证链接
    
*   添加了虚拟方法以在打字机等待字符之前或之后执行操作
    
*   为 Text Animator 和 Typewriter 组件添加了自定义图标
    

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#id-3.0.0-zhi-chi-ui-toolkit-quan-xin-dong-hua-yin-qing-ji-geng-duo-gong-neng-2025.11.18)

3.0.0 - 支持 UI Toolkit、全新动画引擎及更多功能！\[2025.11.18\]

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#xin-zeng-2)

新增

*   从 Unity 6.3 开始支持 UI Toolkit
    
*   你可以只播放一次效果，让它循环 x 次、延迟开始以及通过“播放方式”（无论在编辑器中还是通过富文本标签）实现的许多其他组合
    
*   同一效果现在可以在所有场合下作为出现、持续（之前称为“行为”）和消失播放，进一步增加了可用效果的数量（例如，将“波动”作为出现时的参数与作为持续/行为时的参数不同）。
    
*   你可以使用富文本标签修饰符直接设置效果参数、对其进行乘法运算或设置特定关键字
    
*   你现在可以为效果设置不同的曲线，改变不同过渡及其随时间的移动/影响（例如使旋转看起来滞后，逐步增加）。
    
*   为已有效果添加了更多选项，例如“扩展”和“滑动”的方向
    
*   打字机动作现在可以作为“组件”（而不是仅作为可脚本化对象）创建，使你更容易引用场景对象
    
*   打字机动作现在同时支持协程和无状态的“tick”进程
    
*   新增一个名为“PlaySound”的打字机动作：播放并等待音频源完成后再继续打字机进程
    
*   _添加了许多其他小的改进、工具提示等更多内容。_
    

_我们可能会发现这里漏写了一些功能，并将在接下来的几周更新此页面——已经有_ _**数百**_ _次提交在过去几个月的开发中！_.

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#gai-jin)

**改进**

*   重写了整个文档，希望使其更易于理解，并涵盖更多最佳实践、提示和常见问题
    
*   改进了资产的许可证，现在对独立开发者和更大团队都更易访问。
    
*   优化了效果运行时的零垃圾回收，以及许多其他优化考量
    
*   修复了效果之间的竞态条件（在某些极端情况下发生）
    
*   修复了动作之间的竞态条件，同时允许你为特定打字机指定本地动作
    
*   改进了编辑器的用户体验以及 API。
    
*   改进了欢迎屏幕和设置窗口，现在执行一些额外检查
    
*   你现在可以在多个打字机和文本动画器之间共享设置。
    
*   改进了处理富文本标签参数的 API，现在由 Text Animator 自动处理
    
*   _许多错误修复（例如新输入系统警告）等更多内容_.
    

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#po-huai-xing-api-geng-gai)

破坏性 API 更改

*   大部分 API 都有破坏性更改（因为我们更改了命名空间和一些核心架构，尤其是如果你编写了自定义 C# 效果或动作）。为了实现这个新版本并为我们未来的所有计划做准备，需要进行大量更改——因此我们一次性完成了所有更改（包括许可证变更），这样 a) 你只需考虑一次，b) 我们可以更轻松地进行新更新而不会受阻。请务必阅读 [从 2.X 升级到 3.X](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi/cong-2.x-sheng-ji-dao-3.x)
    。谢谢！
    

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi#yi-zhi-wen-ti)

已知问题


-----------------------------------------------------------------------------------------------------

**我们正在着手修复，并且无论如何会尽快更新该资源。**!

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 从 2.X 升级到 3.X | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi/cong-2.x-sheng-ji-dao-3.x

嗨！这篇文章包含了一些关于从 Text Animator 2.X 升级到 3.0 的有用信息。如果你有任何其他问题，请随时 [通过支持联系我们](https://www.textanimatorforgames.com/support)
!

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi/cong-2.x-sheng-ji-dao-3.x#ru-he-huo-qu-3.x)

如何获取 3.X


--------------------------------------------------------------------------------------------------------------------------------------

为了获得 3.X 版本：

*   **如果你在过去 12 个月内购买了 Text Animator 2.X** （2024 年 11 月及以后），那么你可以免费领取 3.X！只需使用拥有 2.X 的相同账号前往新的资源商店页面，你会看到“免费”选项已解锁。请务必 **现在就领取**，即使你计划稍后再使用 3.X。
    
*   否则， **如果你在 2024 年 11 月之前购买了 Text Animator**，那么你可以以非常优惠的价格升级（过去 5 年我们都免费更新该资源！并且为了让所有人都能负担得起，同时考虑到 3.X 版本的大量工作，我们确实需要你在较大版本发布时给予支持）。
    

👉 **请注意** Text Animator 3.X 使用了不同的许可！它对独立开发者和大团队都更为实惠，你可以 [在此处阅读更多](https://www.textanimatorforgames.com/unity#pricing)
.

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi/cong-2.x-sheng-ji-dao-3.x#jin-zai-xin-xiang-mu-zhong-geng-xin)

仅在新项目中更新


---------------------------------------------------------------------------------------------------------------------------------------------------------

**我们强烈建议你仅在新项目中开始使用 3.X**，鉴于所有这些 [巨大更改](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi)
 和新改进。Text Animator 2.X 现在处于长期支持（LTS）状态，这样你就可以在我们的额外错误修复和支持下继续开发你的游戏。为了实现这个新版本并为我们未来的计划做准备，需要做很多更改 —— 因此我们一次性完成了所有改动（包括许可证更改），这样 a) 你只需一次性考虑此事，b) 我们可以更容易地进行新更新而不会被卡住

**如果你仍然希望在正在运行的项目中将 2.X 更新到 3.0**，难度将取决于你对 2.X 的自定义程度：

*   **如果你只是挂接了 Text Animator 组件**，而没有修改其他脚本，你可能会遇到一些关于命名空间（或过时字段）的错误，但之后你应该可以继续并在检查器中重新创建效果等。
    
*   **如果你编写了大量自定义效果、动作等，** 那么你可能需要花额外时间来迁移所有内容（即使 API 有一些相似之处，新 [核心概念](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/he-xin-gai-nian)
     和 [高级概念](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian)
     在很大程度上改变了公式）。请前往 [编写自定义类](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian)
     以了解更多信息。
    

无论如何，你都需要重新编辑效果数值、数据库并重新挂接组件。我们计划在未来提供自动更新器（这些事情是 _大量_ 的工作）——但请以 3.0 尚未提供此功能为前提进行使用！

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi/cong-2.x-sheng-ji-dao-3.x#zhu-yao-cha-yi)

主要差异


--------------------------------------------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi/cong-2.x-sheng-ji-dao-3.x#he-xin-gai-nian)

核心概念

除了 [更新日志](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi)
之外，如果你过去使用过 2.X，这里有一些核心概念的变化可以帮助你快速上手新版本。

*   现在只有 **一个** 打字机组件（不再是每字符和每单词两个）。 其时序值（每字符和每单词）现在是可脚本化对象，你可以在任何时候引用并切换。更多信息请阅读这里 [动态显示与隐藏字母](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu)
    
*   主要设置现已移至 [全局设置](https://docs.febucci.com/text-animator-unity/3.x-zh/zi-ding-yi/quan-ju-she-zhi)
    
*   某些效果可能有不同的标签，例如 “slide” 变为 “slideh” 和 “slidev”。只需点击效果数据库并根据需要更改标签即可！
    

请务必阅读 [核心概念](https://docs.febucci.com/text-animator-unity/3.x-zh/kuai-su-kai-shi/he-xin-gai-nian)
以及整体文档，以发现新功能和操作方法！

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi/cong-2.x-sheng-ji-dao-3.x#api)

API

对于对 Text Animator 元素的简单引用：

*   命名空间 `Febucci.UI` 现在变为 `Febucci.TextAnimatorForUnity`
    
*   `TypewriterCore` 已被替换为 `TypewriterComponent`
    

对于更高级的更改：

*   请查看 [高级概念](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian)
    
*   查看每个 [编写自定义类](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian)
     页面以了解如何重新实现自定义类。
    

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi/cong-2.x-sheng-ji-dao-3.x#yi-hou-xu-yao-chong-xin-shi-xian-de-que-shi-yuan-su)

以后需要重新实现的缺失元素

*   允许你完全跳过打字机效果的 “notype” 标签。 **变通方法** （如果你在文本中使用过它）**:** 设置一个 tagID 为 “notype” 的样式并将速度设置为非常高。我们将在接下来的几周内更新它！
    

我们也在努力更新 [集成](https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng/ji-cheng-de-cha-jian-yu-dui-hua-xi-tong)
 （即使大多数应该可以立即工作）。在此处阅读更多 [集成的插件与对话系统](https://docs.febucci.com/text-animator-unity/3.x-zh/ji-cheng/ji-cheng-de-cha-jian-yu-dui-hua-xi-tong)
.

最后更新于23天前

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 编写自定义动作（C#） | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-dong-zuo-c

除了使用 [内置动作](https://docs.febucci.com/text-animator-unity/typewriter/wait-actions-when-typing)
之外，你可以通过脚本（C#）编写自己的动作。

务必阅读 [高级概念](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/gao-ji-gai-nian)
 页面。

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-dong-zuo-c#actions-base-class)

创建自定义动作的不同方式


---------------------------------------------------------------------------------------------------------------------------------------------------

自 Text Animator 3.0 起，你可以通过多种方式创建动作，根据项目需求提供更多灵活性。

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-dong-zuo-c#jiang-dong-zuo-zuo-wei-zu-jian-chuang-jian)

将动作作为组件创建

作为组件创建的动作允许你更容易地引用场景对象

复制

    [System.Serializable]
    class ExampleActionComponent : TypewriterActionScriptable
    {
        [SerializeField] float timeToWait;
        
        // 主逻辑在这里， 
        
        // ...可以是无状态的
        protected override IActionState CreateCustomState(ActionMarker marker, object typewriter)
            => new ExampleState(timeToWait);
            
        // ...或作为协程
        protected override IEnumerator PerformAction(TypingInfo typingInfo)
        {
            // yield return ...
        }
    }

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-dong-zuo-c#jiang-dong-zuo-zuo-wei-scriptableobject-chuang-jian)

将动作作为 ScriptableObject 创建

作为 ScriptableObject 的动作可以重复使用并在无需加载场景的情况下被引用

复制

    [System.Serializable]
    [CreateAssetMenu(menuName = "Create Example Action")]
    class ExampleActionScriptable : TypewriterActionScriptable
    {
        [SerializeField] float timeToWait;
        
        // 主逻辑在这里...
        
        // ...可以是无状态的
        protected override IActionState CreateCustomState(ActionMarker marker, object typewriter)
            => new ExampleState(timeToWait);
            
        // ...或作为协程
        protected override IEnumerator PerformAction(TypingInfo typingInfo)
        {
            // yield return ...
        }
    }

附言：别忘了在项目视图中创建你的动作 ScriptableObject，并将其添加到动作数据库中。

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-dong-zuo-c#actions-base-class-1)

实现动作逻辑的不同方式


----------------------------------------------------------------------------------------------------------------------------------------------------

你可以决定如何编写动作的核心逻辑。

*   在协程（IEnumerator）内部，或
    
*   通过单独的“tick”方法（该方法返回动作是否应继续运行或已完成）。
    

首先，导入正确的命名空间：

复制

    using Febucci.TextAnimatorForUnity.Actions;
    using Febucci.TextAnimatorCore.Typing;
    using UnityEngine;

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-dong-zuo-c#actions-base-class-2)

创建协程

编写协程非常简单！

例如，在你的 TypewriterAction 类（无论是组件还是 Scriptable）中，只需重写 PerformAction 方法：

复制

    [SerializeField] AudioSource source;
    
    protected override IEnumerator PerformAction(TypingInfo typingInfo)
    {
        if (source != null && source.clip != null)
        {
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
        }
    }

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-dong-zuo-c#actions-base-class-3)

创建无状态动作

另一方面，创建无状态动作需要你创建一个继承于自定义结构体， **IActionState** 并且该结构体将执行动作（在此示例中：在继续打字器之前等待几秒），例如：

复制

    struct ExampleState : IActionState // <--- 必须继承自此
    {
        float timePassed;
        readonly float timeToWait;
        public ExampleState(float timeToWait)
        {
            timePassed = 0;
            this.timeToWait = timeToWait;
        }
        
        public ActionStatus Progress(float deltaTime, ref TypingInfo typingInfo)
        {
            // 增加已过时间
            timePassed += deltaTime;
            
            // 根据时间决定继续还是停止
            return timePassed >= timeToWait
                ? ActionStatus.Finished
                : ActionStatus.Running;
        }
        
        public void Cancel()
        {
            // 在此用于修改 
        }
    }

然后你可以通过在你的动作类中重写 CreateCustomState 方法来实例化此结构体（我们在这里看到的那个 [创建自定义动作的不同方式](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-dong-zuo-c#actions-base-class)
).

复制

    protected override IActionState CreateCustomState(ActionMarker marker, object typewriter)
            => new ExampleState(timeToWait);

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/bian-xie-zi-ding-yi-lei/bian-xie-zi-ding-yi-dong-zuo-c#attributes)

属性

*   该 `标记` 参数包含关于你的标签的有用信息，例如 ID 或是否有随之而来的任何参数（例如 `<playSound=02>`).
    
*   该 `typewriter` 引用当前正在执行该动作的 Typewriter 组件或 AnimatedLabel
    
*   该 `typingInfo` 包含诸如当前打字速度（你可以修改）和打字器内已过时间等信息。
    

* * *

完成！通过这个简单的步骤，你可以添加任何你想要的自定义动作。

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 Unity용 텍스트 애니메이터 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko

**환영합니다** 문서에 오신 것을 **Text Animator for Unity 3.X**! 텍스트에 애니메이션을 적용하고 플러그인에 익숙해지실 것을 기대합니다.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F74b3Q86Y180EtGnu7Jg5%252FGames%2520Using%2520Text%2520Animator.png%3Falt%3Dmedia%26token%3D9912a72f-fab2-4003-b8f7-3499fc676a33&width=768&dpr=4&quality=100&sign=e99dca13&sv=2)

우리는 가능한 한 짧고 간결하면서도 필요할 때 도움을 줄 수 있는 문서를 작성하고 있습니다. **가장 먼저 그리고 가장 중요한 페이지를 반드시 읽어보세요**! 지금 몇 분이면 되지만 이후에 _많은 시간_ 을 절약해줄 것입니다.

유용한 링크:

[구매](https://www.textanimatorforgames.com/unity#pricing)
 [웹사이트](https://www.textanimatorforgames.com/unity)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko#undefined)

알아두면 좋은 것들

*   이 문서는 여러 언어로 제공됩니다: 영어, 중국어, 한국어, 일본어.
    
*   다음을 통해 이 문서의 다양한 버전과 언어를 찾아볼 수 있습니다 이 페이지 상단에서.
    
*   Text Animator는 다른 엔진에서도 제공됩니다. [여기에서 자세히 알아보기](https://www.textanimatorforgames.com/)
    .
    

언제든 도움이 필요하면 [문제 해결 페이지를 방문하세요](https://docs.febucci.com/text-animator-unity/3.x-ko/other/troubleshooting)
 (일반적인 문제와 해결 방법) 또는 지원 페이지에 방문해 주세요!

[![Logo](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fframerusercontent.com%2Fimages%2Fy1LCEnd5hyGjuX0kKaGBUorzMc.png&width=20&dpr=4&quality=100&sign=82d1be32&sv=2)Support Requests | Text Animator for Unity, Godot and Unrealwww.textanimatorforgames.com](https://www.textanimatorforgames.com/support)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko#undefined-1)

요구 사항

구매 또는 임포트 전에 반드시 [요구사항 및 제한사항](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations)
 페이지를 방문해 주세요. 또한 많은 [자주 묻는 질문을 여기에서 답변해 두었습니다](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq)
 감사합니다!

* * *

**즐겁게 사용하세요** 그리고 우리의 [디스코드](https://discord.com/invite/j4pySDa5rU)
 에 가입하여 대화에 참여하고 당신이 만든 것을 보여주는 것을 잊지 마세요!

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 故障排除 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu

当我设置文本时，在显示新文本之前我会看到上一帧的旧文本[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#dang-wo-she-zhi-wen-ben-shi-zai-xian-shi-xin-wen-ben-zhi-qian-wo-hui-kan-dao-shang-yi-zhen-de-jiu-we)

这可能是因为文本被设置为 TMPro/UITK，而不是直接设置为 Text Animator。

**解决方案**：请查看 [设置文本](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben)
 页面以了解最佳实践和 [动态显示与隐藏字母](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu)
!

**解决方法**：如果问题仍然存在，请确保在显示新文本之前清除文本（例如在禁用时）。

我升级了 Unity 版本（2022.3 -> Unity 6.3），Text Animator 出现了一些错误[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#wo-sheng-ji-le-unity-ban-ben-2022.3-unity-6.3text-animator-chu-xian-le-yi-xie-cuo-wu)

如果在同一项目中从 Unity 2022.3 升级到 Unity 6.3 并且项目中仍保留 Text Animator，可能会出现一些错误。我们实际上为不同的 Unity 版本提供不同版本的包，因此你也需要切换 Text Animator 的版本。

**解决方案**：只需移除该包（不是效果/数据！仅移除脚本），然后从包管理器重新导入即可。

我将资源从 2.X 更新到 3.X 出现了一些错误[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#wo-jiang-zi-yuan-cong-2.x-geng-xin-dao-3.x-chu-xian-le-yi-xie-cuo-wu)

是的！不幸的是这是预料之中的（正如我们在博客文章、公告中所写，并在资源商店中做了重大升级）。

**解决方案** （有点）：由于 3.0 版本带来了一些重要更改，我们确实建议你在此项目中保持使用 2.X 版本，仅在新项目中切换到 Text Animator 3.0。如果你在其上编写了自定义集成/脚本，请查看 [从 2.X 升级到 3.X](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi/cong-2.x-sheng-ji-dao-3.x)
 以获取相关信息！

打字机效果会瞬间显示全部文本[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#da-zi-ji-xiao-guo-hui-shun-jian-xian-shi-quan-bu-wen-ben)

**解决方案**：请确保从检查器/UI Builder 中为时序分配可脚本化对象（scriptable object）！ [动态显示与隐藏字母](https://docs.febucci.com/text-animator-unity/3.x-zh/da-zi-ji/dong-tai-xian-shi-yu-yin-cang-zi-mu)

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#chang-jian-cuo-wu)

常见错误

NullReferenceException：对象引用未设置为对象的实例 TMPro.TMP\_Settings.get\_defaultStyleSheet[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#nullreferenceexception-dui-xiang-yin-yong-wei-she-zhi-wei-dui-xiang-de-shi-li-tmpro.tmpsettings.getd)

请确保已正确导入 TextMeshPro 并初始化“必需项”。在此处阅读更多信息 [设置文本](https://docs.febucci.com/text-animator-unity/3.x-zh/xiao-guo/she-zhi-wen-ben)

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#jing-gao)

警告

Camera Main Camera 不包含额外的相机数据组件。打开该游戏对象的检查器以添加额外的相机数据。[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#camera-main-camera-bu-bao-hanewai-de-xiang-ji-shu-ju-zu-jian-da-kai-gai-you-xi-dui-xiang-de-jian-cha)

如果你安装了 URP 或类似包但示例场景没有，该情况会在示例场景中出现。这不是问题！按照警告说明添加额外数据，资源仍然可以正常工作！

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/gu-zhang-pai-chu#yi-zhi-wen-ti)

已知问题

**我们正在着手修复，并且无论如何会尽快更新该资源。**!

请注意，我们不正式支持 Unity Alpha 和 Beta 版本！我们无法得知 Unity 是否更改了 API 等。 _这一天_ 他们发布新的 alpha 或 beta 的那天，所以我们会使用这些版本进行测试并确保该资源在正式/生产版本中可用。谢谢！

如果你遇到其他任何问题，请随时在此联系我们！我们会尽快修复：

[![Logo](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fframerusercontent.com%2Fimages%2Fy1LCEnd5hyGjuX0kKaGBUorzMc.png&width=20&dpr=4&quality=100&sign=82d1be32&sv=2)Support Requests | Text Animator for Unity, Godot and Unrealwww.textanimatorforgames.com](https://www.textanimatorforgames.com/support)

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 更新日志 | 3.X (ZH) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta

**附言：请务必始终备份你的项目（**_**更好的是：使用版本控制**_**）在更新任何内容之前，即使是在 Text Animator 之外。干杯！**

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#zui-xin-fa-bu)

最新发布


-------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#id-3.2.0-zi-ding-yi-xuan-zhuan-shu-zhou-he-cuo-wu-xiu-fu-2025.12.18)

3.2.0 - 自定义旋转枢轴和错误修复 \[2025.12.18\]

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#xin-zeng)

新增

*   添加了具有自定义枢轴旋转的效果
    
*   重新实现了钟摆效果，适用于出现、持续和消失
    
*   \[API\] 在 CharacterData 中公开了字符的已过时间
    

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#cuo-wu-xiu-fu)

错误修复

*   修复了一个恼人的错误，该错误会在某些用户的脚本重载后显示 Text Animator 窗口
    
*   修复了当 Unity 编辑器出现延迟峰值时打字机跳过字符的问题
    
*   修复了当 Unity 包管理器无法找到该包时的错误
    

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#id-3.1.1-yarn-spinner-da-zi-ji-shi-jian-he-xin-wen-mian-ban-2025.12.03)

3.1.1 - Yarn Spinner、打字机事件和新闻面板 \[2025.12.03\]

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#xin-zeng-1)

新增

*   Yarn Spinner 现在已正式集成！(从 3.1 版开始)
    
*   在检查器（Typewriter 组件）中添加了事件，用于在打字机开始等待字符和完成等待字符时触发
    
*   在关于窗口中直接添加了新闻面板，以便在不离开编辑器的情况下跟踪新更新
    

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#cuo-wu-xiu-fu-1)

错误修复

*   修复了内置动作数据库中 "waitforinput" 操作未正确序列化的问题
    
*   修复了当父对象被禁用时打字机未正确启动的问题
    

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#xiao-gai-dong)

小改动

*   在 package.json 中添加了文档和许可证链接
    
*   添加了虚拟方法以在打字机等待字符之前或之后执行操作
    
*   为 Text Animator 和 Typewriter 组件添加了自定义图标
    

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#id-3.0.0-zhi-chi-ui-toolkit-quan-xin-dong-hua-yin-qing-ji-geng-duo-gong-neng-2025.11.18)

3.0.0 - 支持 UI Toolkit、全新动画引擎及更多功能！\[2025.11.18\]

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#xin-zeng-2)

新增

*   从 Unity 6.3 开始支持 UI Toolkit
    
*   你可以只播放一次效果，让它循环 x 次、延迟开始以及通过“播放方式”（无论在编辑器中还是通过富文本标签）实现的许多其他组合
    
*   同一效果现在可以在所有场合下作为出现、持续（之前称为“行为”）和消失播放，进一步增加了可用效果的数量（例如，将“波动”作为出现时的参数与作为持续/行为时的参数不同）。
    
*   你可以使用富文本标签修饰符直接设置效果参数、对其进行乘法运算或设置特定关键字
    
*   你现在可以为效果设置不同的曲线，改变不同过渡及其随时间的移动/影响（例如使旋转看起来滞后，逐步增加）。
    
*   为已有效果添加了更多选项，例如“扩展”和“滑动”的方向
    
*   打字机动作现在可以作为“组件”（而不是仅作为可脚本化对象）创建，使你更容易引用场景对象
    
*   打字机动作现在同时支持协程和无状态的“tick”进程
    
*   新增一个名为“PlaySound”的打字机动作：播放并等待音频源完成后再继续打字机进程
    
*   _添加了许多其他小的改进、工具提示等更多内容。_
    

_我们可能会发现这里漏写了一些功能，并将在接下来的几周更新此页面——已经有_ _**数百**_ _次提交在过去几个月的开发中！_.

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#gai-jin)

**改进**

*   重写了整个文档，希望使其更易于理解，并涵盖更多最佳实践、提示和常见问题
    
*   改进了资产的许可证，现在对独立开发者和更大团队都更易访问。
    
*   优化了效果运行时的零垃圾回收，以及许多其他优化考量
    
*   修复了效果之间的竞态条件（在某些极端情况下发生）
    
*   修复了动作之间的竞态条件，同时允许你为特定打字机指定本地动作
    
*   改进了编辑器的用户体验以及 API。
    
*   改进了欢迎屏幕和设置窗口，现在执行一些额外检查
    
*   你现在可以在多个打字机和文本动画器之间共享设置。
    
*   改进了处理富文本标签参数的 API，现在由 Text Animator 自动处理
    
*   _许多错误修复（例如新输入系统警告）等更多内容_.
    

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#po-huai-xing-api-geng-gai)

破坏性 API 更改

*   大部分 API 都有破坏性更改（因为我们更改了命名空间和一些核心架构，尤其是如果你编写了自定义 C# 效果或动作）。为了实现这个新版本并为我们未来的所有计划做准备，需要进行大量更改——因此我们一次性完成了所有更改（包括许可证变更），这样 a) 你只需考虑一次，b) 我们可以更轻松地进行新更新而不会受阻。请务必阅读 [从 2.X 升级到 3.X](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta/geng-xin-ri-zhi/cong-2.x-sheng-ji-dao-3.x)
    。谢谢！
    

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-zh/qi-ta#yi-zhi-wen-ti)

已知问题


-------------------------------------------------------------------------------------

**我们正在着手修复，并且无论如何会尽快更新该资源。**!

本站使用 cookie 来提供服务并分析流量。浏览本站，即表示您接受[隐私政策](https://www.febucci.com/privacy_policy/)
。

接受拒绝

---

# 📄 자주 묻는 질문 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#undefined)

현지화

Text Animator는 여러 언어와 함께 작동하나요?[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#text-animator)

간단한 답: **예,** _**하지만 Text Animator에 의해 처리되지는 않습니다**_.

*   번역된 텍스트에 관하여: 예, 하지만 현지화는 Text Animator가 처리하지 않습니다. 현지화는 외부 스크립트(예: 자체 현지화 관리자, 대화 시스템 등)에 의해 처리됩니다. 다시 말해, Text Animator는 현지화 플러그인이 아닙니다. 리치 텍스트 태그를 포함한 텍스트가 있다면 번역된 언어에서도 동일한 레이아웃이어야 합니다(예: “hello <shake> how are you?”는 “ciao <shake> come stai?”처럼 번역되어야 합니다). 그런 다음 단순히 "textAnimatorComponent.ShowText(translatedText);"를 호출하면 됩니다. (이 현지화 과정은 다른 게임/프로젝트에도 동일하게 적용됩니다 \[...\])
    
*   다른 글꼴에 관하여: 예, 하지만 이것도 Text Animator가 처리하는 것이 아니라 Text Mesh Pro가 처리합니다. TMPro가 언어를 지원하면 Text Animator도 동일하게 작동합니다. 이는 Text Animator가 문자만 애니메이션화하고 해당 문자는 TextMeshPro가 생성하기 때문입니다.
    

RTL 텍스트(오른쪽에서 왼쪽으로)가 지원되나요?[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#rtl)

예! 내부적으로 TextAnimator는 메시만 이동시키고, 메시 생성은 TextMeshPro가 담당합니다. TMPro는 RTL 텍스트를 지원합니다(컴포넌트의 인스펙터에서 활성화할 수 있음). 따라서 TextAnimator도 RTL을 지원합니다.

_주의하세요_ "RTLTMPro"와 같은 외부 패키지는 완전히 지원되지 않을 수 있습니다. 우리는 엄격하게 TMPro만을 기준으로 언급하고 있으므로 [통합된 플러그인 및 대화 시스템](https://docs.febucci.com/text-animator-unity/3.x-ko/integrations/integrated-plugins-and-dialogues-systems)
 그 대신.

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#undefined-1)

통합 및 버전

어떤 Unity 버전이 지원되나요?[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#unity)

지원되는 Unity 버전은 여기를 읽으면 확인할 수 있습니다: [요구사항 및 제한사항](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations)

내 \[여기에 대화 시스템 삽입\]이(가) 지원되나요?[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#undefined-2)

어떤 서드파티 플러그인이 이미 Text Animator와 통합되어 있는지는 여기를 읽으면 확인할 수 있습니다: [통합된 플러그인 및 대화 시스템](https://docs.febucci.com/text-animator-unity/3.x-ko/integrations/integrated-plugins-and-dialogues-systems)

UIToolkit이 지원되나요?[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#uitoolkit)

예! Unity 6.3 및 이후 버전에서 지원됩니다.

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#undefined-3)

효과 및 파싱

리치 텍스트 파싱을 위한 기호를 변경할 수 있나요? (예: "<shake>" 대신 "\[shake\]")[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#less-than-shake-greater-than-shake)

예! 해당 설정은 [글로벌 설정](https://docs.febucci.com/text-animator-unity/3.x-ko/customization/global-settings)
 파일에서 가능합니다.

효과는 언제 적용되나요? 태그를 여는 순간인가요, 아니면 닫는 순간인가요?[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#undefined-4)

효과는 태그를 여는 순간부터 적용됩니다.

"<shake>hello" 는 이미 단어 "hello" 가 첫 번째 '\>' 문자를 설정한 순간부터 흔들리게 됩니다.

TextAnimator는 편집 모드에서 효과를 미리보기를 하나요?[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#textanimator)

예! 효과 Scriptable Object를 클릭하면 미리보기를 볼 수 있습니다. [효과 편집 방법](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects)

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#undefined-5)

기타

라이선스에 관해 질문이 있습니다[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#undefined-6)

다음을 읽어보실 수 있습니다 [여기에서 라이선스 정보 확인](https://www.textanimatorforgames.com/unity#faq)
.

웹 빌드에서 Text Animator를 사용할 수 있나요?[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#text-animator-1)

예!

플러그인의 "Example" 폴더를 삭제해도 되나요?[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#example)

물론입니다. 필요하지 않다면 플러그인의 예제 폴더를 삭제할 수 있습니다.

_누가 멋진가요?_[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#undefined-7)

당신이 멋집니다!

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq#undefined-8)

무엇이든 물어보세요

추가 질문이 있으시면 언제든지 문의해 주세요!

[![Logo](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fframerusercontent.com%2Fimages%2Fy1LCEnd5hyGjuX0kKaGBUorzMc.png&width=20&dpr=4&quality=100&sign=82d1be32&sv=2)Support Requests | Text Animator for Unity, Godot and Unrealwww.textanimatorforgames.com](https://www.textanimatorforgames.com/support)

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 요구사항 및 제한사항 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations

텍스트 애니메이터는 요구사항과 제약이 매우 적은 매우 강력한 도구입니다. **구매 전에 여기를 읽어보세요!**

*   [요구사항](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#requirements)
    
*   [제한사항](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#limitations)
    

다음에도 관심이 있을 수 있습니다:

*   [통합](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#integrating-third-party-dialogue-systems-and-plugins)
    
*   [요구사항 및 제한사항](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#frequently-asked-questions)
    
*   [요구사항 및 제한사항](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#known-issues)
    

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#requirements)

요구사항


-------------------------------------------------------------------------------------------------------------------

**이 에셋은 다음 UI 및 유니티 버전과 호환됩니다**:

*   **Text Mesh Pro** (유니티 2022.3 이상)
    
*   **UI 툴킷** (유니티 6.3 이상).
    

새로운 유니티 입력 시스템(및 기존 시스템)도 지원합니다.

Unity 알파 및 베타 버전은 공식적으로 지원하지 않는다는 점을 유의하세요! Unity가 API 등을 변경했는지 우리 쪽에서는 알 수 있는 방법이 없습니다. _그날_ 그들이 새로운 알파 또는 베타를 공개한 날이므로, 우리는 이러한 버전들을 사용해 테스트하고 에셋이 정식/프로덕션 버전에서 작동하는지 확인합니다. 감사합니다!

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#integrating-third-party-dialogue-systems-and-plugins)

통합


---------------------------------------------------------------------------------------------------------------------------------------------------------

타사 대화 시스템 및 플러그인 통합:

다음 몇 주 내에 모든 타사 통합을 이식하고 있습니다! 자세한 내용은 여기를 읽어보세요 [통합된 플러그인 및 대화 시스템](https://docs.febucci.com/text-animator-unity/3.x-ko/integrations/integrated-plugins-and-dialogues-systems)

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#limitations)

제한사항


------------------------------------------------------------------------------------------------------------------

이 에셋이 할 수 없는 것들 _(현재)_ 성취.

"바"는 애니메이션되지 않음(선택 사항)[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#undefined)

텍스트의 “바”(는`취소선` **및** `밑줄`)은 선택에 따라 애니메이션되지 않습니다.

(여기에서 애니메이션된 바가 어떻게 보이는지 확인할 수 있습니다. 보기 좋지 않기 때문에 정적으로 유지하기로 결정했습니다.)

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2Fj1zQb4UQUOp9BRiaMwTh%2Fbarsnotanimated.gif&width=300&dpr=4&quality=100&sign=1b0918ec&sv=2)

패키지 제거 시 태그 제거[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#undefined-1)

알려진 바와 같이(예: TMPro), 이 패키지를 제거하면 대화에서 이 플러그인의 모든 태그를 수동으로 제거해야 합니다.

👍🏻 이것이 걱정된다면 _어떤 이유로든,_ **플러그인을 ‘대체 효과(fallback effects)’만 사용하도록 설정할 수 있습니다** 만 (적용되는) **텍스트 전체에** 태그를 요구하지 않고) 플러그인을 제거하는 경우 모든 것이 건드려지지 않은 채로 남아 있을 것입니다. 야호!

\\r 및 \\b 사용[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#r-b)

텍스트의 _부분을_ 중간에 지우거나 대체할 수 없습니다.

❌ 백스페이스(예: , `\b` )는 현재 지원되지 않습니다

✔️ 전체 **텍스트를 중간에 지우거나/변경하거나/교체할 수 있으며, 특정 부분을 숨길 수도 있습니다.** 자주 묻는 질문

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#undefined-2)

일반적인 문제와 해결 방법을 보려면


---------------------------------------------------------------------------------------------------------------------------------

도 또한 읽어보세요. 감사합니다! [자주 묻는 질문](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq)
알려진 문제

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations#undefined-3)

알려진 문제


--------------------------------------------------------------------------------------------------------------------

**우리는 수정 작업을 진행 중이며 어쨌든 가능한 한 빨리 자산을 업데이트하겠습니다**!

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 핵심 개념 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/core-concepts

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/core-concepts#undefined)

효과

문자의 "생애"의 다양한 단계에서 효과를 적용할 수 있습니다:

**등장**

![An example of the Appearance Effect {vertexp}](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FT7U4C8xOWPU5tjtdhxHT%2Fverticalexpandpreview.gif&width=300&dpr=4&quality=100&sign=2d90d0dc&sv=2)

문자가 화면에 나타날 때만 글자를 애니메이션화하는 용도입니다. _(더보기...__)_

**지속**

![An example of the Behavior Effect <wiggle>](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FkXQFZNbm8mSv67m9nubS%2Fwigglepreviewfebucci.gif&width=300&dpr=4&quality=100&sign=1ff9ee43&sv=2)

문자가 보이는 동안 시간에 걸쳐 글자 효과를 지속적으로 애니메이션화하는 용도입니다.

**사라짐**

![An example of the Disappearance Effect {#size}](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FsHBEkEs6y1POC6EOORwf%2Fdecreasing%2520size%2520text%2520animator%2520unity4.gif&width=300&dpr=4&quality=100&sign=89a11fab&sv=2)

문자가 보이지 않게 되었을 때 글자를 애니메이션화하는 용도입니다.

Text Animator 3.0부터는 어떤 효과든 글자의 어느 단계에서나 재생할 수 있습니다! (등장, 지속 또는 사라짐)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/core-concepts#undefined-1)

값 혼합 및 매치

"기본" 효과와 값이 있더라도 인스펙터나 텍스트를 통해 언제든지 수정할 수 있습니다.

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/core-concepts#undefined-2)

설정 접근성

Text Animator는 애니메이션부터 타이프라이터 등 다양한 설정을 사용합니다.

대부분의 경우, 이러한 설정은 세 가지 수준에서 다르게 적용할 수 있습니다:

*   **로컬:** 설정이 해당 컴포넌트에 묶여 있습니다
    
*   **공유:** 설정이 ScriptableObject에 저장되며, 해당 ScriptableObject 참조를 가진 다른 인스턴스들 간에 공유됩니다.
    
*   **글로벌:** 설정이 다른 설정 위에 적용되거나(예: 효과 인식의 경우) 사용되거나 _오직_ 다른 설정이 지정되지 않은 경우에만(곡선의 "대체(fallback)"와 같은 경우)
    

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/core-concepts#undefined-3)

데이터베이스

Text Animator는 무엇이 존재하는지에 대한 정보와 _사용될 수 있는 것들_ 및 애니메이션과 타이프라이터의 빌딩 블록(효과, 대기 시간, 곡선 등)을 저장하기 위해 ScriptableObject를 사용합니다.

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/core-concepts#undefined-4)

에디터 툴팁

인스펙터의 많은 옵션과 필드 위에 마우스를 올려 놓으면 툴팁과 추가 정보를 표시할 수 있습니다!

Last updated 1 month ago

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 Unity용 텍스트 애니메이터 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/welcome

**환영합니다** 문서에 오신 것을 **Text Animator for Unity 3.X**! 텍스트에 애니메이션을 적용하고 플러그인에 익숙해지실 것을 기대합니다.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F74b3Q86Y180EtGnu7Jg5%252FGames%2520Using%2520Text%2520Animator.png%3Falt%3Dmedia%26token%3D9912a72f-fab2-4003-b8f7-3499fc676a33&width=768&dpr=4&quality=100&sign=e99dca13&sv=2)

우리는 가능한 한 짧고 간결하면서도 필요할 때 도움을 줄 수 있는 문서를 작성하고 있습니다. **가장 먼저 그리고 가장 중요한 페이지를 반드시 읽어보세요**! 지금 몇 분이면 되지만 이후에 _많은 시간_ 을 절약해줄 것입니다.

유용한 링크:

[구매](https://www.textanimatorforgames.com/unity#pricing)
 [웹사이트](https://www.textanimatorforgames.com/unity)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome#undefined)

알아두면 좋은 것들

*   이 문서는 여러 언어로 제공됩니다: 영어, 중국어, 한국어, 일본어.
    
*   다음을 통해 이 문서의 다양한 버전과 언어를 찾아볼 수 있습니다 이 페이지 상단에서.
    
*   Text Animator는 다른 엔진에서도 제공됩니다. [여기에서 자세히 알아보기](https://www.textanimatorforgames.com/)
    .
    

언제든 도움이 필요하면 [문제 해결 페이지를 방문하세요](https://docs.febucci.com/text-animator-unity/3.x-ko/other/troubleshooting)
 (일반적인 문제와 해결 방법) 또는 지원 페이지에 방문해 주세요!

[![Logo](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fframerusercontent.com%2Fimages%2Fy1LCEnd5hyGjuX0kKaGBUorzMc.png&width=20&dpr=4&quality=100&sign=82d1be32&sv=2)Support Requests | Text Animator for Unity, Godot and Unrealwww.textanimatorforgames.com](https://www.textanimatorforgames.com/support)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome#undefined-1)

요구 사항

구매 또는 임포트 전에 반드시 [요구사항 및 제한사항](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/requirements-and-limitations)
 페이지를 방문해 주세요. 또한 많은 [자주 묻는 질문을 여기에서 답변해 두었습니다](https://docs.febucci.com/text-animator-unity/3.x-ko/welcome/faq)
 감사합니다!

* * *

**즐겁게 사용하세요** 그리고 우리의 [디스코드](https://discord.com/invite/j4pySDa5rU)
 에 가입하여 대화에 참여하고 당신이 만든 것을 보여주는 것을 잊지 마세요!

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 텍스트 설정 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts

텍스트 애니메이터에 텍스트를 설정하는 방법은 두 가지 UI 시스템에서 가능합니다:

*   [텍스트 설정](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#ui-toolkit)
    
*   [Text Mesh Pro](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#text-mesh-pro)
    

이 페이지에는 이미 [설치 및 빠른 시작](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/install-and-quick-start)
에 있는 일부 정보가 포함되어 있지만, 각 시스템 및 일반적인 사용에 대한 다른 세부사항과 제안들도 포함되어 있습니다. 반드시 [텍스트 설정](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#best-practices)
 섹션을 읽어보세요!

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#ui)

UI 툴킷


----------------------------------------------------------------------------------------------

_추신. 이미 알고 있다고 가정합니다_ [_UI 툴킷을 사용하는 방법을_](https://docs.unity3d.com/Documentation/Manual/UIElements.html)
 _그리고 그것이 무엇을 하는지._

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#ui)

UI 빌더에서

*   라이브러리 -> 프로젝트로 이동
    
*   드래그 "AnimatedLabel" 를 계층 구조의 "Custom Controls/Febucci/Text Animator for Unity"에서!
    

내장된 레이블과 버튼을 UI 툴킷에서 직접 애니메이션할 수 있도록 작업 중입니다! _(Unity 6.3 이상.)_ 업데이트를 확인하세요!

당신의 .uxml은 다음과 같이 보여야 합니다:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZNwCUmAugxLNcVVO9oBk%252FScreenshot%25202025-11-15%2520alle%252018.02.51.png%3Falt%3Dmedia%26token%3Dced34791-d558-4883-b646-2197664dd637&width=768&dpr=4&quality=100&sign=944232c1&sv=2)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#undefined)

코드로

"의 인스턴스를 생성할 수 있습니다Febucci.TextAnimatorForUnity.AnimatedLabel" 클래스와 UI 문서에 추가하는 방법은 다음과 같습니다:

Copy

    using UnityEngine;
    using UnityEngine.UIElements;
    using Febucci.TextAnimatorForUnity; // <- Text Animator의 네임스페이스를 임포트
    
    public class ExampleScript : MonoBehaviour
    { 
        [SerializeField] UIDocument document;
    
        void Start()
        {
            var container = document.rootVisualElement.contentContainer;
            var animatedLabel = new AnimatedLabel(); // <- 애니메이션 레이블 생성
            container.Add(animatedLabel); // <- 컨텐츠 컨테이너에 추가
            // [..]
            animatedLabel.SetText("<wave>hello"); // <- 텍스트 설정
        }
    }

이제 끝났습니다!! 당신은 준비가 되었습니다 [효과 추가 방법](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects)

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#text-mesh-pro)

Text Mesh Pro


-----------------------------------------------------------------------------------------------------------------

_추신. 이미 알고 있다고 가정합니다_ [_Text Mesh Pro 사용 방법_](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html)
 _그리고 그것이 어떻게 작동하는지._

추가하기 텍스트 애니메이터 - Text Mesh Pro 동일한 GameObject에 구성 요소를 추가하세요 TextMeshPro 구성 요소(UI 또는 월드 공간 중 하나!):

인스펙터는 다음과 같아야 합니다:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FT3h66pIPFdakGOCfToEY%252FScreenshot%25202025-11-15%2520alle%252017.59.18.png%3Falt%3Dmedia%26token%3D26196c49-f0f5-457b-85dd-da358f43c823&width=768&dpr=4&quality=100&sign=78164711&sv=2)

이제 끝났습니다!! 당신은 준비가 되었습니다 [효과 추가 방법](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects)

컴포넌트에 텍스트를 설정했는데 빈 텍스트가 보인다면, TextMeshPro 컴포넌트를 최소 한 번 클릭해 "Essentials"를 가져왔는지 확인하세요(창이 뜨면 가져오라는 요청이 표시됩니다).

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#undefined)

코드로 텍스트를 설정할 때의 모범 사례

코드로 TextMeshPro 객체에 텍스트를 설정할 때는 TMPro 대신 Text Animator의 스크립트를 참조하세요. 예:

Copy

    using UnityEngine;
    using TMPro; 
    using Febucci.TextAnimatorForUnity.TextMeshPro; // <- Text Animator의 네임스페이스를 임포트
    
    public class ExampleScript : MonoBehaviour
    {
        [SerializeField] TMP_Text textMeshPro;
        [SerializeField] TextAnimator_TMP textAnimator;
    
        void Start()
        {
            // 🚫 하지 말 것: TMPro를 통해 텍스트 설정
            textMeshPro.SetText("<wave>hello");
    
            // ✅ 할 것: Text Animator를 통해 직접 텍스트 설정
            textAnimator.SetText("<wave>hello");
        }
    
    }

참고: TMPro를 참조해도 동작은 하지만, TextAnimator로 텍스트를 설정하면 텍스트에 대해 더 많은 제어가 가능하므로 통합이 더 잘됩니다.

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#undefined-1)

모범 사례


-------------------------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#undefined-2)

전체 텍스트/대사는 한 번만 설정하세요

가능하면 텍스트를 한 번만 설정하고, 타입라이터/가시성 메서드를 사용해 표시 방식을 제어하세요.

정말로 나중에 텍스트를 추가해야 한다면 "textAnimator.AppendText" 메서드를 사용할 수 있습니다.

예[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#undefined-3)

캐릭터가 "Helloooo how are you doing?"이라고 말하고 각 글자마다 표시하고 싶다면, 단순히 다음과 같이 하세요: `typewriter.ShowText("Hellooooo how are you doing?");` 그게 전부입니다! [글자 동적 표시 및 숨기기](https://docs.febucci.com/text-animator-unity/3.x-ko/typewriter/show-and-hide-letters-dynamically)

* * *

동적 문자열을 구성하는 경우에도 타입라이터/애니메이터에 값을 설정하기 전에 해당 문자열을 만들 수 있습니다.

Copy

    int apples = 5; // 나중에 게임 상태에서 가져옴
    string playerName = "Bob";
    
    // 먼저 전체 대사 라인을 구성하세요
    string dialogue = $"Hello {playerName}, you've got {apples} apples";
    
    // 그런 다음 텍스트를 한 번 설정하세요
    typewriter.ShowText(dialogue);

(대화 시스템을 사용하고 있다면, 그들이 대신 이 작업을 해줄 것입니다 - 걱정하지 마세요! [통합](https://docs.febucci.com/text-animator-unity/3.x-ko/integrations/integrated-plugins-and-dialogues-systems)
)

왜 전체 텍스트를 한 번에 설정하고 문자별로 설정하지 않아야 하나요?[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#undefined-4)

성능 때문입니다! (Text Animator가 없어도 마찬가지입니다.)

텍스트를 설정할 때마다 TextMeshPro나 UI 툴킷은 메시, 위치 등 계산을 수행해야 하고, Text Animator는 문자 지속시간 등을 다시 계산해야 합니다. 즉 초당 여러 번(예: 글자를 추가할 때) 변경하면 이 계산이 매번 수행됩니다.

문자를 하나씩 표시하려면 전체 텍스트를 한 번 설정한 다음 타입라이터를 시작하면 됩니다: [글자 동적 표시 및 숨기기](https://docs.febucci.com/text-animator-unity/3.x-ko/typewriter/show-and-hide-letters-dynamically)

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 설치 및 빠른 시작 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/install-and-quick-start

애셋을 사용하는 것은 몇 번의 클릭(import -> 컴포넌트 추가 -> 재생 버튼)으로 끝나지만, 모든 것을 더 잘 이해하려면 다음 페이지들을 살펴보셔서 더 빠르고 올바른 방향으로 시작할 수 있습니다.

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/install-and-quick-start#how-to-implement-text-animator)

1\. Text Animator for Unity 가져오기


----------------------------------------------------------------------------------------------------------------------------------------------------------------

가장 먼저 할 일은 프로젝트에 Text Animator for Unity를 가져오는 것입니다.

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/install-and-quick-start#undefined)

호환성 확인

**이 에셋은 다음 UI 및 유니티 버전과 호환됩니다**:

*   **Text Mesh Pro** (유니티 2022.3 이상)
    
*   **UI 툴킷** (유니티 6.3 이상).
    

새로운 유니티 입력 시스템(및 기존 시스템)도 지원합니다.

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/install-and-quick-start#undefined-1)

패키지 가져오기

프로젝트가 올바르게 설정되면 패키지 매니저(Asset Store 탭)에서 Text Animator를 가져올 수 있습니다.

"Samples/BuiltIn" 폴더를 포함했는지 확인하세요. 포함하지 않으면 애셋이 작동하지 않을 수 있습니다.

성공적으로 설치되면 **환영 창** 이 나타나고 Text Animator로 텍스트를 애니메이션할 준비가 됩니다!

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252Fo6lFhmxUPaki6oAtVVXZ%252FScreenshot%25202025-11-15%2520alle%252017.40.31.png%3Falt%3Dmedia%26token%3D729acbd3-556d-4808-9726-7f3918afec84&width=768&dpr=4&quality=100&sign=c733e2ae&sv=2)

가져온 후 표시되는 환영 창의 일부

환영 창이 나타나지 않거나 나중에 다시 보고 싶다면 언제든지 메뉴의 Tools/Febucci/TextAnimator/About Window에서 접근할 수 있습니다!

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/install-and-quick-start#id-2)

2\. 예제 씬


--------------------------------------------------------------------------------------------------------------

대부분의 Text Animator 기능은 인스펙터에서 직접 배우실 수 있고, 예제 씬에서 저희가 어떻게 설정했는지와 그 결과를 바로 확인할 수 있습니다.

"라는 이름의 씬에서 시작하세요.**00 - Welcome**", 또는 Text Animator의 환영 창에서 "Get Started"를 클릭하세요.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FLgTsSXatpKk3M2Nr36VN%252FScreenshot%25202025-11-15%2520alle%252017.45.47.png%3Falt%3Dmedia%26token%3D624c13da-2e67-4653-9caa-076cc5cfa24d&width=768&dpr=4&quality=100&sign=9e51b46f&sv=2)

예제 씬에 접근하려면 해당 씬들을 가져왔는지 확인하세요! 더 이상 필요하지 않으면 안전하게 제거/삭제하실 수 있습니다.

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/install-and-quick-start#animating-your-first-texts)

3\. 첫 번째 텍스트 애니메이션


----------------------------------------------------------------------------------------------------------------------------------------------

몇 번의 클릭으로 텍스트를 실행할 수 있습니다!

UI 툴킷

Text Mesh Pro

_추신. 이미 알고 있다고 가정합니다_ [_UI 툴킷을 사용하는 방법을_](https://docs.unity3d.com/Documentation/Manual/UIElements.html)
 _그리고 그것이 무엇을 하는지._

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/install-and-quick-start#ui)

UI 빌더에서

*   라이브러리 -> 프로젝트로 이동
    
*   드래그 "AnimatedLabel" 를 계층 구조의 "Custom Controls/Febucci/Text Animator for Unity"에서!
    

내장된 레이블과 버튼을 UI 툴킷에서 직접 애니메이션할 수 있도록 작업 중입니다! _(Unity 6.3 이상.)_ 업데이트를 확인하세요!

당신의 .uxml은 다음과 같이 보여야 합니다:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZNwCUmAugxLNcVVO9oBk%252FScreenshot%25202025-11-15%2520alle%252018.02.51.png%3Falt%3Dmedia%26token%3Dced34791-d558-4883-b646-2197664dd637&width=768&dpr=4&quality=100&sign=944232c1&sv=2)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/install-and-quick-start#undefined)

코드로

"의 인스턴스를 생성할 수 있습니다Febucci.TextAnimatorForUnity.AnimatedLabel" 클래스와 UI 문서에 추가하는 방법은 다음과 같습니다:

Copy

    using UnityEngine;
    using UnityEngine.UIElements;
    using Febucci.TextAnimatorForUnity; // <- Text Animator의 네임스페이스를 임포트
    
    public class ExampleScript : MonoBehaviour
    { 
        [SerializeField] UIDocument document;
    
        void Start()
        {
            var container = document.rootVisualElement.contentContainer;
            var animatedLabel = new AnimatedLabel(); // <- 애니메이션 레이블 생성
            container.Add(animatedLabel); // <- 컨텐츠 컨테이너에 추가
            // [..]
            animatedLabel.SetText("<wave>hello"); // <- 텍스트 설정
        }
    }

_추신. 이미 알고 있다고 가정합니다_ [_Text Mesh Pro 사용 방법_](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html)
 _그리고 그것이 어떻게 작동하는지._

추가하기 텍스트 애니메이터 - Text Mesh Pro 동일한 GameObject에 구성 요소를 추가하세요 TextMeshPro 구성 요소(UI 또는 월드 공간 중 하나!):

인스펙터는 다음과 같아야 합니다:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FT3h66pIPFdakGOCfToEY%252FScreenshot%25202025-11-15%2520alle%252017.59.18.png%3Falt%3Dmedia%26token%3D26196c49-f0f5-457b-85dd-da358f43c823&width=768&dpr=4&quality=100&sign=78164711&sv=2)

자세한 내용과 제안은 [텍스트 설정](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts)
 를 참조하세요!

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/install-and-quick-start#id-1-writing-effects-in-your-text)

텍스트에 이펙트 작성하기

텍스트에 이펙트를 추가하는 한 가지 방법은 다음과 같은 리치 텍스트 태그를 사용하는 것입니다: “`I'm <shake>freezing</shake>`”, 여기서 "shake"는 내장 이펙트의 ID입니다.

*   다음 태그들을 사용해 실험하면서 텍스트를 작성해 보세요: `<wiggle>` `<shake>` `<wave>` `<bounce>`, 예를 들어 “`<wiggle>I'm joking</wiggle> hehe now <shake>I'm scared</shake>`” 그런 다음 Unity의 재생 모드를 실행하세요.
    

작성한 이펙트에 따라 글자들이 애니메이션됩니다!

* * *

텍스트 애니메이션을 즐기세요! 애셋의 모든 기능을 더 깊이 살펴보려면 다음 페이지로 진행하세요.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 효과 추가 방법 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects

텍스트에 다음과 같은 방법으로 효과를 추가할 수 있습니다:

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects#set-effects-to-specific-parts-of-the-text)

텍스트의 특정 부분에 효과 설정

다음을 사용하여 텍스트의 특정 부분에 효과를 추가할 수 있습니다 **리치 텍스트 태그.**

효과 태그는 다음과 같이 보입니다:

*   **지속**: `<tagID>` 열기 위해, `</tagID>` 닫기 위해
    
*   **등장**: `{tagID}` 열기 위해, `{/tagID}` 닫기 위해
    
*   **사라짐**: `{#tagID}` 열기 위해, `{/#tagID}` 닫기 위해 _(기본적으로 앞에_ `_#_` _가 붙은 출현 태그로, 소멸은 출현의 역순임을 단순히 상기시키기 위해)_.
    

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects#extra-notes-about-rich-text-formatting)

리치 텍스트 서식에 대한 추가 메모

TextAnimator for Unity를 사용하면:

*   여러 효과를 겹쳐서 적용할 수 있습니다(예: “`<shake><size>`”). (또한 다음을 살펴보세요 [스타일](https://docs.febucci.com/text-animator-unity/3.x-ko/customization/styles)
    )
    
*   현재 열려 있는 효과를 닫을 수 있습니다 **모든** 단일 ‘`/`’ 문자로, 예를 들면:
    
    *   ”`</>`”는 지속 효과(Persistent Effects)를 위한 것입니다
        
    *   ”`{/}`”는 출현 효과(Appearance Effects)를 위한 것입니다
        
    *   ”`{/#}`”는 소멸 효과(Disappearance Effects)를 위한 것입니다.
        
    
*   텍스트의 끝에 있다면 태그를 닫을 필요가 없습니다. Text Animator는 태그를 열자마자 효과를 적용하기 시작하기 때문입니다. (예: "`<shake>hello`" 는 hello가 이미 애니메이션되는 결과를 낳습니다).
    

다양한

* * *

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects#set-default-effects-to-the-entire-text)

텍스트 전체에 기본 효과 설정

기본적으로 모든 글자에 어떤 효과가 적용될지 결정할 수 있습니다, **텍스트 안에 효과 태그를 쓰지 않고도** 덕분에 [애니메이터 설정](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects/animator-settings)
.

UI 툴킷

Text Mesh Pro

AnimatedLabel의 설정은 서로 다른 스크립터블 오브젝트로 관리됩니다(이 경우 아래 이미지에서 강조된 항목). 자세한 내용은 여기서 읽어보세요 [만드는 방법](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects/animator-settings)
.

설정하지 않았다면, [글로벌 설정](https://docs.febucci.com/text-animator-unity/3.x-ko/customization/global-settings)
 에 있는 것이 사용됩니다!

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FagcdeSBrmD3NCQLoVswt%252FScreenshot%25202025-11-15%2520alle%252018.33.00.png%3Falt%3Dmedia%26token%3D6d57fa83-5f8f-475d-940f-280151ce67d5&width=768&dpr=4&quality=100&sign=8de16ada&sv=2)

Text Mesh Pro의 경우 설정은 "로컬"(컴포넌트에 바인드)일 수도 있고, "공유"(다른 Text Animator 인스턴스들 간)일 수도 있습니다.

*   를 수정하려면, **로컬** 설정을 수정하려면 단순히 "TextAnimator - Text Mesh Pro" 컴포넌트 인스펙터로 가서 해당 값을 조정하세요.
    
*   를 수정하려면, **공유** 설정의 경우, 관련 ScriptableObject 인스턴스를 할당하세요. [자세한 내용은 여기를 읽어보세요](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects/animator-settings)
    .
    

설정 내부:

1.  “기본 태그(Default Tags)” 섹션을 방문하세요
    
2.  편집하려는 효과 카테고리를 확장하세요
    
3.  포함하려는 효과 태그를 원하는 대로 추가하세요. 예를 들어:
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FMGbfDEQeK1CRnktW6aue%252FScreenshot%25202025-11-15%2520alle%252018.48.23.png%3Falt%3Dmedia%26token%3D2a7db44e-c31a-48ae-a317-871ca6006070&width=768&dpr=4&quality=100&sign=81952131&sv=2)

기본적으로 아무 효과도 적용하고 싶지 않다면, 효과의 개수를 0으로 설정하세요.

다음도 변경할 수 있습니다 "**기본 태그 모드(Default Tags Mode)**" 를 "**상시**" 로 설정하면 효과가 항상 모든 것 위에 적용되도록 할 수 있습니다.

각 배열 요소에 "shake a=5" 같은 수식어(Modifier)를 추가할 수 있습니다. 자세한 내용은 여기에서 읽으세요: [수정자](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/modifiers)

예시: 대체(fallbacks)[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects#fallbacks)

예를 들어 기본 효과("size")가 하나 있지만 텍스트의 특정 부분에 "fade" 효과를 적용하고 싶다고 가정해봅시다. 다음과 같이 작성하면 그 결과를 얻을 수 있습니다: "default default \`{fade}\` fade fade fade \`{/fade}\` default default"

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FfkwPOWUP3UA38XjdRWRQ%2Ftext-animator-override-appearances-example-ezgif.com-video-to-gif-converter.gif&width=300&dpr=4&quality=100&sign=a2f2d030&sv=2)

보시다시피 "fade" 태그 밖에 있는 글자들은 기본 효과가 적용되고, "{fade}"와 "{/fade}" 안에 있는 부분은 "fade"만 적용됩니다.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 설치 및 빠른 시작 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start

애셋을 사용하는 것은 몇 번의 클릭(import -> 컴포넌트 추가 -> 재생 버튼)으로 끝나지만, 모든 것을 더 잘 이해하려면 다음 페이지들을 살펴보셔서 더 빠르고 올바른 방향으로 시작할 수 있습니다.

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start#how-to-implement-text-animator)

1\. Text Animator for Unity 가져오기


----------------------------------------------------------------------------------------------------------------------------------------

가장 먼저 할 일은 프로젝트에 Text Animator for Unity를 가져오는 것입니다.

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start#undefined)

호환성 확인

**이 에셋은 다음 UI 및 유니티 버전과 호환됩니다**:

*   **Text Mesh Pro** (유니티 2022.3 이상)
    
*   **UI 툴킷** (유니티 6.3 이상).
    

새로운 유니티 입력 시스템(및 기존 시스템)도 지원합니다.

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start#undefined-1)

패키지 가져오기

프로젝트가 올바르게 설정되면 패키지 매니저(Asset Store 탭)에서 Text Animator를 가져올 수 있습니다.

"Samples/BuiltIn" 폴더를 포함했는지 확인하세요. 포함하지 않으면 애셋이 작동하지 않을 수 있습니다.

성공적으로 설치되면 **환영 창** 이 나타나고 Text Animator로 텍스트를 애니메이션할 준비가 됩니다!

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252Fo6lFhmxUPaki6oAtVVXZ%252FScreenshot%25202025-11-15%2520alle%252017.40.31.png%3Falt%3Dmedia%26token%3D729acbd3-556d-4808-9726-7f3918afec84&width=768&dpr=4&quality=100&sign=c733e2ae&sv=2)

가져온 후 표시되는 환영 창의 일부

환영 창이 나타나지 않거나 나중에 다시 보고 싶다면 언제든지 메뉴의 Tools/Febucci/TextAnimator/About Window에서 접근할 수 있습니다!

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start#id-2)

2\. 예제 씬


--------------------------------------------------------------------------------------

대부분의 Text Animator 기능은 인스펙터에서 직접 배우실 수 있고, 예제 씬에서 저희가 어떻게 설정했는지와 그 결과를 바로 확인할 수 있습니다.

"라는 이름의 씬에서 시작하세요.**00 - Welcome**", 또는 Text Animator의 환영 창에서 "Get Started"를 클릭하세요.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FLgTsSXatpKk3M2Nr36VN%252FScreenshot%25202025-11-15%2520alle%252017.45.47.png%3Falt%3Dmedia%26token%3D624c13da-2e67-4653-9caa-076cc5cfa24d&width=768&dpr=4&quality=100&sign=9e51b46f&sv=2)

예제 씬에 접근하려면 해당 씬들을 가져왔는지 확인하세요! 더 이상 필요하지 않으면 안전하게 제거/삭제하실 수 있습니다.

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start#animating-your-first-texts)

3\. 첫 번째 텍스트 애니메이션


----------------------------------------------------------------------------------------------------------------------

몇 번의 클릭으로 텍스트를 실행할 수 있습니다!

UI 툴킷

Text Mesh Pro

_추신. 이미 알고 있다고 가정합니다_ [_UI 툴킷을 사용하는 방법을_](https://docs.unity3d.com/Documentation/Manual/UIElements.html)
 _그리고 그것이 무엇을 하는지._

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start#ui)

UI 빌더에서

*   라이브러리 -> 프로젝트로 이동
    
*   드래그 "AnimatedLabel" 를 계층 구조의 "Custom Controls/Febucci/Text Animator for Unity"에서!
    

내장된 레이블과 버튼을 UI 툴킷에서 직접 애니메이션할 수 있도록 작업 중입니다! _(Unity 6.3 이상.)_ 업데이트를 확인하세요!

당신의 .uxml은 다음과 같이 보여야 합니다:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZNwCUmAugxLNcVVO9oBk%252FScreenshot%25202025-11-15%2520alle%252018.02.51.png%3Falt%3Dmedia%26token%3Dced34791-d558-4883-b646-2197664dd637&width=768&dpr=4&quality=100&sign=944232c1&sv=2)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start#undefined)

코드로

"의 인스턴스를 생성할 수 있습니다Febucci.TextAnimatorForUnity.AnimatedLabel" 클래스와 UI 문서에 추가하는 방법은 다음과 같습니다:

Copy

    using UnityEngine;
    using UnityEngine.UIElements;
    using Febucci.TextAnimatorForUnity; // <- Text Animator의 네임스페이스를 임포트
    
    public class ExampleScript : MonoBehaviour
    { 
        [SerializeField] UIDocument document;
    
        void Start()
        {
            var container = document.rootVisualElement.contentContainer;
            var animatedLabel = new AnimatedLabel(); // <- 애니메이션 레이블 생성
            container.Add(animatedLabel); // <- 컨텐츠 컨테이너에 추가
            // [..]
            animatedLabel.SetText("<wave>hello"); // <- 텍스트 설정
        }
    }

_추신. 이미 알고 있다고 가정합니다_ [_Text Mesh Pro 사용 방법_](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html)
 _그리고 그것이 어떻게 작동하는지._

추가하기 텍스트 애니메이터 - Text Mesh Pro 동일한 GameObject에 구성 요소를 추가하세요 TextMeshPro 구성 요소(UI 또는 월드 공간 중 하나!):

인스펙터는 다음과 같아야 합니다:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FT3h66pIPFdakGOCfToEY%252FScreenshot%25202025-11-15%2520alle%252017.59.18.png%3Falt%3Dmedia%26token%3D26196c49-f0f5-457b-85dd-da358f43c823&width=768&dpr=4&quality=100&sign=78164711&sv=2)

자세한 내용과 제안은 [텍스트 설정](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts)
 를 참조하세요!

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start#id-1-writing-effects-in-your-text)

텍스트에 이펙트 작성하기

텍스트에 이펙트를 추가하는 한 가지 방법은 다음과 같은 리치 텍스트 태그를 사용하는 것입니다: “`I'm <shake>freezing</shake>`”, 여기서 "shake"는 내장 이펙트의 ID입니다.

*   다음 태그들을 사용해 실험하면서 텍스트를 작성해 보세요: `<wiggle>` `<shake>` `<wave>` `<bounce>`, 예를 들어 “`<wiggle>I'm joking</wiggle> hehe now <shake>I'm scared</shake>`” 그런 다음 Unity의 재생 모드를 실행하세요.
    

작성한 이펙트에 따라 글자들이 애니메이션됩니다!

* * *

텍스트 애니메이션을 즐기세요! 애셋의 모든 기능을 더 깊이 살펴보려면 다음 페이지로 진행하세요.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 효과 데이터베이스 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects/effects-database

이펙트는 데이터베이스에 저장됩니다. 데이터베이스 자체도 ScriptableObject입니다.

원하는 방식으로 어떤 데이터베이스에든 이펙트를 추가하거나 제거할 수 있으며, 여러 TextAnimator가 동일한 데이터베이스를 공유할 수도 있습니다. 기본적으로 모든 TextAnimator는 [글로벌 설정](https://docs.febucci.com/text-animator-unity/3.x-ko/customization/global-settings)
 파일의 "기본" 데이터베이스를 공유합니다.

**Text Animator는 어떤 이펙트가 존재하는지 알기 위해 이펙트 데이터베이스가 필요합니다**, 따라서 하나가 있는지 확인하세요!

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FVSXvT5lT5dntsMxKpb75%252FScreenshot%25202025-11-15%2520alle%252020.15.21.png%3Falt%3Dmedia%26token%3D3b2e7fdd-86fb-4193-9b33-6312916accc6&width=768&dpr=4&quality=100&sign=1c9b2294&sv=2)

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects/effects-database#undefined)

커스텀 데이터베이스 생성

프로젝트 뷰에서 마우스 오른쪽 버튼을 클릭 -> Create -> Text Animator for Unity를 선택한 다음 추가하려는 카테고리와 이펙트를 선택하여 새 이펙트를 만들 수 있습니다.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FyeZq580N8EGFfDW3tkwI%252FScreenshot%25202025-11-15%2520alle%252020.14.26.png%3Falt%3Dmedia%26token%3Df232bbae-c247-411f-ab0e-48bbc1ea1a42&width=768&dpr=4&quality=100&sign=efcebf59&sv=2)

각 ScriptableObject에서 이펙트 태그를 편집할 수도 있으므로, 예를 들어 대사가 "추움"을 전달할 때 적용되는 특정 "쉐이크" 이펙트와 "공포"를 전달할 때 적용되는 다른 이펙트를 목적에 따라 따로 만들 수 있습니다.

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 텍스트 설정 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/effects

텍스트 애니메이터에 텍스트를 설정하는 방법은 두 가지 UI 시스템에서 가능합니다:

*   [텍스트 설정](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#ui-toolkit)
    
*   [Text Mesh Pro](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#text-mesh-pro)
    

이 페이지에는 이미 [설치 및 빠른 시작](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/install-and-quick-start)
에 있는 일부 정보가 포함되어 있지만, 각 시스템 및 일반적인 사용에 대한 다른 세부사항과 제안들도 포함되어 있습니다. 반드시 [텍스트 설정](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/setting-up-texts#best-practices)
 섹션을 읽어보세요!

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects#ui)

UI 툴킷


-----------------------------------------------------------------------------

_추신. 이미 알고 있다고 가정합니다_ [_UI 툴킷을 사용하는 방법을_](https://docs.unity3d.com/Documentation/Manual/UIElements.html)
 _그리고 그것이 무엇을 하는지._

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects#ui)

UI 빌더에서

*   라이브러리 -> 프로젝트로 이동
    
*   드래그 "AnimatedLabel" 를 계층 구조의 "Custom Controls/Febucci/Text Animator for Unity"에서!
    

내장된 레이블과 버튼을 UI 툴킷에서 직접 애니메이션할 수 있도록 작업 중입니다! _(Unity 6.3 이상.)_ 업데이트를 확인하세요!

당신의 .uxml은 다음과 같이 보여야 합니다:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZNwCUmAugxLNcVVO9oBk%252FScreenshot%25202025-11-15%2520alle%252018.02.51.png%3Falt%3Dmedia%26token%3Dced34791-d558-4883-b646-2197664dd637&width=768&dpr=4&quality=100&sign=944232c1&sv=2)

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects#undefined)

코드로

"의 인스턴스를 생성할 수 있습니다Febucci.TextAnimatorForUnity.AnimatedLabel" 클래스와 UI 문서에 추가하는 방법은 다음과 같습니다:

Copy

    using UnityEngine;
    using UnityEngine.UIElements;
    using Febucci.TextAnimatorForUnity; // <- Text Animator의 네임스페이스를 임포트
    
    public class ExampleScript : MonoBehaviour
    { 
        [SerializeField] UIDocument document;
    
        void Start()
        {
            var container = document.rootVisualElement.contentContainer;
            var animatedLabel = new AnimatedLabel(); // <- 애니메이션 레이블 생성
            container.Add(animatedLabel); // <- 컨텐츠 컨테이너에 추가
            // [..]
            animatedLabel.SetText("<wave>hello"); // <- 텍스트 설정
        }
    }

이제 끝났습니다!! 당신은 준비가 되었습니다 [효과 추가 방법](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects)

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects#text-mesh-pro)

Text Mesh Pro


------------------------------------------------------------------------------------------------

_추신. 이미 알고 있다고 가정합니다_ [_Text Mesh Pro 사용 방법_](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html)
 _그리고 그것이 어떻게 작동하는지._

추가하기 텍스트 애니메이터 - Text Mesh Pro 동일한 GameObject에 구성 요소를 추가하세요 TextMeshPro 구성 요소(UI 또는 월드 공간 중 하나!):

인스펙터는 다음과 같아야 합니다:

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FT3h66pIPFdakGOCfToEY%252FScreenshot%25202025-11-15%2520alle%252017.59.18.png%3Falt%3Dmedia%26token%3D26196c49-f0f5-457b-85dd-da358f43c823&width=768&dpr=4&quality=100&sign=78164711&sv=2)

이제 끝났습니다!! 당신은 준비가 되었습니다 [효과 추가 방법](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects)

컴포넌트에 텍스트를 설정했는데 빈 텍스트가 보인다면, TextMeshPro 컴포넌트를 최소 한 번 클릭해 "Essentials"를 가져왔는지 확인하세요(창이 뜨면 가져오라는 요청이 표시됩니다).

#### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects#undefined)

코드로 텍스트를 설정할 때의 모범 사례

코드로 TextMeshPro 객체에 텍스트를 설정할 때는 TMPro 대신 Text Animator의 스크립트를 참조하세요. 예:

Copy

    using UnityEngine;
    using TMPro; 
    using Febucci.TextAnimatorForUnity.TextMeshPro; // <- Text Animator의 네임스페이스를 임포트
    
    public class ExampleScript : MonoBehaviour
    {
        [SerializeField] TMP_Text textMeshPro;
        [SerializeField] TextAnimator_TMP textAnimator;
    
        void Start()
        {
            // 🚫 하지 말 것: TMPro를 통해 텍스트 설정
            textMeshPro.SetText("<wave>hello");
    
            // ✅ 할 것: Text Animator를 통해 직접 텍스트 설정
            textAnimator.SetText("<wave>hello");
        }
    
    }

참고: TMPro를 참조해도 동작은 하지만, TextAnimator로 텍스트를 설정하면 텍스트에 대해 더 많은 제어가 가능하므로 통합이 더 잘됩니다.

* * *

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects#undefined-1)

모범 사례


--------------------------------------------------------------------------------------

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects#undefined-2)

전체 텍스트/대사는 한 번만 설정하세요

가능하면 텍스트를 한 번만 설정하고, 타입라이터/가시성 메서드를 사용해 표시 방식을 제어하세요.

정말로 나중에 텍스트를 추가해야 한다면 "textAnimator.AppendText" 메서드를 사용할 수 있습니다.

예[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects#undefined-3)

캐릭터가 "Helloooo how are you doing?"이라고 말하고 각 글자마다 표시하고 싶다면, 단순히 다음과 같이 하세요: `typewriter.ShowText("Hellooooo how are you doing?");` 그게 전부입니다! [글자 동적 표시 및 숨기기](https://docs.febucci.com/text-animator-unity/3.x-ko/typewriter/show-and-hide-letters-dynamically)

* * *

동적 문자열을 구성하는 경우에도 타입라이터/애니메이터에 값을 설정하기 전에 해당 문자열을 만들 수 있습니다.

Copy

    int apples = 5; // 나중에 게임 상태에서 가져옴
    string playerName = "Bob";
    
    // 먼저 전체 대사 라인을 구성하세요
    string dialogue = $"Hello {playerName}, you've got {apples} apples";
    
    // 그런 다음 텍스트를 한 번 설정하세요
    typewriter.ShowText(dialogue);

(대화 시스템을 사용하고 있다면, 그들이 대신 이 작업을 해줄 것입니다 - 걱정하지 마세요! [통합](https://docs.febucci.com/text-animator-unity/3.x-ko/integrations/integrated-plugins-and-dialogues-systems)
)

왜 전체 텍스트를 한 번에 설정하고 문자별로 설정하지 않아야 하나요?[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects#undefined-4)

성능 때문입니다! (Text Animator가 없어도 마찬가지입니다.)

텍스트를 설정할 때마다 TextMeshPro나 UI 툴킷은 메시, 위치 등 계산을 수행해야 하고, Text Animator는 문자 지속시간 등을 다시 계산해야 합니다. 즉 초당 여러 번(예: 글자를 추가할 때) 변경하면 이 계산이 매번 수행됩니다.

문자를 하나씩 표시하려면 전체 텍스트를 한 번 설정한 다음 타입라이터를 시작하면 됩니다: [글자 동적 표시 및 숨기기](https://docs.febucci.com/text-animator-unity/3.x-ko/typewriter/show-and-hide-letters-dynamically)

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 애니메이터 설정 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects/animator-settings

"애니메이터 설정" ([로컬이든 글로벌이든](https://docs.febucci.com/text-animator-unity/3.x-ko/quick-start/core-concepts#settings-accessibility)
) 은 효과가 적용되고 표시되는 방식에 대한 많은 옵션을 포함합니다.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FfTe0N94riod0U2VKoRVi%252FScreenshot%25202025-11-15%2520alle%252018.39.36.png%3Falt%3Dmedia%26token%3D5e297e45-eb51-4eb9-9694-9c2028a893b8&width=768&dpr=4&quality=100&sign=966710ea&sv=2)

이들은 스스로 이해되기 쉬워야 합니다(다음 버전부터 툴팁도 추가할 예정입니다!), 하지만 일부 옵션에 대한 추가 설명은 다음과 같습니다:

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects/animator-settings#undefined)

기본 효과 모드

아래 배열 중 어느 하나에 최소한 하나의 "기본 태그" 요소를 설정한 경우, "기본 효과 모드"를 통해 이러한 태그가 글자에 어떻게 적용되는지 결정할 수 있습니다.

*   **폴백**: 해당 글자에 이미 영향을 미치는 다른 효과/태그가 없는 경우 이 태그들이 적용됩니다
    
*   **상시**: 이 태그들은 _모든_ 텍스트에 적용됩니다(다른 효과가 있는 경우 그 위에 누적됩니다)
    

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects/animator-settings#text-animator)

타이프라이터는 연결된 Text Animator의

효과의 `타임스케일` 모드는 “TextAnimator” 컴포넌트 인스펙터에서 변경할 수 있습니다.

*   **스케일 연동**: 효과는 게임의 Time.timeScale에 따라 느려지거나 일시정지됩니다 ([Unity 참고](https://docs.unity3d.com/ScriptReference/Time-timeScale.html)
    )
    
*   **비연동**: 게임이 일시정지되어도(Time.timeScale = 0) 효과는 비연동/독립 시간으로 업데이트됩니다.
    

타자기 효과를 활성화한 경우, 그 타임스케일은 **상대적인 TextAnimator의 타임스케일과 일치합니다** (즉, 이를 “비연동”으로 설정하면 게임이 일시정지된 상태에서도 글자를 표시할 수 있습니다).

게임 타임스케일이 음수이면 TextAnimator는 일시정지된 것처럼 동작하지만, 타임스케일이 0보다 커지면 자동으로 다시 재개됩니다.

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects/animator-settings#undefined-1)

동적 스케일링

Text Animator는 기본적으로 다양한 화면 해상도에서 일관된 효과 결과를 얻도록 설계되었으며, 이 기능을 활성화된 상태로 유지하는 것을 권장합니다.

설명[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-add-effects/animator-settings#undefined-2)

플레이어들은 서로 다른 화면 크기(모바일부터 모니터 등)를 가질 가능성이 높으므로, 글자를 "50픽셀" 이동시키는 것이 너무 크거나 너무 작게 보일 수 있습니다. 디자이너로서 여러분은 모든 사용자에게 의도한 대로 일관된 경험/결과를 원할 것입니다. 이 때문에 "동적 스케일 사용"을 활성화해 두고 현재 컴퓨터의 글꼴 크기를 기준으로 값을 편집할 것을 강력히 권장합니다(나중에 무엇이 변경되든 동일한 비율을 유지합니다).

*   `참조 글꼴 크기`: 객체가 예상대로 동작하는 크기를 나타냅니다. 참고로, 테스트하는 동안 유니티 에디터에서 글꼴 크기를 선택할 수 있습니다.
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FK4qC74LIOHiJjZWbZrCl%2Ftextanimator%2520unity%2520dynamic%2520scaling.png&width=768&dpr=4&quality=100&sign=931e1049&sv=2)

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 효과 편집 방법 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects

프로젝트 창에서 해당 이펙트의 Scriptable Object를 클릭하면 언제든지 편집할 수 있습니다. 편집 모드에서(유니티 6.3부터, 다른 버전은 가능한 빨리 제공 예정) 라이브 미리보기를 통해 글자의 서로 다른 단계(등장, 소멸 및 지속)에 이펙트가 어떻게 적용되는지 확인할 수 있습니다.

Rich Text 태그를 통해서도 이펙트를 수정할 수 있습니다, [수정자](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/modifiers)
 (예: **<wave s=2>** 로 두 배 빠르게 만듭니다).

* * *

항상 다음을 설정하는 것이 중요합니다 **태그 ID**를 설정하지 않으면 데이터베이스에서 이펙트를 인식하지 못합니다!

인스펙터에서는 다음과 같은 이펙트를 더욱 수정하는 데 유용한 추가 매개변수도 찾을 수 있습니다:

*   **커브 베이크**: 이것을 켜둔 상태로 유지하세요! 특히 많은 글자에 여러 이펙트를 적용하는 등 성능에 민감한 상황에서 이펙트를 최적화해줍니다
    
*   **기본값을 덮어쓰기** [글로벌 설정](https://docs.febucci.com/text-animator-unity/3.x-ko/customization/global-settings)
     커스텀 [커브](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/curves)
     또는 [재생(플레이백)](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/playbacks)
     보다
    

지속형 동기화 시간은 작업 중입니다! 피드백을 알려주세요!

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252F6JMmtL11b32xG7FmgEv7%252FScreenshot%25202025-11-16%2520alle%252017.18.36.png%3Falt%3Dmedia%26token%3Db4a87c65-eb10-44be-864a-c27ceba45445&width=768&dpr=4&quality=100&sign=3039622f&sv=2)

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 커브 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/curves

이펙트는 "state **curve"**를 기반으로 글자 상태(위치, 회전, 크기 등)를 수정하며, 이는 인스펙터에서 할당할 수 있습니다.

항상 그렇듯이, **곡선** 은 스크립터블 오브젝트이며 클릭하면 인스펙터에 미리보기가 표시됩니다.

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/curves#undefined)

내장 곡선

**사인**

사인 곡선을 따릅니다(출현 시 이징 적용)

**선형**

0에서 1까지 선형으로 이동합니다

**홀드**

항상 1에 머뭅니다

**스퀘어**

1 또는 -1 중 하나입니다

**스텝**

0에서 1로 네 단계로 이동합니다

**바운스**

0에서 1로 바운스합니다

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/curves#undefined-1)

인스펙터에서 커스텀 곡선 생성하기

인스펙터에서 커스텀 곡선을 만들려면 Project->Create->Text Animator for Unity로 이동한 다음 "**Custom**".

패널 하단에서 편집 가능한 두 개의 곡선을 포함한 인스펙터를 찾을 수 있습니다.

*   **Curve01** 은 0에서 1로 이동하며 출현과 소멸의 동작을 결정합니다
    
*   **CurveRange** 은 -1에서 1로 이동합니다(부드럽고 이음새 없는 루프를 형성하도록 시작 위치로 끝납니다)이며 지속 효과에 영향을 줍니다
    

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252FZv0T9uTeTrdB1zcBiDNj%252FScreenshot%25202025-11-16%2520alle%252017.04.29.png%3Falt%3Dmedia%26token%3Dd2fc3da7-4456-4cd3-b724-ecf9910219a4&width=768&dpr=4&quality=100&sign=f4e17056&sv=2)

* * *

를 통해 곡선을 설정하는 방법이 [수정자](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/modifiers)
 (유사한 [재생(플레이백)](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/playbacks)
) 향후 릴리스에서 제공될 예정입니다!

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 내장된 효과 목록 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/effects/built-in-effects-list

다음은 이미 사용 가능한 기본/내장 데이터베이스입니다(“Samples” 폴더를 가져오세요!) 및 게임에서 바로 사용할 수 있는 많은 효과를 포함하고 있습니다.

언제든지 자신만의 효과를 만들 수도 있습니다!

*   [나만의 효과 만들기](https://docs.febucci.com/text-animator-unity/3.x-ko/customization/create-your-own-effects)
    
*   [커스텀 효과 작성 (C#)](https://docs.febucci.com/text-animator-unity/3.x-ko/writing-custom-classes/c)
    

Text Animator for Unity 3.0부터 모든 효과는 Appearance(출현), Persistant(지속) 및 Disappearance(소멸)로 재생할 수 있으며, 한 번만 재생하거나 다른 조건에 따라 재생할 수도 있습니다!

다음도 사용할 수 있습니다 [수정자](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/modifiers)
이는 Behavior 효과의 특성을 개별적으로 변경할 수 있게 해줍니다.

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2F577I8LcLJl1quOreidHC%2Fpendulumpreview.gif&width=490&dpr=4&quality=100&sign=fa9422c7&sv=2)

**진자**

태그

pend

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FewfXieMBJaRjEcihXyeT%2Fdanglepreview.gif&width=490&dpr=4&quality=100&sign=d00e4c63&sv=2)

**댕글**

태그

dangle

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2Fd0wCTrvN7t49jUBGNqI0%2Ffadepreview.gif&width=490&dpr=4&quality=100&sign=4a33090f&sv=2)

**페이드**

태그

fade

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FCbIcUivK6TUlvvPHQx9l%2Frainbowpreviewfebucci.gif&width=490&dpr=4&quality=100&sign=fa7368ab&sv=2)

**무지개**

태그

rainb

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FONRSbf0b6oeC6tUYL7Ef%2Frotatingpreviewfebucci.gif&width=490&dpr=4&quality=100&sign=2bfdc2cd&sv=2)

**회전**

태그

rot

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2Fbhm0HLqRADQj3RCVHUN2%2Fbouncepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=ba59014d&sv=2)

**바운스**

태그

bounce

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FImNDiBy3MuZpT1fB0UxF%2Fslidepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=5c1b22c2&sv=2)

**슬라이드**

태그

slideh

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2F9zSq1hqy61sKFcWpOxNI%2Fswingpreviewfebucci.gif&width=490&dpr=4&quality=100&sign=dec9d5f5&sv=2)

**스윙**

태그

swing

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FaZftI1kdTYBEZedse6qJ%2Fwavepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=9cb0fc71&sv=2)

**웨이브**

태그

wave

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2FT3x704G3ZSzv4Hi4h4jA%2Fsizepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=8e27b570&sv=2)

**크기 증가**

태그

incr

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2F21sLOk7GG8dv7I0XaGMO%2Fshakepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=13725beb&sv=2)

**쉐이크**

태그

shake

![Cover](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2Fcontent.gitbook.com%2Fcontent%2FXuXUTa2X5PYuYL6yRvl1%2Fblobs%2Fcwposy2qWMvqTWq81T5e%2Fwigglepreviewfebucci.gif&width=490&dpr=4&quality=100&sign=b01cd84b&sv=2)

**위글**

태그

wiggle

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/built-in-effects-list#undefined)

용어집

수정자 ID

수정자 값

이름

다른 말로 하면

a

부동 소수점 숫자, 예: 3

진폭

효과의 강도

s

부동 소수점 숫자, 예: 3

속도

속도

*   `태그`: 효과 태그를 나타내며 카테고리 내에서 고유합니다(예: <shake>)
    

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

# 📄 단계 | 3.X (KO) | Text Animator for Unity
**Source:** https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/phases

A "**phase**"는 효과가 문자들 사이에서 어떻게 달라지는지를 설명합니다.

![](https://docs.febucci.com/text-animator-unity/~gitbook/image?url=https%3A%2F%2F3113271786-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252FXuXUTa2X5PYuYL6yRvl1%252Fuploads%252Fe0rVNhfYkoaST18lc2so%252FClipboard-20251116-152040-561.gif%3Falt%3Dmedia%26token%3Dae40450e-cf37-4859-9e27-7b05a986a44d&width=768&dpr=4&quality=100&sign=d44b971f&sv=2)

검사기에서 또는 리치 텍스트 태그를 통해 효과 단계를 수정할 수 있습니다 [수정자](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/modifiers)
.

**문자 오프셋**

문자들 간의 시간 차이

ModifierID

i

**단어 오프셋**

단어 간의 시간 차이

ModifierID

w

**속도**

효과 속도(또한 영향을 줌 [재생(플레이백)](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/playbacks)
)

ModifierID

s

리치 텍스트 태그 예:

*   효과를 두 배로 빠르게 만들기: "<wave s=2\>"
    
*   오프셋 수정: "<wave i=.1 w=.3\>" (문자 오프셋을 0.1로, 단어 오프셋을 0.3으로 설정합니다)
    

### 

[](https://docs.febucci.com/text-animator-unity/3.x-ko/effects/how-to-edit-effects/phases#undefined)

오프셋에 대한 추가 메모

*   오프셋이 0이거나 1이면 모든 문자에 대해 효과가 동일하다는 뜻입니다
    
*   0에서 0.5로 가면 효과가 한 방향으로 이동하고, 1에서 0.5로 가면 반대 방향으로 이동합니다(여기서 0.5가 더 큽니다)
    
*   오프셋이 0.5이면 한 문자는 한 방향에 있고 다른 문자는 반대 방향에 있다는 뜻입니다
    

* * *

Last updated 1 month ago

This site uses cookies to deliver its service and to analyze traffic. By browsing this site, you accept the [privacy policy](https://www.febucci.com/privacy_policy/)
.

AcceptReject

---

