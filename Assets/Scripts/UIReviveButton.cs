using UnityEngine;
using UnityEngine.UI;

public class UIReviveButton : MonoBehaviour
{
    public Button icewaterButton;
    public GameObject noIceTubigText;

    void Start()
    {
        UpdateButtonState();
    }

    void UpdateButtonState()
    {
        if (PlayerInventory.Instance == null) return;

        bool hasRevives = PlayerInventory.Instance.IceTubigCount > 0;
        icewaterButton.interactable = true;
    }

    public void OnReviveClicked()
    {
        if (PlayerInventory.Instance.IceTubigCount <= 0)
        {
            ShowFloatingText();
        }
    }

    void ShowFloatingText()
    {
        if (noIceTubigText == null) return;

        noIceTubigText.SetActive(true);
        CancelInvoke(nameof(HideFloatingText));
        Invoke(nameof(HideFloatingText), 1.2f);
    }

    void HideFloatingText()
    {
        noIceTubigText.SetActive(false);
    }
}