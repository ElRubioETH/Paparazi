using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void Starts()
    {
        SceneManager.LoadScene(1);
    }
    public void Options ()
    {

    }
    public void Quit()
    {
        Application.Quit();
    }
}
