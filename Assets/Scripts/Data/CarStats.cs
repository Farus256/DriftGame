using UnityEngine;

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
}