using UnityEngine;

public class MenuCarSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    private CarStats[] allCars;
    private int currentCarIndex;
    private GameObject currentCarInstance;

    private void Awake()
    {
        allCars = CarDataManager.LoadAllCarStats();
        currentCarIndex = 0;
        ShowCar(currentCarIndex);
    }

    private void OnEnable()
    {
        GlobalEventManager.onCarUpdated.AddListener(RefreshCurrentCar);
    }

    private void OnDisable()
    {
        GlobalEventManager.onCarUpdated.RemoveListener(RefreshCurrentCar);
    }

    public void ShowNextCar()
    {
        if (allCars == null || allCars.Length == 0) return;
        currentCarIndex++;
        if (currentCarIndex >= allCars.Length) currentCarIndex = 0;
        ShowCar(currentCarIndex);
    }

    public void ShowPreviousCar()
    {
        if (allCars == null || allCars.Length == 0) return;
        currentCarIndex--;
        if (currentCarIndex < 0) currentCarIndex = allCars.Length - 1;
        ShowCar(currentCarIndex);
    }

    public CarStats GetCurrentCarStats()
    {
        if (allCars == null || allCars.Length == 0) return null;
        return allCars[currentCarIndex];
    }

    public CarStats[] GetAllCarStats()
    {
        return allCars;
    }

    public GameObject GetCurrentCarInstance()
    {
        return currentCarInstance;
    }

    private void ShowCar(int index)
    {
        if (currentCarInstance)
        {
            Destroy(currentCarInstance);
            currentCarInstance = null;
        }
        if (allCars == null || index < 0 || index >= allCars.Length) return;
       
        CarStats stats = allCars[index];
        
        CarSelection.SelectCar(stats.ID);
        
        string prefabPath = $"CarPrefabs/{stats.PrefabName}";
        
        GameObject carPrefab = Resources.Load<GameObject>(prefabPath);
        
        if (!carPrefab) return;
        
        if (!spawnPoint) spawnPoint = transform;
        currentCarInstance = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
       
        ApplyPaintColor(currentCarInstance, stats.PaintColor);
       
        ActivateExtraParts(currentCarInstance, stats.ActiveExtraParts);
        
        Rigidbody rb = currentCarInstance.GetComponent<Rigidbody>();
        if (rb) rb.mass = stats.Mass;
        
        TuningUIManager.Instance?.RefreshUI();
    }

    private void RefreshCurrentCar()
    {
        ShowCar(currentCarIndex);
    }

    private static void ApplyPaintColor(GameObject carInstance, string colorHex)
    {
        if (ColorUtility.TryParseHtmlString(colorHex, out Color newColor))
        {
            Renderer[] renderers = carInstance.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials[i].color = newColor;
                }
            }
        }
    }

    private static void ActivateExtraParts(GameObject carInstance, System.Collections.Generic.List<string> activeParts)
    {
        foreach (string partName in activeParts)
        {
            Transform partTransform = carInstance.transform.Find(partName);
            if (partTransform) partTransform.gameObject.SetActive(true);
        }
    }
}
