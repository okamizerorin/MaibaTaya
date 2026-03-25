using UnityEngine;

public class ObstacleDespawner : MonoBehaviour
{
    public float despawnDistance = 15f;

    private Transform player;

    void OnEnable()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) 
            return;

        if (transform.position.z < player.position.z - despawnDistance)
        {
            Despawn();
        }
    }

    void Despawn()
    {
        transform.SetParent(null);

        if (ObstaclePooler.Instance != null)
        {
            ObstaclePooler.Instance.ReturnToPool(this.gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}