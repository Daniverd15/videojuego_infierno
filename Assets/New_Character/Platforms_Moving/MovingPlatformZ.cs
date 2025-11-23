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
}
