using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectibleManagerS : MonoBehaviour
{
    [Header("Secuencia Movimiento")]
    public float amplitud = 0.25f;
    public float speed = 2f;
    public float rotationSpeed = 45f;

    [Header("Sistema Recolección")]
    public TextMeshProUGUI itemCounter;
    public TextMeshProUGUI crossCounter; // New UI element for crosses count
    public int totalItemsScene = 2;
    public string collectibleTag = "Collectible";
    public string crossTag = "Cruz"; // Assuming crosses have this tag
    private static int itemsCollected = 0;
    private int crossesCollected = 0; // New count for crosses collected

    [Header("Lista de objetos")]
    public List<Transform> collectibles = new List<Transform>();
    private Dictionary<Transform, Vector3> startPosition = new Dictionary<Transform, Vector3>();

    [Header("Referencias")]
    [SerializeField] private GameTimer timer;     // ⬅️ arrástralo en el Inspector

    void Start()
    {
        // fallback por si olvidaste asignarlo
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

        // ⬇️ SUMA TIEMPO si este ítem tiene TimeBonus
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

        // Check if collectible is a cross (by tag)
        if (obj.CompareTag(crossTag))
        {
            crossesCollected++;
            UpdateCrossesUI();
        }
        else
        {
            itemsCollected++;
            UpdatedCounterUI();
        }

        Destroy(obj.gameObject);
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
    }
}
