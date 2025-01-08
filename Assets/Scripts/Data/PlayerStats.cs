using UnityEngine;

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
    
    // Если нужны дополнительные поля, раскомментируйте и добавьте их с [SerializeField]
    // [SerializeField]
    // private int selectedCarIndex;
    // public int SelectedCarIndex => selectedCarIndex;

    // [SerializeField]
    // private int totalWins;
    // public int TotalWins => totalWins;

    // Пустой конструктор для десериализации
    public PlayerStats() { }

    // Конструктор для инициализации
    public PlayerStats(string playerName, float initialMoney, int initialLevel)
    {
        this.playerName = playerName;
        this.money = initialMoney;
        this.totalDriftPoints = 0f; // Начальные очки дрифта равны нулю
        this.level = initialLevel;
    }
    
    public void AddMoney(float amount)
    {
        money += amount;
    }
    
    public void AddDriftPoints(float amount)
    {
        totalDriftPoints += amount;
    }
}