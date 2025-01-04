using UnityEngine;
using UnityEngine.UI;

public class CarInitializer : MonoBehaviour
{
    [Header("UI Elements (Optional)")]
    public Text carNameLabel; 
    // Или лучше использовать TextMeshPro, но для примера пойдёт

    [Header("Spawn Settings")]
    public Transform spawnPoint; 
    // Точка, где будет появляться машина (камера гаража смотрит на неё)

    private CarStats[] _allCars;   // список всех машин из JSON
    private int _currentCarIndex;  // индекс выбранной машины
    private GameObject _currentCarInstance;

    private void Start()
    {
        // 1) Загружаем все характеристики
        _allCars = CarDataManager.LoadAllCarStats();

        // 2) Начинаем с 0-й машины
        _currentCarIndex = 0;

        // 3) Показываем её
        ShowCar(_currentCarIndex);
    }

    public void ShowNextCar()
    {
        if (_allCars == null || _allCars.Length == 0)
            return;

        _currentCarIndex++;
        if (_currentCarIndex >= _allCars.Length)
            _currentCarIndex = 0;

        ShowCar(_currentCarIndex);
    }

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
    /// Возвращает текущие характеристики, если нужны в другом месте (например, MenuUIManager).
    /// </summary>
    public CarStats GetCurrentCarStats()
    {
        if (_allCars == null || _allCars.Length == 0) return null;
        return _allCars[_currentCarIndex];
    }

    /// <summary>
    /// Спавнит машину по индексу, удаляя предыдущую.
    /// </summary>
    private void ShowCar(int index)
    {
        // Удаляем предыдущую машину
        if (_currentCarInstance != null)
        {
            Destroy(_currentCarInstance);
        }

        if (_allCars == null || index < 0 || index >= _allCars.Length)
            return;

        CarStats stats = _allCars[index];

        // Если вы храните prefabName в JSON, и положили префабы в папку Resources,
        // то можно загрузить их так:
        GameObject carPrefab = Resources.Load<GameObject>(stats.prefabName);
        if (carPrefab == null)
        {
            Debug.LogError($"Prefab not found in Resources: {stats.prefabName}");
            return;
        }

        // Создаём экземпляр машины
        if (!spawnPoint) spawnPoint = transform;
        _currentCarInstance = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
        _currentCarInstance.transform.SetParent(spawnPoint, true);

        // Обновляем UI (название машины)
        if (carNameLabel != null)
        {
            carNameLabel.text = stats.carName;
        }

        // Если вам нужна физика в меню (например, вращать колёса),
        // можно прямо здесь поставить mass и т.д.:
        Rigidbody rb = _currentCarInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = stats.mass;
        }
        // Но часто в меню делают просто статическую модель без физики.
    }
}
