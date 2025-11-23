using UnityEngine;

public class MovingPlatformHorizontal : MonoBehaviour
{
    [Header("Configuración de la plataforma (offsets horizontales)")]
    [Tooltip("Posición mínima relativa en x (desde la posición inicial)")]
    [SerializeField] private float minWidth = 0f;

    [Tooltip("Posición máxima relativa en x (desde la posición inicial)")]
    [SerializeField] private float maxWidth = 3f;

    [Tooltip("Velocidad en unidades/seg")]
    [SerializeField] private float speed = 2f;

    private Vector3 initialPosition;
    private float baseX;
    private float travel; // distancia total (max-min)

    void Start()
    {
        initialPosition = transform.position;
        // Línea base: x inicial + minOffset
        baseX = initialPosition.x + minWidth;
        // Recorrido total
        travel = Mathf.Max(0f, maxWidth - minWidth);
    }

    // Usa Update cuando mueves transform directamente (más suave)
    void Update()
    {
        if (travel <= 0f) return;

        // PingPong recorre [0, travel] y vuelve sin "clavar"
        float x = baseX + Mathf.PingPong(Time.time * speed, travel);
        transform.position = new Vector3(x, initialPosition.y, initialPosition.z);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 p0 = new Vector3((Application.isPlaying ? initialPosition.x : transform.position.x) + minWidth, transform.position.y, transform.position.z);
        Vector3 p1 = new Vector3((Application.isPlaying ? initialPosition.x : transform.position.x) + maxWidth, transform.position.y, transform.position.z);
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
