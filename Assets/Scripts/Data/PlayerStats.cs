using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    [SerializeField] private string playerName;
    public string PlayerName => playerName;
    
    [SerializeField] private float money;
    public float Money => money;
    
    [SerializeField] private float gold;
    public float Gold => gold;
    
    [SerializeField] private float totalDriftPoints;
    public float TotalDriftPoints => totalDriftPoints;

    [SerializeField] private int level;
    public int Level => level;

    [SerializeField] private List<int> purchasedCars = new List<int>();
    public List<int> PurchasedCars => purchasedCars;
    public event Action<float> OnMoneyChanged;
    public event Action<float> OnGoldChanged;

    public PlayerStats() { }

    public PlayerStats(string playerName, float initialMoney, int initialLevel, float gold)
    {
        this.playerName = playerName;
        this.money = initialMoney;
        this.gold = gold;
        totalDriftPoints = 0f;
        level = initialLevel;
        purchasedCars = new List<int>();
    }
    
    public void AddMoney(float amount)
    {
        Debug.Log($"Adding money {amount}");
        money += amount;
        OnMoneyChanged?.Invoke(Money);
    }
    
    public void AddGold(float amount)
    {
        Debug.Log($"Adding gold {amount}");
        gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }

    public void AddDriftPoints(float amount)
    {
        totalDriftPoints += amount;
    }

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
