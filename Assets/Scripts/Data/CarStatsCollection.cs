using UnityEngine;

[System.Serializable]
public class CarStatsCollection
{
    [SerializeField] private CarStats[] cars;
    public CarStats[] Cars => cars;

    // Метод для установки массива машин
    public void SetCars(CarStats[] newCars)
    {
        cars = newCars;
    }
}