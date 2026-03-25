using TMPro;
using UnityEngine;

public class HighScoreUI : MonoBehaviour
{
    public TMP_Text highScoreText;

    void Start()
    {
        if (HighScoreStorage.Instance != null)
        {
            highScoreText.text = HighScoreStorage.Instance.HighScore.ToString();
        }
    }
}