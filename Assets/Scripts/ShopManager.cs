using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public enum ShopItem
    {
        IceTubig,
        ShieldStarter,
        SpeedStarter,
        DoubleCoinsStarter
    }

    [Header("UI Panels")]
    public GameObject iceTubigPanel;
    public Slider iceTubigSlider;
    public Button iceTubigBuyButton;
    public TextMeshProUGUI iceTubigOwnedText;
    public TextMeshProUGUI iceTubigTotalPriceText;

    public GameObject shieldPanel;
    public Slider shieldSlider;
    public Button shieldBuyButton;
    public TextMeshProUGUI shieldOwnedText;
    public TextMeshProUGUI shieldTotalPriceText;

    public GameObject speedPanel;
    public Slider speedSlider;
    public Button speedBuyButton;
    public TextMeshProUGUI speedOwnedText;
    public TextMeshProUGUI speedTotalPriceText;

    public GameObject duoCoinPanel;
    public Slider duoCoinSlider;
    public Button duoCoinBuyButton;
    public TextMeshProUGUI duoCoinOwnedText;
    public TextMeshProUGUI duoCoinTotalPriceText;

    [Header("Confirm")]
    public GameObject confirmationPanel;
    public TextMeshProUGUI confirmationText;

    [Header("Prices")]
    public int tubigPrice = 100;
    public int shieldPrice = 150;
    public int speedPrice = 200;
    public int coinsStarterPrice = 200;

    private ShopItem currentItem;
    private int currentAmount = 1;

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        // sliders
        iceTubigSlider.minValue = 1; iceTubigSlider.maxValue = 5; iceTubigSlider.wholeNumbers = true;
        shieldSlider.minValue = 1; shieldSlider.maxValue = 5; shieldSlider.wholeNumbers = true;
        speedSlider.minValue = 1; speedSlider.maxValue = 5; speedSlider.wholeNumbers = true;
        duoCoinSlider.minValue = 1; duoCoinSlider.maxValue = 5; duoCoinSlider.wholeNumbers = true;

        iceTubigSlider.onValueChanged.AddListener((v) 
            => OnAmountChanged(v, ShopItem.IceTubig));
        shieldSlider.onValueChanged.AddListener((v)
            => OnAmountChanged(v, ShopItem.ShieldStarter));
        speedSlider.onValueChanged.AddListener((v) 
            => OnAmountChanged(v, ShopItem.SpeedStarter));
        duoCoinSlider.onValueChanged.AddListener((v) 
            => OnAmountChanged(v, ShopItem.DoubleCoinsStarter));

        // buy buttons
        iceTubigBuyButton.onClick.AddListener(() 
            => ConfirmPurchase(ShopItem.IceTubig));
        shieldBuyButton.onClick.AddListener(() 
            => ConfirmPurchase(ShopItem.ShieldStarter));
        speedBuyButton.onClick.AddListener(() 
            => ConfirmPurchase(ShopItem.SpeedStarter));
        duoCoinBuyButton.onClick.AddListener(() 
            => ConfirmPurchase(ShopItem.DoubleCoinsStarter));

        // panels v
        iceTubigPanel.SetActive(false);
        shieldPanel.SetActive(false);
        speedPanel.SetActive(false);
        duoCoinPanel.SetActive(false);
    }

    public void ShowPanel(ShopItem item)
    {
        currentItem = item;
        currentAmount = 1;

        iceTubigPanel.SetActive(item == ShopItem.IceTubig);
        shieldPanel.SetActive(item == ShopItem.ShieldStarter);
        speedPanel.SetActive(item == ShopItem.SpeedStarter);
        duoCoinPanel.SetActive(item == ShopItem.DoubleCoinsStarter);

        RefreshUI(item);
    }

    public void ShowIceTubigPanel() 
        => ShowPanel(ShopItem.IceTubig);

    public void ShowShieldPanel() 
        => ShowPanel(ShopItem.ShieldStarter);

    public void ShowSpeedPanel() 
        => ShowPanel(ShopItem.SpeedStarter);

    public void ShowDuoCoinPanel() 
        => ShowPanel(ShopItem.DoubleCoinsStarter);

    void OnAmountChanged(float value, ShopItem item)
    {
        currentAmount = Mathf.RoundToInt(value);
        RefreshUI(item);
    }

    void RefreshUI(ShopItem item)
    {
        int totalPrice = GetUnitPrice(item) * currentAmount;

        switch (item)
        {
            case ShopItem.IceTubig:
                iceTubigTotalPriceText.text = totalPrice.ToString();
                iceTubigOwnedText.text = " " + PlayerInventory.Instance.IceTubigCount;
                iceTubigBuyButton.interactable = CoinStorage.Instance.CanAfford(totalPrice);
                break;

            case ShopItem.ShieldStarter:
                shieldTotalPriceText.text = totalPrice.ToString();
                shieldOwnedText.text = " " + PlayerInventory.Instance.ShieldStarterCount;
                shieldBuyButton.interactable = CoinStorage.Instance.CanAfford(totalPrice);
                break;

            case ShopItem.SpeedStarter:
                speedTotalPriceText.text = totalPrice.ToString();
                speedOwnedText.text = " " + PlayerInventory.Instance.SpeedStarterCount;
                speedBuyButton.interactable = CoinStorage.Instance.CanAfford(totalPrice);
                break;

            case ShopItem.DoubleCoinsStarter:
                duoCoinTotalPriceText.text = totalPrice.ToString();
                duoCoinOwnedText.text = " " + PlayerInventory.Instance.DoubleCoinsStarterCount;
                duoCoinBuyButton.interactable = CoinStorage.Instance.CanAfford(totalPrice);
                break;
        }
    }

    // bili
    int GetUnitPrice(ShopItem item)
    {
        return item switch
        {
            ShopItem.IceTubig => tubigPrice,
            ShopItem.ShieldStarter => shieldPrice,
            ShopItem.SpeedStarter => speedPrice,
            ShopItem.DoubleCoinsStarter => coinsStarterPrice,
            _ => 0
        };
    }

    void ConfirmPurchase(ShopItem item)
    {
        int totalPrice = GetUnitPrice(item) * currentAmount;
        if (!CoinStorage.Instance.CanAfford(totalPrice)) return;

        CoinStorage.Instance.Spend(totalPrice);

        switch (item)
        {
            case ShopItem.IceTubig:
                PlayerInventory.Instance.AddRevive(currentAmount);
                break;
            case ShopItem.ShieldStarter:
                PlayerInventory.Instance.AddShieldStarter(currentAmount);
                break;
            case ShopItem.SpeedStarter:
                PlayerInventory.Instance.AddSpeedStarter(currentAmount);
                break;
            case ShopItem.DoubleCoinsStarter:
                PlayerInventory.Instance.AddDoubleCoinsStarter(currentAmount);
                break;
        }

        RefreshUI(item);
        ShowConfirmation(currentAmount, currentItem.ToString());
    }

    void ShowConfirmation(int amount, string itemName)
    {
        if (confirmationPanel != null)
        {
            confirmationText.text = $"You bought {amount} x {itemName}!";
            confirmationPanel.SetActive(true);
            // optional: hide after 1.5s
            StartCoroutine(HideConfirmationRoutine(1.5f));
        }
    }

    IEnumerator HideConfirmationRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }
}
