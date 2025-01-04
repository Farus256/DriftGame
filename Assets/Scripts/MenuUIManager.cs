using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private CarInitializer carInitializer;

    public void OnPlayButton()
    {
        // Получаем текущую машину
        CarStats chosenCar = carInitializer.GetCurrentCarStats();
        // Можно запомнить это в GameData.SelectedCar, 
        // или передать напрямую в GameSceneManager.
        
        // Переходим в следующую сцену (игровую)
        SceneManager.LoadScene("Game");
    }

    public void OnNextCarButton()
    {
        carInitializer.ShowNextCar();
    }

    public void OnPreviousCarButton()
    {
        carInitializer.ShowPreviousCar();
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}