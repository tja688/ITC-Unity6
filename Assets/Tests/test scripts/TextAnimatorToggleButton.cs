using Febucci.TextAnimatorForUnity.TextMeshPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[DisallowMultipleComponent]
public class TextAnimatorToggleButton : MonoBehaviour
{
    [SerializeField] private TextAnimator_TMP mTextAnimator;
    [SerializeField, TextArea(2, 4)] private string mText =
        "{fade}{#fade}Text animator text for unity. Projext for Itc.{/#fade}{/fade}";

    private Button mButton;
    private bool mIsVisible;

    private void Awake()
    {
        if (!TryGetComponent(out mButton))
        {
            Debug.LogError("Missing Button component.", this);
            enabled = false;
            return;
        }

        if (mTextAnimator == null)
        {
            mTextAnimator = FindAnyObjectByType<TextAnimator_TMP>();
            if (mTextAnimator == null)
            {
                Debug.LogError("Missing TextAnimator_TMP reference.", this);
                enabled = false;
            }
        }
    }

    private void Start()
    {
        if (mTextAnimator == null)
            return;

        mTextAnimator.SetText(mText, true);
        mIsVisible = false;
    }

    private void OnEnable()
    {
        if (mButton != null)
            mButton.onClick.AddListener(ToggleText);
    }

    private void OnDisable()
    {
        if (mButton != null)
            mButton.onClick.RemoveListener(ToggleText);
    }

    private void ToggleText()
    {
        if (mTextAnimator == null)
            return;

        mIsVisible = !mIsVisible;
        mTextAnimator.SetVisibilityEntireText(mIsVisible, true);
    }
}
