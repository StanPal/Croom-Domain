using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    

}
