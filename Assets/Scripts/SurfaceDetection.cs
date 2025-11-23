using UnityEngine;

public class SurfaceDetection : MonoBehaviour
{
    [Tooltip("Distance to check below the player for surface detection")]
    public float detectionDistance = 1.5f;

    private Collider currentSurfaceCollider;
    private LayerMask currentSurfaceLayerMask;
    private string currentSurfaceTag;

    void Update()
    {
        DetectSurface();
    }

    private void DetectSurface()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        if (Physics.Raycast(origin, Vector3.down, out hit, detectionDistance))
        {
            currentSurfaceCollider = hit.collider;
            currentSurfaceLayerMask = 1 << hit.collider.gameObject.layer;
            currentSurfaceTag = hit.collider.gameObject.tag;
            Debug.Log("Current surface tag: " + currentSurfaceTag);
        }
        else
        {
            currentSurfaceCollider = null;
            currentSurfaceLayerMask = 0;
            currentSurfaceTag = null;
            Debug.Log("No surface detected beneath player.");
        }
    }

    /// <summary>
    /// Checks if the player is currently on a surface with given layer mask
    /// </summary>
    public bool IsOnSurfaceLayerMask(LayerMask mask)
    {
        if (currentSurfaceCollider == null) return false;
        return ((1 << currentSurfaceCollider.gameObject.layer) & mask) != 0;
    }

    /// <summary>
    /// Checks if the player is currently on a surface with given tag
    /// </summary>
    public bool IsOnSurfaceTag(string tag)
    {
        if (currentSurfaceTag == null) return false;
        return currentSurfaceTag == tag;
    }

    public Collider GetCurrentSurfaceCollider()
    {
        return currentSurfaceCollider;
    }
}
