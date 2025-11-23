using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PortalInteract : MonoBehaviour
{
    // ❌ Eliminamos la variable de instancia, ya no es necesaria:
    // private CollectibleManagerS collectibleManager; 
    private bool playerInRange = false;
    public TextMeshProUGUI portalMessageText; 
    private bool messageShown = false;

    void Start()
    {
        // ❌ Eliminamos la búsqueda FindObjectOfType ya que usaremos el método estático
        // if (collectibleManager == null)
        // {
        //     Debug.LogError("CollectibleManagerS not found in the scene.");
        // }

        if (portalMessageText != null)
            portalMessageText.text = "";
        else
            Debug.LogWarning("portalMessageText is not assigned in the inspector.");
    }

    // ❌ ELIMINAMOS LA LÓGICA DE INTERACCIÓN CON LA TECLA 'E' EN UPDATE
    void Update()
    {
        // ...
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter: Portal tag is '{gameObject.tag}', other tag is '{other.tag}'");

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered portal range.");

            // 🔑 NUEVA COMPROBACIÓN: Llama directamente al método estático de la clase
            bool crossesComplete = CollectibleManagerS.CrossesComplete();
            Debug.Log($"OnTriggerEnter: CollectibleManagerS.CrossesComplete() returned: {crossesComplete}");

            if (crossesComplete)
            {
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 3;
                int sceneCount = SceneManager.sceneCountInBuildSettings;
                if (nextSceneIndex < sceneCount)
                {
                    Debug.Log($"Cruces completas. Cargando siguiente escena con índice: {nextSceneIndex}");
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    Debug.LogWarning($"El índice de escena a cargar ({nextSceneIndex}) excede el número de escenas en Build Settings ({sceneCount}). No se carga ninguna escena.");
                    ShowPortalMessage("Error: La siguiente escena no está configurada.");
                }
            }
            else
            {
                Debug.Log("No se tienen todas las cruces. Mostrando mensaje.");
                ShowPortalMessage("No se tienen todas las 4 cruces. No puedes usar el portal.");
            }
            
            // ❌ Eliminamos el bloque 'else' que causaba el error de verificación fallida
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"OnTriggerExit: Portal tag is '{gameObject.tag}', other tag is '{other.tag}'");
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            ClearPortalMessage();
        }
    }

    private void ShowPortalMessage(string message)
    {
        if (portalMessageText != null)
        {
            portalMessageText.text = message;
            messageShown = true;
            Debug.Log($"Mensaje portal mostrado: {message}");
        }
        else
        {
            Debug.LogWarning($"Intento de mostrar mensaje '{message}' fallido: portalMessageText es null.");
        }
    }

    private void ClearPortalMessage()
    {
        if (portalMessageText != null)
        {
            portalMessageText.text = "";
            messageShown = false;
            Debug.Log("Mensaje portal limpiado.");
        }
    }
}