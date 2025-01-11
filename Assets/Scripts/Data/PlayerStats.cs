using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerStats
{
    [SerializeField] private string playerName;
    public string PlayerName => playerName;

    [SerializeField] private float money;
    public float Money => money;

    [SerializeField] private float totalDriftPoints;
    public float TotalDriftPoints => totalDriftPoints;

    [SerializeField] private int level;
    public int Level => level;

    // Новое поле: список купленных машин
    [SerializeField] private List<int> purchasedCars = new List<int>();
    public List<int> PurchasedCars => purchasedCars;

    public PlayerStats() { }

    public PlayerStats(string playerName, float initialMoney, int initialLevel)
    {
        this.playerName = playerName;
        this.money = initialMoney;
        this.totalDriftPoints = 0f;
        this.level = initialLevel;
        this.purchasedCars = new List<int>();
    }

    public void AddMoney(float amount)
    {
        money += amount;
    }

    public void AddDriftPoints(float amount)
    {
        totalDriftPoints += amount;
    }

    // Методы для покупок
    public bool IsCarPurchased(int carId)
    {
        return purchasedCars.Contains(carId);
    }

    public void PurchaseCar(int carId)
    {
        if (!purchasedCars.Contains(carId))
        {
            purchasedCars.Add(carId);
        }
    }
}