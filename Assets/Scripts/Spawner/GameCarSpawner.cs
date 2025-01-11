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
        if (CarSelection.SelectedCarId < 0) return;
        CarStats[] allCars = CarDataManager.LoadAllCarStats();
        CarStats selectedCar = null;
        for (int i = 0; i < allCars.Length; i++)
        {
            if (allCars[i].ID == CarSelection.SelectedCarId)
            {
                selectedCar = allCars[i];
                break;
            }
        }
        if (selectedCar == null) return;
        string prefabPath = $"CarPrefabs/{selectedCar.PrefabName}";
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (!prefab) return;
        GameObject playerCar = Instantiate(prefab, playerCarSpawnPoint.position, playerCarSpawnPoint.rotation);
        ApplyCarParameters(playerCar, selectedCar);
        ApplyPaintColor(playerCar, selectedCar.PaintColor);
        ApplyExtraParts(playerCar, selectedCar.ActiveExtraParts);
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

    private void ApplyExtraParts(GameObject car, System.Collections.Generic.List<string> activeParts)
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
