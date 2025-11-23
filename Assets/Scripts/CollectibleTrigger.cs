using UnityEngine;

public class CollectibleTrigger : MonoBehaviour
{
    [Tooltip("Referencia al script del caldero perseguidor")]
    public ChaserOnCaldero chaser;

    [Tooltip("Tag que identifica al jugador")]
    public string playerTag = "Player";

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!activated && other.CompareTag(playerTag))
        {
            activated = true;
            if (chaser != null)
            {
                chaser.isActive = true;
                Debug.Log("Chaser activated by collectible trigger.");
            }
            else
            {
                Debug.LogWarning("Chaser reference not assigned in CollectibleTrigger.");
            }

            // Optionally, disable or destroy this collectible after activation
            gameObject.SetActive(false);
        }
    }
}
