using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    private void Awake()
    {
        // Инициализируем все основные данные один раз при запуске
        GameData.Initialize();

        // Переходим на сцену меню (или любую другую)
        SceneManager.LoadScene("Menu");
    }
}