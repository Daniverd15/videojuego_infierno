using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LiveSystem : MonoBehaviour
{
    [Header("Vidas")]
    public int maxLives = 3;
    private int currentLives;
    public Transform respawnPoint;
    public TextMeshProUGUI livesText;
    CharacterController cc;
    // Start is called before the first frame update
    void Start()
    {
        currentLives = maxLives;
        if (respawnPoint == null) respawnPoint = transform;
        cc = GetComponent<CharacterController>();
        UpdateLivesUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = $"{currentLives}/{maxLives}";
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            currentLives--;
            UpdateLivesUI();   
            Debug.Log("Vidas restantes: " + currentLives);

            if (currentLives <= -1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);;
            }
            else
            {
                transform.position = respawnPoint.position;
            }
        }
    }

    // Added method to handle life decrease and respawn on platform crush
    public void DecreaseLifeAndRespawn()
    {
        currentLives--;
        UpdateLivesUI();
        Debug.Log("Vidas restantes tras aplastamiento: " + currentLives);

        if (currentLives < 0)
        {
            SceneManager.LoadScene("SampleScene"); // Reload scene if no lives left
        }
        else
        {
            // Respawn player at respawn point
            if (cc != null)
            {
                cc.enabled = false;  // Disable character controller to allow position change
                transform.position = respawnPoint.position;
                cc.enabled = true;   // Re-enable character controller
            }
            else
            {
                transform.position = respawnPoint.position;
            }
        }
    }
}
