using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    const string REVIVE_KEY = "INV_REVIVE";
    const string SHIELD_KEY = "INV_STARTER_SHIELD";
    const string SPEED_KEY = "INV_STARTER_SPEED";
    const string COINS_KEY = "INV_STARTER_COINS";

    public int IceTubigCount { get; private set; }
    public int ShieldStarterCount { get; private set; }
    public int SpeedStarterCount { get; private set; }
    public int DoubleCoinsStarterCount { get; private set; }

    public System.Action<int> OnIceTubigChanged;

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
        IceTubigCount = PlayerPrefs.GetInt(REVIVE_KEY, 0);
        ShieldStarterCount = PlayerPrefs.GetInt(SHIELD_KEY, 0);
        SpeedStarterCount = PlayerPrefs.GetInt(SPEED_KEY, 0);
        DoubleCoinsStarterCount = PlayerPrefs.GetInt(COINS_KEY, 0);

        OnIceTubigChanged?.Invoke(IceTubigCount);
    }

    void Save()
    {
        PlayerPrefs.SetInt(REVIVE_KEY, IceTubigCount);
        PlayerPrefs.SetInt(SHIELD_KEY, ShieldStarterCount);
        PlayerPrefs.SetInt(SPEED_KEY, SpeedStarterCount);
        PlayerPrefs.SetInt(COINS_KEY, DoubleCoinsStarterCount);
        PlayerPrefs.Save();
    }

    // add
    public void AddRevive(int amount)
    {
        IceTubigCount += amount;
        Save();
        OnIceTubigChanged?.Invoke(IceTubigCount);
    }


    public void AddShieldStarter(int amount)
    {
        ShieldStarterCount += amount;
        Save();
    }

    public void AddSpeedStarter(int amount)
    {
        SpeedStarterCount += amount;
        Save();
    }

    public void AddDoubleCoinsStarter(int amount)
    {
        DoubleCoinsStarterCount += amount;
        Save();
    }


    // use
    public bool UseRevive()
    {
        if (IceTubigCount <= 0) return false;

        IceTubigCount--;
        Save();
        OnIceTubigChanged?.Invoke(IceTubigCount);
        return true;
    }

    public bool UseShieldStarter()
    {
        if (ShieldStarterCount <= 0) return false;
        ShieldStarterCount--;
        Save();
        return true;
    }

    public bool UseSpeedStarter()
    {
        if (SpeedStarterCount <= 0) return false;
        SpeedStarterCount--;
        Save();
        return true;
    }

    public bool UseDoubleCoinsStarter()
    {
        if (DoubleCoinsStarterCount <= 0) return false;

        DoubleCoinsStarterCount--;
        Save();
        return true;
    }
}
