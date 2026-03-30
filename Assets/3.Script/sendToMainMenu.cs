using UnityEngine;
using UnityEngine.SceneManagement;

public class sendToMainMenu : MonoBehaviour
{
    void OnDestroy()
    {
        SceneManager.LoadScene("Menu");
    }
}
