using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private MenuCarSpawner menuCarSpawner;

    public void OnPlayButton()
    {
        CarStats chosenCar = menuCarSpawner.GetCurrentCarStats();
        
        if (chosenCar == null)
        {
            Debug.LogWarning("[MenuUIManager] No car selected!");
            return;
        }
        
        CarSelection.selectedCarId = chosenCar.id;
        
        SceneManager.LoadScene("TestScene");
    }

    public void OnNextCarButton()
    {
        menuCarSpawner.ShowNextCar();
    }

    public void OnPreviousCarButton()
    {
        menuCarSpawner.ShowPreviousCar();
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}