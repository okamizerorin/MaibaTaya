using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ObstaclesCollision : MonoBehaviour
{
    public PlayerMovement movement;
    private GamePowerups powerups;
    private bool isFailed;

    void Awake()
    {
        powerups = movement.GetComponent<GamePowerups>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (isFailed) return;

        if (powerups != null && powerups.isShieldActive)
            return;

        if (other.CompareTag("CollisionSoft"))
        {
            // the stagger
            if (RevivalSystem.Instance != null)
                RevivalSystem.Instance.StaggerPlayer();
        }
        else if (other.CompareTag("CollisionHard"))
        {
            Fail();
        }
    }

    void Fail()
    {
        isFailed = true;
        movement.Freeze();

        if (RevivalSystem.Instance != null)
        {
            RevivalSystem.Instance.PlayFailAnimation();
            RevivalSystem.Instance.OnPlayerFailed();
        }
    }

    public void ResetFailState() 
    { 
        isFailed = false; 
    }
}