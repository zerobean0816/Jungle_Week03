using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    

    public void GameStart()
    {
        SceneManager.LoadScene("Main");
    }

    public void ToTurtorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void ToQUit()
    {
        Application.Quit();
    }
}
