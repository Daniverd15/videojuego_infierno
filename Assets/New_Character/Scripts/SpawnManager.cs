using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    private Dictionary<string, Vector3> sceneSpawnPoints = new Dictionary<string, Vector3>();
    private Vector3? nextSpawnPoint = null;
    private bool spawnPointSetForNextScene = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    /// <summary>
    /// Registers a spawn point position for a given scene name.
    /// Call this during setup for each known spawn location.
    /// </summary>
    public void RegisterSpawnPoint(string sceneName, Vector3 position)
    {
        sceneSpawnPoints[sceneName] = position;
    }

    /// <summary>
    /// Requests the next spawn to be at this position.
    /// Portals or other systems should call this before scene load.
    /// </summary>
    public void SetNextSpawnPoint(Vector3 spawnPosition)
    {
        nextSpawnPoint = spawnPosition;
        spawnPointSetForNextScene = true;
    }

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    Debug.Log($"SpawnManager: OnSceneLoaded called for scene {scene.name}. spawnPointSetForNextScene={spawnPointSetForNextScene}, nextSpawnPoint={nextSpawnPoint}");

    if (spawnPointSetForNextScene && nextSpawnPoint.HasValue)
    {
        TeleportPlayer(nextSpawnPoint.Value);
    }
    else if (sceneSpawnPoints.ContainsKey(scene.name))
    {
        TeleportPlayer(sceneSpawnPoints[scene.name]);
    }
    else
    {
        Debug.LogWarning($"SpawnManager: No spawn point registered for scene {scene.name} and no next spawn explicitly set.");
    }

    spawnPointSetForNextScene = false;
    nextSpawnPoint = null;
}

private void TeleportPlayer(Vector3 position)
{
    Debug.Log($"SpawnManager: TeleportPlayer called with position {position}");
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
        New_CharacterController playerController = player.GetComponent<New_CharacterController>();
        if (playerController != null)
        {
            playerController.TeleportTo(position);
            Debug.Log($"SpawnManager: Player teleported to {position}");
        }
        else
        {
            Debug.LogWarning("SpawnManager: Player missing New_CharacterController component.");
        }
    }
    else
    {
        Debug.LogWarning("SpawnManager: Player GameObject with tag 'Player' not found.");
    }
}
}
