using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICharacEquip : MonoBehaviour
{
    public Button equipButton;
    public int characterIndex;

    void Update()
    {
        int equipped = CharacterSystem.Instance.selectedCharacterIndex;

        bool isEquipped = equipped == characterIndex;

        equipButton.interactable = !isEquipped;

    }
}

