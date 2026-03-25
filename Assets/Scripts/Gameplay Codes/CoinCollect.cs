using UnityEngine;
using TMPro;

public class CoinCollect : MonoBehaviour
{
    public static CoinCollect Instance;

    TMP_Text text;
    public int coins; // curernt coins

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        UpdateText();

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        UpdateText();
    }

    void UpdateText()
    {
        if (text != null)
            text.text = coins.ToString();
    }

    public void ResetCoins()
    {
        coins = 0;
        UpdateText();
    }

    public void SaveToTotal()
    {
        CoinStorage.Instance.AddCoins(coins); 
        coins = 0;
        UpdateText();
    }
}
