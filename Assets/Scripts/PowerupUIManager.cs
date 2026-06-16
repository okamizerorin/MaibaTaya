using UnityEngine;
using UnityEngine.UI;

public class PowerupUIManager : MonoBehaviour
{
    public Transform container;
    public GameObject powerupPrefab;

    public Sprite shieldIcon;
    public Sprite speedIcon;
    public Sprite coinsIcon;
    public Sprite jumpIcon;

    public void Show(PowerupType type, float duration)
    {
        GameObject obj = Instantiate(powerupPrefab, container);

        PowerupTimerUI ui = obj.GetComponent<PowerupTimerUI>();

        Image icon = obj.transform.Find("Icon").GetComponent<Image>();

        switch (type)
        {
            case PowerupType.Shield:
                icon.sprite = shieldIcon;
                break;

            case PowerupType.DoubleSpeed:
                icon.sprite = speedIcon;
                break;

            case PowerupType.DoubleCoins:
                icon.sprite = coinsIcon;
                break;

            case PowerupType.HighJump:
                icon.sprite = jumpIcon;
                break;
        }

        ui.fillImage = obj.transform.Find("Timer").GetComponent<Image>();
        ui.canvasGroup = obj.GetComponent<CanvasGroup>();
        ui.glow = obj.transform.Find("Glow").gameObject;

        ui.StartTimer(duration);
    }
}