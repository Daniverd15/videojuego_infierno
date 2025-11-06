using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Configuración de la plataforma (offsets verticales)")]
    [Tooltip("Altura mínima relativa (desde la posición inicial)")]
    [SerializeField] private float minHeight = 0f;

    [Tooltip("Altura máxima relativa (desde la posición inicial)")]
    [SerializeField] private float maxHeight = 3f;

    [Tooltip("Velocidad en unidades/seg")]
    [SerializeField] private float speed = 2f;

    private Vector3 initialPosition;
    private float baseY;
    private float travel; // distancia total (max-min)

    void Start()
    {
        initialPosition = transform.position;
        // Línea base: y inicial + minOffset
        baseY = initialPosition.y + minHeight;
        // Recorrido total
        travel = Mathf.Max(0f, maxHeight - minHeight);
    }

    // Usa Update cuando mueves transform directamente (más suave)
    void Update()
    {
        if (travel <= 0f) return;

        // PingPong recorre [0, travel] y vuelve sin "clavar"
        float y = baseY + Mathf.PingPong(Time.time * speed, travel);
        transform.position = new Vector3(initialPosition.x, y, initialPosition.z);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 p0 = new Vector3(transform.position.x, (Application.isPlaying ? initialPosition.y : transform.position.y) + minHeight, transform.position.z);
        Vector3 p1 = new Vector3(transform.position.x, (Application.isPlaying ? initialPosition.y : transform.position.y) + maxHeight, transform.position.z);
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawWireSphere(p0, 0.15f);
        Gizmos.DrawWireSphere(p1, 0.15f);
    }

    // Parenting clásico (igual que tu enfoque original)
    void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;
        if (!root.CompareTag("Player")) return;
        root.SetParent(transform);
    }

    void OnTriggerExit(Collider other)
    {
        Transform root = other.transform.root;
        if (!root.CompareTag("Player")) return;
        root.SetParent(null);
    }
}