using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class MultiPlayerCarSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints; // Массив точек спавна

    private void Start()
    {
        // Проверяем, подключены ли мы к Photon и находимся ли в комнате
        if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom)
        {
            Debug.LogWarning("[MultiPlayerCarSpawner] Photon not ready or not in a room. Skipping spawn.");
            return;
        }

        // Спавним машину только для локального игрока
        SpawnPlayerCar();
    }

    private void SpawnPlayerCar()
    {
        // Выбираем случайный спавн-поинт
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Убедимся, что выбранная машина корректна
        if (CarSelection.SelectedCarId < 0)
        {
            Debug.LogWarning("[MultiPlayerCarSpawner] No car selected. Using default...");
            return;
        }

        // Загружаем данные о машинах
        CarStats[] allCars = CarDataManager.LoadAllCarStats();
        CarStats selectedCar = null;

        foreach (CarStats car in allCars)
        {
            if (car.ID == CarSelection.SelectedCarId)
            {
                selectedCar = car;
                break;
            }
        }

        if (selectedCar == null)
        {
            Debug.LogError($"[MultiPlayerCarSpawner] Car with ID={CarSelection.SelectedCarId} not found in data!");
            return;
        }

        // Спавним машину через PhotonNetwork.Instantiate, указывая путь "CarPrefabs/..."
        string prefabPath = $"CarPrefabs/{selectedCar.PrefabName}"; // Путь относительно Resources
        GameObject playerCar = PhotonNetwork.Instantiate(
            prefabPath,
            spawnPoint.position,
            spawnPoint.rotation,
            0 // Group (по умолчанию)
        );

        // Привязываем ввод только к локальному игроку
        var photonView = playerCar.GetComponent<PhotonView>();
        if (photonView != null && photonView.IsMine)
        {
            // Включаем управление только для своей машины
            ApplyCarParameters(playerCar, selectedCar);

            // Применяем цвет и детали только на локальном объекте.
            // (Хотя можно применить и для всех, если хотим, чтобы видели все —
            //  тогда лучше сделать это без условий "IsMine", чтобы все видели часть и цвет.)
            ApplyPaintColor(playerCar, selectedCar.PaintColor);
            ApplyExtraParts(playerCar, selectedCar.ActiveExtraParts);
        }
        else
        {
            // Если это не наш объект, можно отключить у него управление
            var controller = playerCar.GetComponent<CarController>();
            if (controller != null)
            {
                controller.enabled = false;
            }
        }

        Debug.Log($"[MultiPlayerCarSpawner] Spawned car '{selectedCar.CarName}' at {spawnPoint.position}");
    }

    /// <summary>
    /// Применяет параметры машины (мотор, тормоза, масса) из CarStats.
    /// </summary>
    private void ApplyCarParameters(GameObject car, CarStats stats)
    {
        CarController controller = car.GetComponent<CarController>();
        if (controller != null)
        {
            controller.motorPower = stats.MotorPower;
            controller.brakeForce = stats.BrakeForce;
        }

        Rigidbody rb = car.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = stats.Mass;
        }
    }

    /// <summary>
    /// Применяет цвет покраски к машине.
    /// </summary>
    private void ApplyPaintColor(GameObject car, string colorHex)
    {
        if (ColorUtility.TryParseHtmlString(colorHex, out Color newColor))
        {
            Renderer[] renderers = car.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    rend.materials[i].color = newColor;
                }
            }
            Debug.Log($"[MultiPlayerCarSpawner] Applied color {colorHex} to car.");
        }
        else
        {
            Debug.LogError($"[MultiPlayerCarSpawner] Invalid color code: {colorHex}");
        }
    }

    /// <summary>
    /// Включает/выключает дополнительные детали (Spoiler, SideSkirts и т.д.).
    /// Предполагается, что объекты в префабе называются так же, как и в списке activeParts.
    /// </summary>
    private void ApplyExtraParts(GameObject car, List<string> activeParts)
    {
        string[] possibleParts = { "Spoiler", "SideSkirts" };

        foreach (string partName in possibleParts)
        {
            Transform partTransform = car.transform.Find(partName);
            if (partTransform != null)
            {
                bool shouldBeActive = activeParts.Contains(partName);
                partTransform.gameObject.SetActive(shouldBeActive);
                Debug.Log($"[MultiPlayerCarSpawner] {(shouldBeActive ? "Enabled" : "Disabled")} part: {partName}");
            }
            else
            {
                Debug.LogWarning($"[MultiPlayerCarSpawner] Part '{partName}' not found in prefab hierarchy.");
            }
        }
    }
}
