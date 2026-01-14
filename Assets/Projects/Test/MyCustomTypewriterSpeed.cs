using UnityEngine;
using Febucci.TextAnimatorCore;
using Febucci.TextAnimatorCore.Text;
using Febucci.TextAnimatorForUnity;

// 这行代码会让它出现在右键菜单中
[CreateAssetMenu(menuName = "Text Animator/Custom Typing Waits")] 
public class MyCustomTypewriterSpeed : TypingsTimingsScriptableBase
{
    [SerializeField] float normalDelay = 0.03f;
    [SerializeField] float punctuationDelay = 0.1f;

    public override float GetWaitAppearanceTimeOf(CharacterData character, TextAnimator animator)
    {
        // 如果是标点符号，停顿久一点
        if (char.IsPunctuation(character.info.character))
            return punctuationDelay;

        // 否则返回普通速度
        return normalDelay;
    }
    
}