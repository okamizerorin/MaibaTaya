using UnityEngine;

public class CoinStorage : MonoBehaviour
{
    public static CoinStorage Instance;

    const string COINS_KEY = "PLAYER_COINS";

    public System.Action<int> OnCoinsChanged;
    public int Coins { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    void Load()
    {
        Coins = PlayerPrefs.GetInt(COINS_KEY, 0);
        OnCoinsChanged?.Invoke(Coins);
    }

    void Save()
    {
        PlayerPrefs.SetInt(COINS_KEY, Coins);
        PlayerPrefs.Save();
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        Save();
        OnCoinsChanged?.Invoke(Coins); 
    }


    public bool CanAfford(int amount)
    {
        return Coins >= amount;
    }

    public void Spend(int amount)
    {
        Coins -= amount;
        Save();
        OnCoinsChanged?.Invoke(Coins); 
    }

}
