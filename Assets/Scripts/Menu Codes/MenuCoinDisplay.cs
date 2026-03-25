using TMPro;
using UnityEngine;

public class MenuCoinsDisplay : MonoBehaviour
{
    public TMP_Text coinsText;

    void OnEnable()
    {
        if (CoinStorage.Instance != null)
        {
            CoinStorage.Instance.OnCoinsChanged += UpdateCoins;
            UpdateCoins(CoinStorage.Instance.Coins); 
        }
    }

    void OnDisable()
    {
        if (CoinStorage.Instance != null)
            CoinStorage.Instance.OnCoinsChanged -= UpdateCoins;
    }

    void UpdateCoins(int coins)
    {
        if (coinsText != null)
            coinsText.text = coins.ToString();
    }
}
