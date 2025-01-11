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

    [SerializeField] private List<string> availableExtraParts;
    public List<string> AvailableExtraParts => availableExtraParts;

    [SerializeField] private List<string> activeExtraParts;
    public List<string> ActiveExtraParts => activeExtraParts;

    [SerializeField] private string paintColor;
    public string PaintColor => paintColor;

    [SerializeField] private float cost;
    public float Cost => cost;

    public void UpgradeMotorPower(float amount)
    {
        motorPower += amount;
        Debug.Log($"[CarStats] Motor Power +{amount} = {motorPower}");
    }

    public void UpgradeBrakeForce(float amount)
    {
        brakeForce += amount;
        Debug.Log($"[CarStats] Brake Force +{amount} = {brakeForce}");
    }

    public void ActivateExtraPart(string partName)
    {
        if (!activeExtraParts.Contains(partName) && availableExtraParts.Contains(partName))
        {
            activeExtraParts.Add(partName);
            Debug.Log($"[CarStats] Activated part: {partName}");
        }
    }

    public void DeactivateExtraPart(string partName)
    {
        if (activeExtraParts.Contains(partName))
        {
            activeExtraParts.Remove(partName);
            Debug.Log($"[CarStats] Deactivated part: {partName}");
        }
    }

    public void SetPaintColor(string colorHex)
    {
        paintColor = colorHex;
        Debug.Log($"[CarStats] Paint color = {paintColor}");
    }
}
