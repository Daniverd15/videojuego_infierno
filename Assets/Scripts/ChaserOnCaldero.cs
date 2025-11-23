using UnityEngine;

public class ChaserOnCaldero : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform del jugador a perseguir")]
    public Transform playerTransform;

    private SurfaceDetection surfaceDetection;

    [Header("Configuración de movimiento")]
    [Tooltip("Velocidad de movimiento del perseguidor")]
    public float chaseSpeed = 5f;

    [Tooltip("Velocidad de rotación rápida en X para simular caminar")]
    public float walkRotationSpeed = 1000f;

    [Tooltip("Amplitud de rotación en grados para simular el movimiento de caminar")]
    public float walkRotationAmplitude = 15f;

    [Header("Detección de proximidad")]
    [Tooltip("Radio para detectar proximidad del jugador y activar la persecución")]
    public float chaseRadius = 10f;

    private Rigidbody rb;

    public bool isActive = false;

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("[ChaserOnCaldero] El playerTransform no está asignado.");
            enabled = false;
            return;
        }
        surfaceDetection = playerTransform.GetComponent<SurfaceDetection>();
        if (surfaceDetection == null)
        {
            Debug.LogWarning("[ChaserOnCaldero] No se encontró componente SurfaceDetection en el jugador.");
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("[ChaserOnCaldero] No se encontró Rigidbody en el chaser.");
        }
    }

    void Update()
    {
        if (playerTransform == null || surfaceDetection == null) return;

        // Simplify to check only if player is on "ChaserSurface"
        isActive = surfaceDetection.IsOnSurfaceTag("ChaserSurface");

        if(!isActive)
        {
            // Stop chasing, optionally apply gravity or idle behavior
            return;
        }

        // Move towards player on XZ plane
        Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        if(direction.sqrMagnitude > 0.001f)
        {
            // Move chaser
            transform.position += direction * chaseSpeed * Time.deltaTime;

            // Rotate to face player on Y axis
            Vector3 lookDirection = (targetPosition - transform.position).normalized;
            if(lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                Vector3 currentEuler = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(currentEuler.x, targetRotation.eulerAngles.y, currentEuler.z);
            }

            // Oscillate rotation on X to simulate walking
            float rotationX = Mathf.Sin(Time.time * walkRotationSpeed) * walkRotationAmplitude;
            Vector3 eulerAngles = transform.localEulerAngles;
            if (eulerAngles.x > 180) eulerAngles.x -= 360; // Normalize angle
            eulerAngles.x = rotationX;
            transform.localEulerAngles = eulerAngles;
        }
    }
}
