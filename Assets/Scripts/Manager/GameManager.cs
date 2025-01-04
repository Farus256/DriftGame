using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform playerCarSpawnPoint; // Точка спавна для игрока

    private void Start()
    {
        InitializePlayerCar();
    }

    private void InitializePlayerCar()
    {
        CarStats selectedCar = GameData.SelectedCar;
        if (selectedCar == null)
        {
            Debug.LogError("No car selected!");
            return;
        }

        string prefabPath = $"CarPrefabs/{selectedCar.prefabName}";
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab != null)
        {
            GameObject playerCar = Instantiate(prefab, playerCarSpawnPoint.position, playerCarSpawnPoint.rotation);
            CarController controller = playerCar.GetComponent<CarController>();
            if (controller != null)
            {
                //controller.Initialize(selectedCar);
            }
            else
            {
                Debug.LogWarning($"CarController not found on prefab: {selectedCar.carName}");
            }
        }
        else
        {
            Debug.LogError($"Player car prefab not found at path: {prefabPath}");
        }
    }
}