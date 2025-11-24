using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PortalInfierno : MonoBehaviour
{
    private bool playerInRange = false;
    public TextMeshProUGUI portalMessageText;
    private bool messageShown = false;

    // CAMBIO CLAVE: Usamos Vector3 en lugar de Transform para evitar referencias cross-scene perdidas.
    [Header("Spawn al destino")]
    [Tooltip("Las coordenadas exactas de la posición de spawn en la escena de destino (Infierno).")]
    public Vector3 targetSpawnPosition;


    void Start()
    {
        if (portalMessageText != null)
            portalMessageText.text = "";
        else
            Debug.LogWarning("portalMessageText is not assigned in the inspector.");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter: PortalInfierno tag is '{gameObject.tag}', other tag is '{other.tag}'");

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered PortalInfierno range.");

            // Check if rosarios are collected
            // Asumo que CollectibleManagerS.RosariosComplete() existe
            bool rosariosComplete = CollectibleManagerS.RosariosComplete(); 
            Debug.Log($"OnTriggerEnter: CollectibleManagerS.RosariosComplete() returned: {rosariosComplete}");

            if (rosariosComplete)
            {
                // Ajusta el índice de escena según sea necesario.
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex +1; 

                int sceneCount = SceneManager.sceneCountInBuildSettings;
                if (nextSceneIndex < sceneCount)
                {
                    Debug.Log($"Rosarios completas. Setting spawn point and loading next scene with index: {nextSceneIndex}");


                    // Lógica modificada: Usamos la posición Vector3 guardada.
                    if (SpawnManager.Instance != null)
                    {
                        // En lugar de newSpawnPoint.position, usamos targetSpawnPosition
                        SpawnManager.Instance.SetNextSpawnPoint(targetSpawnPosition);
                        Debug.Log($"SpawnManager.Instance.SetNextSpawnPoint called with: {targetSpawnPosition}");
                    }
                    else
                    {
                        Debug.LogWarning("SpawnManager.Instance is null. Make sure a SpawnManager is in the scene.");
                    }

                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    Debug.LogWarning($"Scene index to load ({nextSceneIndex}) exceeds number of scenes in Build Settings ({sceneCount}). No scene loaded.");
                    ShowPortalMessage("Error: The next scene is not configured.");
                }
            }
            else
            {
                Debug.Log("Not enough rosarios collected. Showing message.");
                ShowPortalMessage("No tienes suficientes 2 rosarios. No puedes usar el portal.");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"OnTriggerExit: PortalInfierno tag is '{gameObject.tag}', other tag is '{other.tag}'");
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
            Debug.Log($"PortalInfierno message shown: {message}");
        }
        else
        {
            Debug.LogWarning($"Attempted to show portal message '{message}' but portalMessageText is null.");
        }
    }

    private void ClearPortalMessage()
    {
        if (portalMessageText != null)
        {
            portalMessageText.text = "";
            messageShown = false;
            Debug.Log("PortalInfierno message cleared.");
        }
    }
}