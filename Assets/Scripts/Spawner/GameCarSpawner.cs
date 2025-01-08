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

        // 5) Устанавливаем CarController характеристики (если нужно)
        CarController controller = playerCar.GetComponent<CarController>();
        if (controller != null)
        {
            controller.motorPower = selectedCar.MotorPower;
            controller.brakeForce = selectedCar.BrakeForce;
        }

        // Или настраиваем Rigidbody
        Rigidbody rb = playerCar.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = selectedCar.Mass;
        }

        Debug.Log($"[GameSceneManager] Spawned car '{selectedCar.CarName}' with ID={selectedCar.ID}");
    }
}
