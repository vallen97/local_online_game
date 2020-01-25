using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    //Start the game button
    public void startButtonClicked()
    {
        //load scene
        SceneManager.LoadScene("Scene1");
    }

    //Get back to main menu
    public void mainmenuButtonClicked()
    {
        SceneManager.LoadScene("MenuScene");
    }

    //Quit the Game
    public void quitButtonClicked()
    {
        //quit the game
        Application.Quit();
    }
}
