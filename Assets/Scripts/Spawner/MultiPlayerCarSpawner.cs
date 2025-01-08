using UnityEngine;
using Photon.Pun;

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

        // Спавним машину через PhotonNetwork.Instantiate, указывая путь с "CarPrefabs/"
        string prefabPath = $"CarPrefabs/{selectedCar.PrefabName}"; // Путь относительно Resources
        GameObject playerCar = PhotonNetwork.Instantiate(
            prefabPath,
            spawnPoint.position,
            spawnPoint.rotation,
            0 // Group (по умолчанию)
        );

        // Привязываем ввод только к локальному игроку
        var controller = playerCar.GetComponent<CarController>();
        if (controller != null)
        {
            if (playerCar.GetComponent<PhotonView>().IsMine)
            {
                controller.enabled = true; // Включаем управление только для локального игрока
            }
            else
            {
                controller.enabled = false; // Отключаем для других
            }
        }

        // Настраиваем параметры машины
        var rb = playerCar.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = selectedCar.Mass;
        }

        Debug.Log($"[MultiPlayerCarSpawner] Spawned car '{selectedCar.CarName}' at {spawnPoint.position}");
    }

}
