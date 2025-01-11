// CarSelection.cs

using UnityEngine;

public static class CarSelection
{
    public static int SelectedCarId { get; set; } = -1;

    public static void SelectCar(int carId)
    {
        SelectedCarId = carId;
        Debug.Log($"[CarSelection] SelectedCarId set to {SelectedCarId}");
    }
}