using System.Collections.Generic;
using UnityEngine;

public class MenuCarSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;

    private CarStats[] _allCars;   // Список всех машин из JSON
    private int _currentCarIndex;  // Текущий индекс в массиве
    private GameObject _currentCarInstance;

    private void Awake()
    {
        Debug.Log("[MenuCarSpawner] Awake called.");

        // Загружаем массив машин из JSON через CarDataManager
        _allCars = CarDataManager.LoadAllCarStats();
        Debug.Log($"[MenuCarSpawner] Loaded {_allCars.Length} cars.");
    }
    
    private void Start()
    {
        // Начинаем с 0-й машины
        _currentCarIndex = 0;
        ShowCar(_currentCarIndex);
    }
    
    private void OnEnable()
    {
        // Подписка на событие обновления машины
        GlobalEventManager.onCarUpdated.AddListener(RefreshCurrentCar);
    }

    private void OnDisable()
    {
        // Отписка от события
        GlobalEventManager.onCarUpdated.RemoveListener(RefreshCurrentCar);
    }

    /// <summary>
    /// Переключиться на следующую машину
    /// </summary>
    public void ShowNextCar()
    {
        if (_allCars == null || _allCars.Length == 0)
        {
            Debug.LogWarning("[MenuCarSpawner] No cars to show.");
            return;
        }

        _currentCarIndex++;
        if (_currentCarIndex >= _allCars.Length)
            _currentCarIndex = 0;

        ShowCar(_currentCarIndex);
    }

    /// <summary>
    /// Переключиться на предыдущую машину
    /// </summary>
    public void ShowPreviousCar()
    {
        if (_allCars == null || _allCars.Length == 0)
        {
            Debug.LogWarning("[MenuCarSpawner] No cars to show.");
            return;
        }

        _currentCarIndex--;
        if (_currentCarIndex < 0)
            _currentCarIndex = _allCars.Length - 1;

        ShowCar(_currentCarIndex);
    }

    /// <summary>
    /// Получить CarStats текущей машины (если нужно где-то ещё).
    /// </summary>
    public CarStats GetCurrentCarStats()
    {
        if (_allCars == null || _allCars.Length == 0) return null;
        return _allCars[_currentCarIndex];
    }

    /// <summary>
    /// Получить все CarStats (если нужно где-то ещё).
    /// </summary>
    public CarStats[] GetAllCarStats()
    {
        return _allCars;
    }

    /// <summary>
    /// Получить экземпляр текущей машины
    /// </summary>
    public GameObject GetCurrentCarInstance()
    {
        return _currentCarInstance;
    }

    /// <summary>
    /// Отобразить машину по индексу (удаляет предыдущую)
    /// </summary>
    private void ShowCar(int index)
    {
        Debug.Log($"[MenuCarSpawner] Showing car at index: {index}");

        // Удаляем предыдущую машину (если есть)
        if (_currentCarInstance != null)
        {
            Destroy(_currentCarInstance);
            _currentCarInstance = null;
        }

        // Защита от неверного индекса
        if (_allCars == null || index < 0 || index >= _allCars.Length)
        {
            Debug.LogError("[MenuCarSpawner] Invalid car index.");
            return;
        }

        CarStats stats = _allCars[index];
        Debug.Log($"[MenuCarSpawner] Loading car: {stats.CarName} (ID={stats.ID})");

        // Устанавливаем текущий выбранный CarId через CarSelection
        CarSelection.SelectCar(stats.ID);

        // Загружаем префаб из папки Resources/CarPrefabs/<prefabName>.prefab
        string prefabPath = $"CarPrefabs/{stats.PrefabName}";
        GameObject carPrefab = Resources.Load<GameObject>(prefabPath);
        if (carPrefab == null)
        {
            Debug.LogError($"[MenuCarSpawner] Prefab not found in Resources: {prefabPath}");
            return;
        }

        // Спавним префаб
        if (!spawnPoint) spawnPoint = transform;
        _currentCarInstance = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);

        Debug.Log("[MenuCarSpawner] Car prefab instantiated.");

        // Применяем покраску
        ApplyPaintColor(_currentCarInstance, stats.PaintColor);

        // Активируем дополнительные детали
        ActivateExtraParts(_currentCarInstance, stats.ActiveExtraParts);

        // Если хотим физику в меню — задаём массу:
        Rigidbody rb = _currentCarInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = stats.Mass;
            Debug.Log($"[MenuCarSpawner] Set Rigidbody mass to {stats.Mass}");
        }

        Debug.Log($"[MenuCarSpawner] Showing car: {stats.CarName} (ID={stats.ID}), index={index}");
    }

    /// <summary>
    /// Обработчик события "машина обновилась"
    /// </summary>
    private void RefreshCurrentCar()
    {
        Debug.Log("[MenuCarSpawner] RefreshCurrentCar called.");
        // Пересоздаём текущую машину
        ShowCar(_currentCarIndex);
    }

    /// <summary>
    /// Применяет цвет покраски к машине
    /// </summary>
    private void ApplyPaintColor(GameObject carInstance, string colorHex)
    {
        Renderer[] renderers = carInstance.GetComponentsInChildren<Renderer>();
        if (ColorUtility.TryParseHtmlString(colorHex, out Color newColor))
        {
            foreach (Renderer renderer in renderers)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials[i].color = newColor;
                }
            }
            Debug.Log($"[MenuCarSpawner] Applied color {colorHex} to car.");
        }
        else
        {
            Debug.LogError($"[MenuCarSpawner] Invalid color code: {colorHex}");
        }
    }

    /// <summary>
    /// Активирует дополнительные детали на машине
    /// </summary>
    private void ActivateExtraParts(GameObject carInstance, List<string> activeParts)
    {
        foreach (string partName in activeParts)
        {
            Transform partTransform = carInstance.transform.Find(partName);
            if (partTransform != null)
            {
                partTransform.gameObject.SetActive(true);
                Debug.Log($"[MenuCarSpawner] Activated part: {partName}");
            }
            else
            {
                Debug.LogWarning($"[MenuCarSpawner] Part not found: {partName}");
            }
        }
    }
}
