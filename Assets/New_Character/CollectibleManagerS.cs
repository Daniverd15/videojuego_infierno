using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class CollectibleManagerS : MonoBehaviour
{
    [Header("Secuencia Movimiento")]
    public float amplitud = 0.25f;
    public float speed = 2f;
    public float rotationSpeed = 45f;

    [Header("Sistema Recolección")]
    public TextMeshProUGUI itemCounter;
    public TextMeshProUGUI crossCounter; // UI element for crosses count
    public TextMeshProUGUI crossBonusMessage; // UI element for bonus message
    public int totalItemsScene = 2;
    public string collectibleTag = "Collectible";
    public string crossTag = "Cruz"; // Tag for crosses
    public string rosarioTag = "Rosario"; // Tag for rosarios
    public string winTag = "Win"; // Tag for win collectible

    private static int itemsCollected = 0;
    private static int crossesCollected = 0; 
    private static int rosariosCollected = 0; // Counter for rosarios

    [Header("Lista de objetos")]
    public List<Transform> collectibles = new List<Transform>();

    [Header("UI Rosario")]
    public TextMeshProUGUI rosarioCounter; // UI element for rosario count

    private Dictionary<Transform, Vector3> startPosition = new Dictionary<Transform, Vector3>();

    [Header("Referencias")]
    [SerializeField] private GameTimer timer;     // Assign in inspector

    void Start()
    {
        if (timer == null) timer = FindObjectOfType<GameTimer>(true);

        foreach (var obj in collectibles)
        {
            if (obj == null) continue;

            startPosition[obj] = obj.position;

            var col = obj.GetComponent<Collider>();
            if (col == null) col = obj.gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;

            if (obj.GetComponent<PlayerCollectibleDetector>() == null)
                obj.gameObject.AddComponent<PlayerCollectibleDetector>().Init(this);
        }

        UpdatedCounterUI();
        UpdateCrossesUI();
    }

    void Update()
    {
        foreach (var obj in collectibles)
        {
            if (obj == null) continue;

            Vector3 StartPos = startPosition[obj];
            float newY = StartPos.y + Mathf.Sin(Time.time * speed) * amplitud;
            obj.position = new Vector3(StartPos.x, newY, StartPos.z);
            obj.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    public void Collect(Transform obj)
    {
        if (!collectibles.Contains(obj)) return;

        var bonus = obj.GetComponent<TimeBonus>();
        if (bonus != null)
        {
            if (timer == null) timer = FindObjectOfType<GameTimer>(true);
            if (timer != null)
            {
                timer.AddTime(bonus.secondsToAdd);
                Debug.Log($"[CollectibleManagerS] +{bonus.secondsToAdd}s al timer");
            }
            else
            {
                Debug.LogWarning("[CollectibleManagerS] No se encontró GameTimer en la escena.");
            }
        }

        collectibles.Remove(obj);

        if (obj.CompareTag(winTag))
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex - 2;
            Debug.Log($"[CollectibleManagerS] Win collectible obtained! Loading scene with buildIndex: {nextSceneIndex}");
            SceneManager.LoadScene(nextSceneIndex);
            return; // Stop further processing after win
        }
        else if (obj.CompareTag(crossTag))
        {
            crossesCollected++;
            Debug.Log($"[CollectibleManagerS] Cross collected. Current crossesCollected: {crossesCollected}");
            UpdateCrossesUI();
        }
        else if (obj.CompareTag(rosarioTag))
        {
            rosariosCollected++;
            Debug.Log($"[CollectibleManagerS] Rosario collected. Current rosariosCollected: {rosariosCollected}");
            if (rosarioCounter != null)
                rosarioCounter.text = $"{rosariosCollected}";
        }
        else
        {
            itemsCollected++;
            UpdatedCounterUI();
        }

        Destroy(obj.gameObject);

        Debug.Log($"[CollectibleManagerS] Collect(): crossesCollected={crossesCollected}, rosariosCollected={rosariosCollected}, itemsCollected={itemsCollected}, collectibles.Count={collectibles.Count}");
    }

    void UpdatedCounterUI()
    {
        if (itemCounter != null)
            itemCounter.text = $"{itemsCollected} / {totalItemsScene}";
    }

    void UpdateCrossesUI()
    {
        if (crossCounter != null)
            crossCounter.text = $"{crossesCollected}";

        if (crossBonusMessage != null)
        {
            if (CrossesComplete())
            {
                crossBonusMessage.text = "cruces completas, vuelve al pentagrama para llegar al cielo (BONUS LEVEL)";
            }
            else
            {
                crossBonusMessage.text = "";
            }
        }

        if (rosarioCounter != null)
            rosarioCounter.text = $"{rosariosCollected}";
    }

    public static bool CrossesComplete()
    {
        bool complete = crossesCollected == 4;
        Debug.Log($"[CollectibleManagerS] CrossesComplete() (STATIC) called, returns: {complete} (crossesCollected={crossesCollected})");
        return complete;
    }

    public static bool RosariosComplete()
    {
        bool complete = rosariosCollected == 2;
        Debug.Log($"[CollectibleManagerS] RosariosComplete() (STATIC) called, returns: {complete} (rosariosCollected={rosariosCollected})");
        return complete;
    }
}
