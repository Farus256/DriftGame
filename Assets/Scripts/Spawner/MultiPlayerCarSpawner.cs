using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class MultiPlayerCarSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom) return;
        SpawnPlayerCar();
    }

    private void SpawnPlayerCar()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        if (CarSelection.SelectedCarId < 0) return;
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
        if (selectedCar == null) return;
        string prefabPath = $"CarPrefabs/{selectedCar.PrefabName}";
        GameObject playerCar = PhotonNetwork.Instantiate(prefabPath, spawnPoint.position, spawnPoint.rotation, 0);
        var pv = playerCar.GetComponent<PhotonView>();
        if (pv && pv.IsMine)
        {
            ApplyCarParameters(playerCar, selectedCar);
            ApplyPaintColor(playerCar, selectedCar.PaintColor);
            ApplyExtraParts(playerCar, selectedCar.ActiveExtraParts);
        }
        else
        {
            var controller = playerCar.GetComponent<CarController>();
            if (controller) controller.enabled = false;
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
        if (rb) rb.mass = stats.Mass;
    }

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
        }
    }

    private void ApplyExtraParts(GameObject car, List<string> activeParts)
    {
        string[] possibleParts = { "Spoiler", "SideSkirts" };
        foreach (string partName in possibleParts)
        {
            Transform partTransform = car.transform.Find(partName);
            if (partTransform)
            {
                bool isActive = activeParts.Contains(partName);
                partTransform.gameObject.SetActive(isActive);
            }
        }
    }
}
