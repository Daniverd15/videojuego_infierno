using UnityEngine;

public class LavaScroller : MonoBehaviour
{
    [Header("Velocidad de desplazamiento EXTREMADAMENTE LENTA")]
    [Tooltip("Velocidad en el eje U (X)")]
    public float scrollSpeedU = 0.00005f;   // SUPER LENTO

    [Tooltip("Velocidad en el eje V (Y)")]
    public float scrollSpeedV = 0.00002f;   // ULTRA LENTO

    private Renderer rend;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
        offset = Vector2.zero;
    }

    void Update()
    {
        // Movimiento casi imperceptible
        offset.x += scrollSpeedU * Time.deltaTime;
        offset.y += scrollSpeedV * Time.deltaTime;

        if (rend != null)
        {
            rend.material.SetTextureOffset("_MainTex", offset);

            if (rend.material.HasProperty("_EmissionMap"))
            {
                rend.material.SetTextureOffset("_EmissionMap", offset);
            }
        }
    }
}

