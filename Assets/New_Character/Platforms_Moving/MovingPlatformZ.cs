using System.Collections;
using UnityEngine;

public class MovingPlatformZ : MonoBehaviour
{
    public enum ScaleShrinkAxis
    {
        X,
        Z
    }

    [Header("Configuración de la plataforma (offsets en profundidad)")]
    [Tooltip("Posición mínima relativa en z (desde la posición inicial)")]
    [SerializeField] private float minDepth = 0f;

    [Tooltip("Posición máxima relativa en z (desde la posición inicial)")]
    [SerializeField] private float maxDepth = 3f;

    [Tooltip("Velocidad cuando se mueve hacia adelante (rápido) en unidades/seg")]
    [SerializeField] private float forwardSpeed = 4f;

    [Tooltip("Velocidad cuando se mueve hacia atrás (lento) en unidades/seg")]
    [SerializeField] private float backwardSpeed = 1f;

    [Header("Configuración de movimiento")]
    [Tooltip("Dirección inicial hacia donde la plataforma inicia su movimiento")]
    [SerializeField] private bool startMovingForward = true;

    [Tooltip("Tiempo en segundos para retrasar el inicio del movimiento")]
    [SerializeField] private float startDelay = 0f;

    [Header("Configuración del jugador")]
    [Tooltip("Eje de escala que se reducirá cuando el jugador toque la plataforma")]
    [SerializeField] private ScaleShrinkAxis shrinkAxis = ScaleShrinkAxis.X;

    [Tooltip("Factor de reducción de escala")]
    [SerializeField] private float shrinkFactor = 0.5f;

    [Tooltip("Tag del jugador para detección de colisiones")]
    [SerializeField] private string playerTag = "Player";

    private Vector3 initialPosition;
    private float baseZ;
    private float travel; // distancia total (max-min)
    private float currentZ;
    private bool movingForward;
    private float delayTimer;
    // Bandera para evitar llamadas múltiples a la corrutina de aplastamiento
    private bool isCrushing = false; 

    private void Start()
    {
        initialPosition = transform.position;
        baseZ = initialPosition.z + minDepth;
        travel = Mathf.Max(0f, maxDepth - minDepth);
        currentZ = baseZ;
        movingForward = startMovingForward;
        delayTimer = startDelay;
    }

    private void Update()
    {
        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        if (travel <= 0f) return;

        float speedToUse = movingForward ? forwardSpeed : backwardSpeed;

        currentZ += speedToUse * Time.deltaTime * (movingForward ? 1f : -1f);

        if (currentZ >= initialPosition.z + maxDepth)
        {
            currentZ = initialPosition.z + maxDepth;
            movingForward = false;
        }
        else if (currentZ <= initialPosition.z + minDepth)
        {
            currentZ = initialPosition.z + minDepth;
            movingForward = true;
        }

        transform.position = new Vector3(initialPosition.x, initialPosition.y, currentZ);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 p0 = new Vector3(transform.position.x, transform.position.y, (Application.isPlaying ? initialPosition.z : transform.position.z) + minDepth);
        Vector3 p1 = new Vector3(transform.position.x, transform.position.y, (Application.isPlaying ? initialPosition.z : transform.position.z) + maxDepth);
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawWireSphere(p0, 0.15f);
        Gizmos.DrawWireSphere(p1, 0.15f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;
        if (!root.CompareTag(playerTag)) return;

        // Parent player to platform for movement (Solo le decimos que se mueva con nosotros)
        root.SetParent(transform);
    }

    // NUEVA LÓGICA: Comprueba el aplastamiento continuo
    private void OnTriggerStay(Collider other)
    {
        Transform root = other.transform.root;
        if (!root.CompareTag(playerTag)) return;

        // La plataforma está en su posición final de aplastamiento (Max Depth)
        // Puedes cambiar la condición a <= initialPosition.z + minDepth si el aplastamiento es al inicio.
        bool atCrushPosition = currentZ >= initialPosition.z + maxDepth; 

        // Si el jugador está dentro y estamos en la posición de aplastamiento Y la corrutina no está activa
        if (atCrushPosition && !isCrushing)
        {
            LiveSystem liveSystem = root.GetComponent<LiveSystem>();
            if (liveSystem != null)
            {
                StartCoroutine(ShrinkPlayerAndRespawn(root, liveSystem));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Transform root = other.transform.root;
        if (!root.CompareTag(playerTag)) return;

        // Unparent player from platform
        root.SetParent(null);
    }

    private IEnumerator ShrinkPlayerAndRespawn(Transform playerTransform, LiveSystem liveSystem)
    {
        isCrushing = true; // Establecer la bandera para evitar llamadas múltiples
        
        Vector3 originalScale = playerTransform.localScale;
        Vector3 targetScale = originalScale;

        if (shrinkAxis == ScaleShrinkAxis.X)
        {
            targetScale.x *= shrinkFactor;
        }
        else if (shrinkAxis == ScaleShrinkAxis.Z)
        {
            targetScale.z *= shrinkFactor;
        }

        // Smooth scale down over 0.5 seconds
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            playerTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerTransform.localScale = targetScale;

        // Small delay at minimum scale
        yield return new WaitForSeconds(0.5f);

        // Decrement life and respawn player
        liveSystem.SendMessage("DecreaseLifeAndRespawn", SendMessageOptions.DontRequireReceiver);

        // Restore scale smoothly over 0.5 seconds
        elapsed = 0f;
        while (elapsed < duration)
        {
            playerTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerTransform.localScale = originalScale;

        // Unparent player (if respawn does not do it)
        playerTransform.SetParent(null);

        isCrushing = false; // Reset flag
    }
}