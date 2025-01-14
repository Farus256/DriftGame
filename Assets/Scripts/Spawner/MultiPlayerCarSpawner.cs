using UnityEngine;
using Photon.Pun;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MultiPlayerCarSpawner : MonoBehaviourPun
{
    [SerializeField] private Transform[] spawnPoints;
    public static event Action<GameObject> OnLocalCarSpawned;

    private void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom)
        {
            Debug.LogWarning("[MultiPlayerCarSpawner] PhotonNetwork не готов или не в комнате.");
            return;
        }
        SpawnPlayerCar();
    }

    private void SpawnPlayerCar()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("[MultiPlayerCarSpawner] Нет доступных точек спавна!");
            return;
        }
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        
        if (CarSelection.SelectedCarId < 0)
        {
            Debug.LogError("[MultiPlayerCarSpawner] CarSelection.SelectedCarId не выбран!");
            return;
        }
        
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
            Debug.LogError($"[MultiPlayerCarSpawner] Автомобиль с ID {CarSelection.SelectedCarId} не найден!");
            return;
        }
        
        string prefabPath = $"CarPrefabs/{selectedCar.PrefabName}";
        int carID = selectedCar.ID;
        string paintColorHex = selectedCar.PaintColor;
        string[] activePartsArr = selectedCar.ActiveExtraParts.ToArray();

        object[] instantiationData = new object[]
        {
            carID,
            paintColorHex,
            activePartsArr
        };
        
        GameObject playerCar = PhotonNetwork.Instantiate(
            prefabPath,
            spawnPoint.position,
            spawnPoint.rotation,
            0,
            instantiationData
        );

        if (playerCar == null)
        {
            Debug.LogError($"[MultiPlayerCarSpawner] Не удалось заспавнить автомобиль по пути {prefabPath}.");
            return;
        }

        var pv = playerCar.GetComponent<PhotonView>();
        if (pv == null)
        {
            Debug.LogError($"[MultiPlayerCarSpawner] PhotonView отсутствует на {playerCar.name}.");
            return;
        }

        if (pv.IsMine)
        {
            ApplyCarParameters(playerCar, selectedCar);
            
            playerCar.tag = "Player";
            
            OnLocalCarSpawned?.Invoke(playerCar);
        }
        else
        {
            var controller = playerCar.GetComponent<CarController>();
            if (controller)
            {
                controller.enabled = false;
            }
        }
    }

    private void ApplyCarParameters(GameObject car, CarStats stats)
    {
        CarController controller = car.GetComponent<CarController>();
        if (controller)
        {
            controller.motorPower = stats.MotorPower;
            controller.brakeForce = stats.BrakeForce;
        }
        Rigidbody rb = car.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.mass = stats.Mass;
        }
    }
}
