using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCarSpawner : MonoBehaviour
{
    [SerializeField] private Transform playerCarSpawnPoint;

    private void Awake()
    {
        InitializePlayerCar();
    }

    private void InitializePlayerCar()
    {
        // 1) Проверим, что в CarSelection что-то выбрано
        if (CarSelection.SelectedCarId < 0)
        {
            Debug.LogWarning("[GameSceneManager] No car was selected. Using default or just skipping...");
            return;
        }

        // 2) Загрузим список машин (через CarDataManager)
        CarStats[] allCars = CarDataManager.LoadAllCarStats();

        // Найдём машину по ID
        CarStats selectedCar = null;
        for (int i = 0; i < allCars.Length; i++)
        {
            if (allCars[i].ID == CarSelection.SelectedCarId)
            {
                selectedCar = allCars[i];
                break;
            }
        }

        if (selectedCar == null)
        {
            Debug.LogError($"[GameSceneManager] Car with ID={CarSelection.SelectedCarId} not found in data!");
            return;
        }

        // 3) Загружаем префаб по имени (prefabName) из Resources/CarPrefabs/
        string prefabPath = $"CarPrefabs/{selectedCar.PrefabName}";
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[GameSceneManager] Prefab not found at path: {prefabPath}");
            return;
        }

        // 4) Спавним машину в сцене
        GameObject playerCar = Instantiate(prefab, playerCarSpawnPoint.position, playerCarSpawnPoint.rotation);

        // 5) Настраиваем общие характеристики: мотор, тормоза, масса
        ApplyCarParameters(playerCar, selectedCar);

        // 6) Применяем цвет и дополнительные детали
        ApplyPaintColor(playerCar, selectedCar.PaintColor);
        ApplyExtraParts(playerCar, selectedCar.ActiveExtraParts);

        Debug.Log($"[GameSceneManager] Spawned car '{selectedCar.CarName}' with ID={selectedCar.ID}");
    }

    /// <summary>
    /// Применяет параметры машины (мотор, тормоза, масса) из CarStats.
    /// </summary>
    private void ApplyCarParameters(GameObject car, CarStats stats)
    {
        // CarController (если в вашем проекте нужен для управления)
        CarController controller = car.GetComponent<CarController>();
        if (controller != null)
        {
            controller.motorPower = stats.MotorPower;
            controller.brakeForce = stats.BrakeForce;
        }

        // Rigidbody
        Rigidbody rb = car.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = stats.Mass;
        }
    }

    /// <summary>
    /// Применяет цвет покраски к машине (по всему Mesh-у).
    /// </summary>
    private void ApplyPaintColor(GameObject car, string colorHex)
    {
        if (ColorUtility.TryParseHtmlString(colorHex, out Color newColor))
        {
            Renderer[] renderers = car.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                // Меняем цвет у всех материалов рендера
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    rend.materials[i].color = newColor;
                }
            }
            Debug.Log($"[GameSceneManager] Applied color {colorHex} to car.");
        }
        else
        {
            Debug.LogError($"[GameSceneManager] Invalid color code: {colorHex}");
        }
    }

    /// <summary>
    /// Включает/выключает дополнительные детали (Spoiler, SideSkirts и т.д.).
    /// Предполагается, что объекты в префабе называются так же, как и в ActiveExtraParts.
    /// </summary>
    private void ApplyExtraParts(GameObject car, List<string> activeParts)
    {
        // Для упрощения считаем, что все доступные детали
        // уже есть в иерархии префаба с соответствующими именами.
        // Если детали нет - выводим предупреждение.
        string[] possibleParts = { "Spoiler", "SideSkirts" }; // Можно расширить

        foreach (string partName in possibleParts)
        {
            Transform partTransform = car.transform.Find(partName);
            if (partTransform != null)
            {
                // Активируем, если в списке, иначе — деактивируем
                bool shouldBeActive = activeParts.Contains(partName);
                partTransform.gameObject.SetActive(shouldBeActive);
                Debug.Log($"[GameSceneManager] {(shouldBeActive ? "Enabled" : "Disabled")} part: {partName}");
            }
            else
            {
                Debug.LogWarning($"[GameSceneManager] Part '{partName}' not found in prefab hierarchy.");
            }
        }
    }
}
