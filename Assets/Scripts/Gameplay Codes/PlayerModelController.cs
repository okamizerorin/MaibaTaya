using UnityEngine;
using System.Collections.Generic;

public class PlayerVisualController : MonoBehaviour
{
    public GameObject[] characterModels;

    void Start()
    {
        EquipSelectedCharacter();
    }

    void EquipSelectedCharacter()
    {
        int index = 0;

        if (CharacterSystem.Instance != null)
            index = CharacterSystem.Instance.selectedCharacterIndex;

        index = Mathf.Clamp(index, 0, characterModels.Length - 1);

        foreach (GameObject model in characterModels)
            model.SetActive(false);

        characterModels[index].SetActive(true);

        Debug.Log("Gameplay character activated: " + index);
    }
}
