using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CarStats
{
    [SerializeField] private int id;
    public int ID => id;

    [SerializeField] private string carName;
    public string CarName => carName;

    [SerializeField] private float mass;
    public float Mass => mass;

    [SerializeField] private float motorPower;
    public float MotorPower => motorPower;

    [SerializeField] private float brakeForce;
    public float BrakeForce => brakeForce;

    [SerializeField] private string prefabName;
    public string PrefabName => prefabName;

    // Список доступных дополнительных деталей
    [SerializeField] private List<string> availableExtraParts;
    public List<string> AvailableExtraParts => availableExtraParts;

    // Список активных дополнительных деталей
    [SerializeField] private List<string> activeExtraParts;
    public List<string> ActiveExtraParts => activeExtraParts;

    // Цвет покраски в формате HEX
    [SerializeField] private string paintColor; // Пример: "#FFFFFF" для белого
    public string PaintColor => paintColor;

    // Новое поле - стоимость машины
    [SerializeField] private float cost;
    public float Cost => cost;

    // Методы для улучшения характеристик
    public void UpgradeMotorPower(float amount)
    {
        motorPower += amount;
        Debug.Log($"[CarStats] Motor Power upgraded by {amount}. New Motor Power: {motorPower}");
    }

    public void UpgradeBrakeForce(float amount)
    {
        brakeForce += amount;
        Debug.Log($"[CarStats] Brake Force upgraded by {amount}. New Brake Force: {brakeForce}");
    }

    // Методы для активации и деактивации дополнительных деталей
    public void ActivateExtraPart(string partName)
    {
        if (!activeExtraParts.Contains(partName) && availableExtraParts.Contains(partName))
        {
            activeExtraParts.Add(partName);
            Debug.Log($"[CarStats] Activated extra part: {partName}");
        }
    }

    public void DeactivateExtraPart(string partName)
    {
        if (activeExtraParts.Contains(partName))
        {
            activeExtraParts.Remove(partName);
            Debug.Log($"[CarStats] Deactivated extra part: {partName}");
        }
    }

    // Метод для установки цвета покраски
    public void SetPaintColor(string colorHex)
    {
        paintColor = colorHex;
        Debug.Log($"[CarStats] Paint color set to: {paintColor}");
    }
}
