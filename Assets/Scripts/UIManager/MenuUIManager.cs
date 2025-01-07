using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private MenuCarSpawner menuCarSpawner;
    [SerializeField] private TMP_Text moneyText;
    
    private void Start()
    {
        PlayerStats stats = PlayerDataManager.LoadPlayerStats();

        moneyText.text = stats.GetMoney().ToString();

    }
    
    public void OnPlayButton()
    {
        CarStats chosenCar = menuCarSpawner.GetCurrentCarStats();
        
        if (chosenCar == null)
        {
            Debug.LogWarning("[MenuUIManager] No car selected!");
            return;
        }
        
        CarSelection.selectedCarId = chosenCar.id;
        
        GlobalEventManager.TriggerLevelStart();
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