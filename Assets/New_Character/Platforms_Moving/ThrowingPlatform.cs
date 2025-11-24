using UnityEngine;

public class ThrowingPlatform : MonoBehaviour
{
    [Tooltip("Launch velocity applied to the player when standing on this platform.")]
    public Vector3 launchVelocity = new Vector3(0f, 15f, 5f);

    private New_CharacterController playerController;
    private bool hasLaunchedPlayer = false;

    private void Awake()
    {
        // Ensure the platform has the correct tag
        gameObject.tag = "ThrowingPlatform";
    }

private bool isPlayerOnPlatform = false;

private void Update()
{
    if (playerController == null)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<New_CharacterController>();
        }
    }

    if (playerController != null)
    {
        var surfaceDetection = playerController.surfaceDetection;
        if (surfaceDetection != null)
        {
            Collider currentSurface = surfaceDetection.GetCurrentSurfaceCollider();

            if (currentSurface != null && currentSurface.gameObject == gameObject && currentSurface.CompareTag("ThrowingPlatform"))
            {
                if (!isPlayerOnPlatform)
                {
                    isPlayerOnPlatform = true;
                    if (!hasLaunchedPlayer)
                    {
                        Vector3 forward = transform.forward.normalized;
                        Vector3 velocity = new Vector3(forward.x * launchVelocity.z, launchVelocity.y, forward.z * launchVelocity.z);
                        playerController.LaunchPlayer(velocity);
                        hasLaunchedPlayer = true;
                    }
                }
            }
            else
            {
                isPlayerOnPlatform = false;
                hasLaunchedPlayer = false;
            }
        }
    }
}
}
