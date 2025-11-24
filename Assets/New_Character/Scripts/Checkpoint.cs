using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            LiveSystem liveSystem = other.GetComponent<LiveSystem>();
            if (liveSystem != null)
            {
                liveSystem.respawnPoint = this.transform;
                Debug.Log("Checkpoint activated at position: " + transform.position);
                isActive = true;
                // Optional: Add visual or audio feedback for checkpoint activation here
            }
            else
            {
                Debug.LogWarning("Player does not have a LiveSystem component.");
            }
        }
    }
}
