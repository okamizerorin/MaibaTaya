using UnityEngine;
using System.Collections.Generic;

public class CharacterDatabase : MonoBehaviour
{
    public static CharacterDatabase Instance;

    [System.Serializable]
    public class CharacterUnlockData
    {
        public string characterName;
        public bool unlockedByDefault;
        public int requiredDistance;
        public int requiredTotalCoins;
    }

    public List<CharacterUnlockData> unlockData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}