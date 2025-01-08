using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuCarSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;

    private CarStats[] _allCars;     // Список всех машин из JSON
    private int _currentCarIndex;    // Текущий индекс в массиве
    private GameObject _currentCarInstance;

    private void Awake()
    {
        // Загружаем массив машин из JSON через CarDataManager
        _allCars = CarDataManager.LoadAllCarStats();

        // Начинаем с 0-й машины
        _currentCarIndex = 0;
        ShowCar(_currentCarIndex);
    }

    /// <summary>
    /// Переключиться на следующую машину
    /// </summary>
    public void ShowNextCar()
    {
        if (_allCars == null || _allCars.Length == 0)
            return;

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
            return;

        _currentCarIndex--;
        if (_currentCarIndex < 0)
            _currentCarIndex = _allCars.Length - 1;

        ShowCar(_currentCarIndex);
    }

    /// <summary>
    /// Получить CarStats текущей машины
    /// </summary>
    public CarStats GetCurrentCarStats()
    {
        if (_allCars == null || _allCars.Length == 0) return null;
        return _allCars[_currentCarIndex];
    }

    /// <summary>
    /// Получить все CarStats
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
        // Удаляем предыдущую машину (если есть)
        if (_currentCarInstance != null)
        {
            Destroy(_currentCarInstance);
        }

        // Защита от неверного индекса
        if (_allCars == null || index < 0 || index >= _allCars.Length)
            return;

        CarStats stats = _allCars[index];

        // Загружаем префаб из папки Resources/CarPrefabs/<prefabName>.prefab
        string prefabPath = $"CarPrefabs/{stats.PrefabName}";
        GameObject carPrefab = Resources.Load<GameObject>(prefabPath);
        if (carPrefab == null)
        {
            Debug.LogError($"[MenuCarSpawner] Prefab not found in Resources: {prefabPath}");
            return;
        }

        // Спавним префаб в spawnPoint
        if (!spawnPoint) spawnPoint = transform;
        _currentCarInstance = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);

        // Применяем покраску
        ApplyPaintColor(_currentCarInstance, stats.PaintColor);

        // Активируем дополнительные детали
        ActivateExtraParts(_currentCarInstance, stats.ActiveExtraParts);

        // Если хотим физику в меню — задаём массу:
        Rigidbody rb = _currentCarInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = stats.Mass;
        }

        Debug.Log($"[MenuCarSpawner] Showing car: {stats.CarName} (ID={stats.ID}), index={index}");
    }

    /// <summary>
    /// Применяет цвет покраски к машине
    /// </summary>
    private void ApplyPaintColor(GameObject carInstance, string colorHex)
    {
        Renderer[] renderers = carInstance.GetComponentsInChildren<Renderer>();
        Color newColor;
        if (ColorUtility.TryParseHtmlString(colorHex, out newColor))
        {
            foreach (Renderer renderer in renderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.color = newColor;
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
