using UnityEngine;
using System.Collections.Generic;

public class CameraObstacleFade : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement player;
    public GamePowerups powerups;

    [Header("Fade Settings")]
    [Range(0f, 1f)] public float fadeAlpha = 0.3f; // transparency level
    public LayerMask obstacleLayer;

    // Track obstacles that are currently faded
    private Dictionary<Renderer, Material[]> fadedObstacles = new();

    void LateUpdate()
    {
        if (player == null || powerups == null) return;

        // Only fade obstacles during invincibility/shield
        if (!powerups.IsInvincible())
        {
            RestoreAllObstacles();
            return;
        }


        // Raycast from camera to player to check obstacles
        Vector3 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, obstacleLayer);

        HashSet<Renderer> currentHits = new();

        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend == null) continue;

            currentHits.Add(rend);

            if (!fadedObstacles.ContainsKey(rend))
                FadeRenderer(rend);
        }

        // Restore obstacles that are no longer in the raycast
        List<Renderer> toRestore = new();
        foreach (var rend in fadedObstacles.Keys)
            if (!currentHits.Contains(rend)) toRestore.Add(rend);

        foreach (var rend in toRestore)
            RestoreRenderer(rend);
    }

    void FadeRenderer(Renderer rend)
    {
        // Save original materials
        fadedObstacles[rend] = rend.materials;

        Material[] mats = new Material[rend.materials.Length];
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = new Material(rend.materials[i]);
            Color c = mats[i].color;
            c.a = fadeAlpha;
            mats[i].color = c;

            // Setup for transparency
            mats[i].SetFloat("_Mode", 2); // Transparent
            mats[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mats[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mats[i].SetInt("_ZWrite", 0);
            mats[i].DisableKeyword("_ALPHATEST_ON");
            mats[i].EnableKeyword("_ALPHABLEND_ON");
            mats[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mats[i].renderQueue = 3000;
        }

        rend.materials = mats;
    }

    void RestoreRenderer(Renderer rend)
    {
        if (!fadedObstacles.ContainsKey(rend)) return;

        rend.materials = fadedObstacles[rend];
        fadedObstacles.Remove(rend);
    }

    void RestoreAllObstacles()
    {
        foreach (var rend in new List<Renderer>(fadedObstacles.Keys))
            RestoreRenderer(rend);
    }
}

