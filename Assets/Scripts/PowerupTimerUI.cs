using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PowerupTimerUI : MonoBehaviour
{
    [Header("References")]
    public Image fillImage;
    public CanvasGroup canvasGroup;
    public GameObject glow;

    [Header("Settings")]
    public float fadeDuration = 0.25f;

    private Tween fillTween;
    private Tween fadeTween;
    private Tween glowTween;

    LayoutElement layout;

    void Awake()
    {
        layout = GetComponent<LayoutElement>();

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);

        if (glow != null)
            glow.SetActive(false);
    }

    public void StartTimer(float duration)
    {
        // reset old animations
        fillTween?.Kill();
        fadeTween?.Kill();
        glowTween?.Kill();

        gameObject.SetActive(true);

        // join layout stack
        if (layout != null)
            layout.ignoreLayout = false;

        // move to TOP of stack
        transform.SetAsFirstSibling();

        // fade in
        canvasGroup.alpha = 0f;
        fadeTween = canvasGroup.DOFade(1f, fadeDuration);

        // glow ON + pulse
        if (glow != null)
        {
            glow.SetActive(true);
            glow.transform.localScale = Vector3.one;

            glowTween = glow.transform
                .DOScale(1.15f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        // reset fill
        fillImage.fillAmount = 1f;

        // countdown
        fillTween = fillImage
            .DOFillAmount(0f, duration)
            .SetEase(Ease.Linear)
            .OnComplete(OnEnd);
    }

    void OnEnd()
    {
        fillTween?.Kill();
        fadeTween?.Kill();
        glowTween?.Kill();

        fadeTween = canvasGroup
            .DOFade(0f, fadeDuration)
            .OnComplete(() =>
            {
                if (layout != null)
                    layout.ignoreLayout = true;

                if (glow != null)
                {
                    glow.SetActive(false);
                    glow.transform.localScale = Vector3.one;
                }

                Destroy(gameObject);
            });
    }

    public void StopTimer()
    {
        fillTween?.Kill();
        fadeTween?.Kill();
        glowTween?.Kill();

        canvasGroup.DOFade(0f, fadeDuration)
            .OnComplete(() => Destroy(gameObject));
    }
}