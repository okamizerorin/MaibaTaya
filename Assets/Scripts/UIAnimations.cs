using UnityEngine;
using DG.Tweening;

public class UIAnimations : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform rect;

    [Header("Timing")]
    public float duration = 0.25f;

    [Header("Scale Settings")]
    public float startScale = 0f;
    public float overshootScale = 1.05f;  //bounce

    private Vector3 originalScale;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (rect == null)
            rect = GetComponent<RectTransform>();

        originalScale = rect.localScale;
    }

    public void ShowPanel()
    {
        AnimateIn();
    }

    public void AnimateIn()
    {
        rect.DOKill();
        canvasGroup.DOKill();

        gameObject.SetActive(true);

        canvasGroup.alpha = 0f;
        rect.localScale = originalScale * startScale;

        canvasGroup.DOFade(1f, duration * 0.8f);

        Sequence seq = DOTween.Sequence();

        seq.Append(rect.DOScale(originalScale * overshootScale, duration * 0.7f)
            .SetEase(Ease.OutBack));

        seq.Append(rect.DOScale(originalScale, duration * 0.3f)
            .SetEase(Ease.OutQuad));
    }

    public void HidePanel()
    {
        AnimateOut();
    }

    public void AnimateOut(System.Action onComplete = null)
    {
        rect.DOKill();
        canvasGroup.DOKill();

        canvasGroup.DOFade(0f, duration * 0.7f);

        rect.DOScale(originalScale * 0f, duration * 0.7f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }
}