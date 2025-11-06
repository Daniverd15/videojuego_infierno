using System.Collections;
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
    public int totalItemsScene = 2;
    public string collectibleTag = "Collectible";
    private static int itemsCollected = 0;

    [Header("Lista de objetos")]
    public List<Transform> collectibles = new List<Transform>();
    private Dictionary<Transform, Vector3> startPosition = new Dictionary<Transform, Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (var obj in collectibles)
        {
            if (obj != null)
            {
                startPosition[obj] = obj.position;

                Collider col = obj.GetComponent<Collider>();
                if (col == null)
                    col = obj.gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;

                if (obj.GetComponent<PlayerCollectibleDetector>() == null)
                    obj.gameObject.AddComponent<PlayerCollectibleDetector>().Init(this);
            }
        }

        UpdatedCounterUI();
    }

    // Update is called once per frame
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

       collectibles.Remove(obj);
       itemsCollected++;
       UpdatedCounterUI();
       Destroy(obj.gameObject);
    }
    
    void UpdatedCounterUI()
    {
        if (itemCounter != null)
        {
           itemCounter.text = $"{itemsCollected} / {totalItemsScene}";
        }
    }
}
