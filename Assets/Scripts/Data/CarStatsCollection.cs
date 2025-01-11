using UnityEngine;

[System.Serializable]
public class CarStatsCollection
{
    [SerializeField] private CarStats[] cars;
    public CarStats[] Cars => cars;

    public void SetCars(CarStats[] newCars)
    {
        cars = newCars;
    }
}