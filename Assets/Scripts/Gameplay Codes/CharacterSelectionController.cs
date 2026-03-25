using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CharacterSelectionController : MonoBehaviour
{
    public List<GameObject> characterPrefabs;
    public Transform selectionPoint;

    [Header("Character Voices")]
    public List<AudioClip> equipVoices;
    public AudioSource sfxSource;

    public Camera previewCamera;
    public GameObject panel;

    private GameObject currentCharacter;
    public int currentIndex;

    public GameObject equipButton;
    public GameObject equippedLabel;

    [Header("Unlock UI")]
    public TextMeshProUGUI requirementText;

    void ShowCharacter(int index)
    {
        if (currentCharacter != null)
            Destroy(currentCharacter);

        currentIndex = index;

        currentCharacter = Instantiate(
            characterPrefabs[currentIndex],
            selectionPoint.position,
            selectionPoint.rotation
        );

        currentCharacter.transform.SetParent(selectionPoint);

        int previewLayer = LayerMask.NameToLayer("CharacterPreview");

        currentCharacter.layer = previewLayer;
        foreach (Transform child in currentCharacter.transform)
            child.gameObject.layer = previewLayer;

        UpdateUI();
    }

    public void NextCharacter()
    {
        ShowCharacter((currentIndex + 1) % characterPrefabs.Count);
    }

    public void PreviousCharacter()
    {
        ShowCharacter((currentIndex - 1 + characterPrefabs.Count) % characterPrefabs.Count);
    }

    public void OpenPanel()
    {
        panel.SetActive(true);

        if (previewCamera != null)
            previewCamera.gameObject.SetActive(true);

        currentIndex = CharacterSystem.Instance.selectedCharacterIndex;

        ShowCharacter(currentIndex);
        RefreshUI();
    }

    public void ClosePanel()
    {
        panel.SetActive(false);

        if (previewCamera != null)
            previewCamera.gameObject.SetActive(false);

        if (currentCharacter != null)
            Destroy(currentCharacter);
    }

    public bool IsCharacterUnlocked(int index)
    {
        return CharacterUnlockSystem.Instance == null ||
               CharacterUnlockSystem.Instance.IsUnlocked(index);
    }

    void UpdateUI()
    {
        bool isUnlocked = IsCharacterUnlocked(currentIndex);
        bool isEquipped = CharacterSystem.Instance.selectedCharacterIndex == currentIndex;

        equipButton.SetActive(isUnlocked && !isEquipped);
        equippedLabel.SetActive(isUnlocked && isEquipped);

        if (!isUnlocked)
        {
            var data = CharacterDatabase.Instance.unlockData[currentIndex];

            requirementText.gameObject.SetActive(true);
            requirementText.text =
                $"Reach {data.requiredDistance}m\n" +
                $"Collect {data.requiredTotalCoins} coins";
        }
        else
        {
            requirementText.gameObject.SetActive(false);
        }
    }

    public void RefreshUI()
    {
        UpdateUI();
    }
}