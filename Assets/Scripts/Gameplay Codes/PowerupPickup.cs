using UnityEngine;

public enum PowerupType
{
    HighJump,
    DoubleSpeed,
    DoubleCoins,
    Shield
}

public class PowerupPickup : MonoBehaviour
{
    public PowerupType powerupType;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GamePowerups powerups = other.GetComponent<GamePowerups>();
        if (powerups == null) return;

        switch (powerupType)
        {
            case PowerupType.HighJump:
                powerups.ActivateHighJump();
                MusicBGManager.Instance.PlayPowerup();
                break;

            case PowerupType.DoubleSpeed:
                powerups.ActivateDoubleSpeed();
                MusicBGManager.Instance.PlayPowerup();
                break;

            case PowerupType.DoubleCoins:
                powerups.ActivateDoubleCoins();
                MusicBGManager.Instance.PlayPowerup();
                break;

            case PowerupType.Shield:
                powerups.ActivateShield();
                MusicBGManager.Instance.PlayPowerup();
                break;
        }

        Destroy(gameObject);
    }
}
