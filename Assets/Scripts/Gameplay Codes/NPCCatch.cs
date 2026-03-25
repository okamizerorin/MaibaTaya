using UnityEngine;

public class NPCCatch : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RevivalSystem.Instance.OnNPCCaught();

            // add sfx?
        }
    }
}
