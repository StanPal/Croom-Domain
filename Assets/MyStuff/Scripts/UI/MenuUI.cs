using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{   
    
    public void GoToSinglePlayer()
    {
        SceneManager.LoadScene("Battle");
    }

    public void GoToCoOpMode()
    {
        SceneManager.LoadScene("CoOpMode");
    }

    public void GoToCharacter()
    {
        SceneManager.LoadScene("Character");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
}
