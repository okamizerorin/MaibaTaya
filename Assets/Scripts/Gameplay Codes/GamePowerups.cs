using UnityEngine;
using System.Collections;

public class GamePowerups : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement movement;
    public PowerupUIManager uiManager;

    private int playerLayer;
    private int obstacleLayer;

    [Header("High Jump")]
    public float highJumpMultiplier = 1.8f;
    public float highJumpDuration = 5f;
    private Coroutine highJumpRoutine;

    [Header("Double Speed")]
    public float speedMultiplier = 2f;
    public float speedDuration = 5f;
    private Coroutine speedRoutine;
    public bool isSpeedBoostActive { get; private set; }

    [Header("Double Coins")]
    public float doubleCoinsDuration = 5f;
    private Coroutine doubleCoinsRoutine;
    public bool isDoubleCoinsActive { get; private set; }

    [Header("Shield")]
    public float shieldDuration = 5f;
    public GameObject shieldVisual;

    private Coroutine shieldRoutine;
    private Coroutine revivalRoutine;
    public bool isShieldActive { get; private set; }

    void Awake()
    {
        if (movement == null)
            movement = GetComponent<PlayerMovement>();

        playerLayer = LayerMask.NameToLayer("Player");
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
    }

    // jumpp
    public void ActivateHighJump()
    {
        if (highJumpRoutine != null)
            StopCoroutine(highJumpRoutine);

        highJumpRoutine = StartCoroutine(HighJumpRoutine());

        uiManager?.Show(PowerupType.HighJump, highJumpDuration);
    }

    IEnumerator HighJumpRoutine()
    {
        float originalJump = movement.jumpHeight;
        movement.jumpHeight *= highJumpMultiplier;

        yield return new WaitForSeconds(highJumpDuration);

        movement.jumpHeight = originalJump;
        highJumpRoutine = null;
    }

    // speedup
    public void ActivateDoubleSpeed()
    {
        if (speedRoutine != null)
            StopCoroutine(speedRoutine);

        speedRoutine = StartCoroutine(SpeedRoutine());

        uiManager?.Show(PowerupType.DoubleSpeed, speedDuration);
    }

    IEnumerator SpeedRoutine()
    {
        isSpeedBoostActive = true;

        yield return new WaitForSeconds(speedDuration);

        isSpeedBoostActive = false;
        speedRoutine = null;
    }

    // double barya
    public void ActivateDoubleCoins()
    {
        if (doubleCoinsRoutine != null)
            StopCoroutine(doubleCoinsRoutine);

        doubleCoinsRoutine = StartCoroutine(DoubleCoinsRoutine());

        uiManager?.Show(PowerupType.DoubleCoins, doubleCoinsDuration);
    }

    IEnumerator DoubleCoinsRoutine()
    {
        isDoubleCoinsActive = true;

        yield return new WaitForSeconds(doubleCoinsDuration);

        isDoubleCoinsActive = false;
        doubleCoinsRoutine = null;
    }

    // shieldo
    public void ActivateShield()
    {
        if (shieldRoutine != null)
            StopCoroutine(shieldRoutine);

        IgnoreObstacleCollision(true);

        shieldRoutine = StartCoroutine(ShieldRoutine());

        uiManager?.Show(PowerupType.Shield, shieldDuration);
    }

    IEnumerator ShieldRoutine()
    {
        isShieldActive = true;

        if (shieldVisual != null)
            shieldVisual.SetActive(true);

        yield return new WaitForSeconds(shieldDuration);

        isShieldActive = false;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        IgnoreObstacleCollision(false);
        shieldRoutine = null;
    }

    void IgnoreObstacleCollision(bool ignore)
    {
        Physics.IgnoreLayerCollision(playerLayer, obstacleLayer, ignore);
    }

    // invincibility s
    public void ActivateInvincibility(float duration)
    {
        if (revivalRoutine != null) return;

        revivalRoutine = StartCoroutine(TemporaryInvincibility(duration));
    }

    IEnumerator TemporaryInvincibility(float duration)
    {
        IgnoreObstacleCollision(true);

        float elapsed = 0;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        while (elapsed < duration)
        {
            foreach (Renderer r in renderers)
            {
                if (r != null) r.enabled = !r.enabled;
            }

            yield return new WaitForSecondsRealtime(0.1f);
            elapsed += 0.1f;
        }

        foreach (Renderer r in renderers)
        {
            if (r != null) r.enabled = true;
        }

        if (!isShieldActive)
            IgnoreObstacleCollision(false);

        revivalRoutine = null;
    }

    public bool IsInvincible()
    {
        return isShieldActive || revivalRoutine != null;
    }

    // reset
    public void ResetAllPowerups()
    {
        if (highJumpRoutine != null) StopCoroutine(highJumpRoutine);
        if (speedRoutine != null) StopCoroutine(speedRoutine);
        if (doubleCoinsRoutine != null) StopCoroutine(doubleCoinsRoutine);
        if (shieldRoutine != null) StopCoroutine(shieldRoutine);
        if (revivalRoutine != null) StopCoroutine(revivalRoutine);

        highJumpRoutine = null;
        speedRoutine = null;
        doubleCoinsRoutine = null;
        shieldRoutine = null;
        revivalRoutine = null;

        isSpeedBoostActive = false;
        isDoubleCoinsActive = false;
        isShieldActive = false;

        movement.jumpHeight = 7f;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        IgnoreObstacleCollision(false);
    }
}