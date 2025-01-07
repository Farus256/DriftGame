[System.Serializable]
public class PlayerStats
{
    private string _playerName;
    private float _money;
    private float _totalDriftPoints;
    private int _level;
    // public int selectedCarIndex;
    // public int totalWins;
    public PlayerStats(string playerName, float initialMoney, int initialLevel)
    {
        _playerName = playerName;
        _money = initialMoney;
        _level = initialLevel;
        _totalDriftPoints = 0; // Начальные очки дрифта равны нулю
    }
    
    public float GetMoney()
    {
        return _money;
    }
    public void AddMoney(float amount)
    {
        _money += amount;
    }
    public void AddDriftPoints(float amount)
    {
        _totalDriftPoints += amount;
    }

    public string GetName()
    {
        return _playerName;
    }
}