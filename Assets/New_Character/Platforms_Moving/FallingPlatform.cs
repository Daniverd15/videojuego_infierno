using UnityEngine;
using System.Collections; // Necesario para las corrutinas

public class FallingPlatform : MonoBehaviour
{
    // --- Ajustes de la Plataforma ---
    
    [Header("Platform Settings")]
    [Tooltip("Tiempo que tarda la plataforma en caer después de que el jugador aterriza.")]
    public float timeUntilDrop = 0.5f;
    
    [Tooltip("Tiempo que tarda la plataforma en volver a subir.")]
    public float timeUntilRespawn = 3f;
    
    [Tooltip("Velocidad de caída de la plataforma.")]
    public float fallSpeed = 20f;
    
    [Tooltip("Posición máxima a la que caerá la plataforma (Distancia en Y desde su posición inicial).")]
    public float maxDropDistance = 10f;

    // --- Ajustes del Temblor ---
    
    [Header("Shake Settings")]
    [Tooltip("Intensidad del temblor (amplitud).")]
    public float shakeIntensity = 0.1f;
    
    [Tooltip("Velocidad del temblor (frecuencia).")]
    public float shakeSpeed = 50f;

    // --- Componentes y Estado Interno ---
    
    private Vector3 initialPosition;
    private Rigidbody rb;
    private bool isPlayerOnPlatform = false;
    private bool isFalling = false;
    private bool dropSequenceRunning = false;
    
    // El script del jugador que contiene SurfaceDetection.
    // **Asegúrate de que este componente esté en el objeto jugador.**
    private SurfaceDetection playerSurfaceDetection;

    void Start()
    {
        // Guardar la posición inicial para el resurgimiento
        initialPosition = transform.position;

        // Añadir Rigidbody si no existe (Necesario para la caída)
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configurar el Rigidbody
        rb.isKinematic = true; // Empieza como Kinematic
        rb.useGravity = false;

        // **NOTA:** Asumiendo que el script SurfaceDetection está en el objeto del jugador.
        // Podrías necesitar un enfoque diferente (como un Trigger) si el jugador no es un solo objeto.
        // Aquí asumimos que el jugador tiene el tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerSurfaceDetection = player.GetComponent<SurfaceDetection>();
            if (playerSurfaceDetection == null)
            {
                Debug.LogError("SurfaceDetection script not found on player.");
            }
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found.");
        }
        
        // **IMPORTANTE:** Asegúrate de que el objeto de la plataforma tenga el Tag **"FallingPlatform"** en el Inspector de Unity.
    }

    void Update()
    {
        // Solo verificamos si el jugador está sobre la plataforma si NO está cayendo
        if (!isFalling && playerSurfaceDetection != null)
        {
            Collider currentSurface = playerSurfaceDetection.GetCurrentSurfaceCollider();
            
            // Comprobamos si el jugador está sobre *este* objeto específico Y si *este* objeto tiene el Tag correcto
            if (currentSurface != null && currentSurface.gameObject == gameObject && currentSurface.CompareTag("FallingPlatform"))
            {
                if (!isPlayerOnPlatform)
                {
                    isPlayerOnPlatform = true;
                    Debug.Log("Player detected on FallingPlatform. Starting drop sequence.");
                    if (!dropSequenceRunning)
                    {
                        StartCoroutine(DropSequence());
                    }
                }
            }
            else
            {
                // El jugador ya no está sobre la plataforma.
                isPlayerOnPlatform = false;
            }
        }
    }

    /// <summary>
    /// Corrutina principal que maneja la secuencia de temblar, caer y resurgir.
    /// </summary>
    IEnumerator DropSequence()
    {
        dropSequenceRunning = true;
        isFalling = true;
        float timer = 0f;
        
        // Temblar por el tiempo de espera
        while (timer < timeUntilDrop)
        {
            // Mover la plataforma usando un seno/coseno para el efecto de temblor
            float x = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * 2f - 1f;
            float z = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * 2f - 1f;
            
            // Aplicar el temblor en el plano horizontal
            transform.position = initialPosition + new Vector3(x, 0f, z) * shakeIntensity;

            timer += Time.deltaTime;
            yield return null; // Esperar un frame
        }

        // 2. Caer
        
        // Restablecer la posición justo antes de caer para eliminar el temblor
        transform.position = initialPosition;
        
        // Habilitar caída manual controlando la velocidad (desactivar gravedad para controlar la caída)
        rb.isKinematic = false;
        rb.useGravity = false; // disable built-in gravity to control velocity manually
        
        // Aplicar la velocidad de caída manualmente
        rb.velocity = Vector3.down * fallSpeed;

        // Bucle para controlar el límite de caída
        while (transform.position.y > initialPosition.y - maxDropDistance)
        {
            yield return null; 
        }

        // 3. Esperar y Resurgir
        
        // Desactivar temporalmente la plataforma después de alcanzar el límite
        rb.velocity = Vector3.zero;
        rb.isKinematic = true; // detener la física para reposicionamiento
        GetComponent<Collider>().enabled = false;
        
        yield return new WaitForSeconds(timeUntilRespawn);

        // 4. Volver a la posición inicial
        
        // Teletransportar a la posición inicial
        transform.position = initialPosition;

        // Restablecer el estado
        GetComponent<Collider>().enabled = true;
        isFalling = false;
        isPlayerOnPlatform = false;
        dropSequenceRunning = false;
    }
}
