using UnityEngine;

public class HighScoreStorage : MonoBehaviour
{
    public static HighScoreStorage Instance;

    const string HIGH_SCORE_KEY = "HIGH_SCORE";

    public int HighScore { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    public bool TrySetNewScore(int distance)
    {
        if (distance > HighScore)
        {
            HighScore = distance;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, HighScore);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
}
