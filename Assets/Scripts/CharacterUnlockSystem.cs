using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CharacterUnlockSystem : MonoBehaviour
{
    public static CharacterUnlockSystem Instance;

    const string UNLOCK_KEY = "CHAR_UNLOCK_";

    [Header("Unlock Popup UI")]
    public GameObject unlockPanel;
    public Transform previewPoint;
    public List<GameObject> characterPrefabs;
    public TextMeshProUGUI unlockNameText;
    public Button nextButton; // button to advance to next unlock

    GameObject currentPreview;
    Queue<int> unlockQueue = new Queue<int>();

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

    public bool IsUnlocked(int index)
    {
        return PlayerPrefs.GetInt(UNLOCK_KEY + index, index == 0 ? 1 : 0) == 1;
    }

    public void UnlockCharacter(int index)
    {
        PlayerPrefs.SetInt(UNLOCK_KEY + index, 1);
        PlayerPrefs.Save();
    }

    public List<int> CheckForUnlocks()
    {
        List<int> newlyUnlocked = new List<int>();
        if (CharacterDatabase.Instance == null) return newlyUnlocked;

        int bestDistance = HighScoreStorage.Instance != null ? HighScoreStorage.Instance.HighScore : 0;
        int totalCoins = CoinStorage.Instance != null ? CoinStorage.Instance.Coins : 0;

        var dataList = CharacterDatabase.Instance.unlockData;

        for (int i = 0; i < dataList.Count; i++)
        {
            if (IsUnlocked(i)) continue;

            var data = dataList[i];

            if (data.unlockedByDefault ||
                (bestDistance >= data.requiredDistance && totalCoins >= data.requiredTotalCoins))
            {
                UnlockCharacter(i);
                newlyUnlocked.Add(i);
            }
        }

        if (newlyUnlocked.Count > 0)
            ShowUnlocksSequentially(newlyUnlocked);

        return newlyUnlocked;
    }

    void ShowUnlocksSequentially(List<int> newUnlocks)
    {
        unlockQueue.Clear();
        foreach (var i in newUnlocks)
            unlockQueue.Enqueue(i);

        ShowNextUnlock();
    }

    void ShowNextUnlock()
    {
        if (unlockQueue.Count == 0)
        {
            ClosePopup();
            return;
        }

        int index = unlockQueue.Dequeue();
        ShowUnlockPopup(index);

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(ShowNextUnlock);
        }
    }

    public void ShowUnlockPopup(int index)
    {
        if (unlockPanel == null || previewPoint == null || CharacterDatabase.Instance == null) return;

        unlockPanel.SetActive(true);

        if (currentPreview != null)
            Destroy(currentPreview);

        currentPreview = Instantiate(
            characterPrefabs[index],
            previewPoint.position,
            previewPoint.rotation
        );
        currentPreview.transform.SetParent(previewPoint);

        if (unlockNameText != null)
            unlockNameText.text = CharacterDatabase.Instance.unlockData[index].characterName;
    }

    public void ClosePopup()
    {
        unlockPanel.SetActive(false);
        if (currentPreview != null)
            Destroy(currentPreview);
    }
}