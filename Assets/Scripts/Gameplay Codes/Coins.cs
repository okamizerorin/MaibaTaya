using UnityEngine;

public class Coins : MonoBehaviour
{
    public float spinSpeed = 180f; // degrees per sec
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GamePowerups powerups = other.GetComponent<GamePowerups>();

        int finalValue = value;
        if (powerups != null && powerups.isDoubleCoinsActive)
            finalValue *= 2;

        CoinCollect.Instance.AddCoin(finalValue);
        MissionSystem.Instance.AddProgress(MissionType.Coins, finalValue);
        MusicBGManager.Instance.PlayCoin();

        Destroy(gameObject);
    }

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.Self);
    }
}
