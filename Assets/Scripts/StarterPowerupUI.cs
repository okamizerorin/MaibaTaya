using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class StarterPowerupUI : MonoBehaviour
{
    [Header("Starter UI")]
    public GameObject panel;

    public Button shieldButton;
    public Button speedButton;
    public Button coinsButton;

    public Image shieldDim;
    public Image speedDim;
    public Image coinsDim;

    public TMP_Text shieldCountText;
    public TMP_Text speedCountText;
    public TMP_Text coinsCountText;

    public float duration = 10f;

    private bool shieldUsed;
    private bool speedUsed;
    private bool coinsUsed;

    GamePowerups powerups;

    void Start()
    {
        if (powerups == null) powerups = FindObjectOfType<GamePowerups>();

        if (PlayerInventory.Instance == null)
        {
            return;
        }

        panel.SetActive(true);

        SetupButtons();
        UpdateCounts();

        StartCoroutine(HideAfterDelay());
    }

    void SetupButtons()
    {
        // reset states
        shieldUsed = false;
        speedUsed = false;
        coinsUsed = false;

        shieldDim.gameObject.SetActive(false);
        speedDim.gameObject.SetActive(false);
        coinsDim.gameObject.SetActive(false);

        shieldButton.onClick.RemoveAllListeners();
        speedButton.onClick.RemoveAllListeners();
        coinsButton.onClick.RemoveAllListeners();

        shieldButton.onClick.AddListener(UseShield);
        speedButton.onClick.AddListener(UseSpeed);
        coinsButton.onClick.AddListener(UseCoins);

        // nothing in store
        shieldButton.interactable = PlayerInventory.Instance.ShieldStarterCount > 0;
        speedButton.interactable = PlayerInventory.Instance.SpeedStarterCount > 0;
        coinsButton.interactable = PlayerInventory.Instance.DoubleCoinsStarterCount > 0;
    }

    // owned textt
    void UpdateCounts()
    {
        shieldCountText.text = "x" + PlayerInventory.Instance.ShieldStarterCount;
        speedCountText.text = "x" + PlayerInventory.Instance.SpeedStarterCount;
        coinsCountText.text = "x" + PlayerInventory.Instance.DoubleCoinsStarterCount;

        shieldButton.interactable = !shieldUsed && PlayerInventory.Instance.ShieldStarterCount > 0;
        speedButton.interactable = !speedUsed && PlayerInventory.Instance.SpeedStarterCount > 0;
        coinsButton.interactable = !coinsUsed && PlayerInventory.Instance.DoubleCoinsStarterCount > 0;
    }

    // use starters
    void UseShield()
    {
        if (shieldUsed) return;

        if (PlayerInventory.Instance.UseShieldStarter())
        {
            powerups.ActivateShield();
            shieldUsed = true;
            shieldDim.gameObject.SetActive(true);

            UpdateCounts();
        }
    }

    void UseSpeed()
    {
        if (speedUsed) return;

        if (PlayerInventory.Instance.UseSpeedStarter())
        {
            powerups.ActivateDoubleSpeed();
            speedUsed = true;
            speedDim.gameObject.SetActive(true);

            UpdateCounts();
        }
    }

    void UseCoins()
    {
        if (coinsUsed) return;

        if (PlayerInventory.Instance.UseDoubleCoinsStarter())
        {
            powerups.ActivateDoubleCoins();
            coinsUsed = true;
            coinsDim.gameObject.SetActive(true);

            UpdateCounts();
        }
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(duration);

        panel.SetActive(false);
    }
}