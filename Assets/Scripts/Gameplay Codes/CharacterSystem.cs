using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSystem : MonoBehaviour
{
    public static CharacterSystem Instance;

    public int selectedCharacterIndex;
    public CharacterSelectionController preview;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            selectedCharacterIndex = PlayerPrefs.GetInt("EquippedCharacter", 0);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        preview = FindObjectOfType<CharacterSelectionController>();

        if (preview != null)
        {
            preview.currentIndex = selectedCharacterIndex;
            preview.RefreshUI();
        }
    }

    public void EquipCharacter()
    {
        if (preview == null) return;

        int index = preview.currentIndex;

        if (!preview.IsCharacterUnlocked(index))
        {
            Debug.Log("Character locked!");
            return;
        }

        selectedCharacterIndex = index;

        PlayerPrefs.SetInt("EquippedCharacter", selectedCharacterIndex);
        PlayerPrefs.Save();

        // always refresh preview panel
        if (preview != null)
            preview.RefreshUI();

        if (preview.sfxSource != null && preview.equipVoices.Count > index)
        {
            AudioClip clip = preview.equipVoices[index];
            if (clip != null)
                preview.sfxSource.PlayOneShot(clip);
        }
    }
}