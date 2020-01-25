using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager_Custom : NetworkManager
{
    // Controlling UI Buttons for joining, hosting, disconnecting from game

    public void startUpHost()
    {
        // reset network if thing go wrong
        NetworkServer.Reset();
        // set ip address and port
        setIPAddress();
        setPort();
        // start host
        NetworkManager.singleton.StartHost();
    }

    public void joinGame()
    {
        // set ip address and port
        setIPAddress();
        setPort();
        // connect to host
        NetworkManager.singleton.StartClient();
    }

    // set ip address from input field
    void setIPAddress()
    {
        string ipAddress = GameObject.Find("inputFieldIPAddress").GetComponent<InputField>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    // set port
    void setPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }

    // how to play button
    public void setHowToPlay()
    {
        GameObject.Find("TextHowToPlay").GetComponent<Text>().enabled = true;
        GameObject.Find("ButtonHowToPlay").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonHowToPlay").GetComponent<Button>().onClick.AddListener(removeHowToPlay);
    }

    // remove how to play 
    public void removeHowToPlay()
    {
        GameObject.Find("TextHowToPlay").GetComponent<Text>().enabled = false;
        GameObject.Find("ButtonHowToPlay").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonHowToPlay").GetComponent<Button>().onClick.AddListener(setHowToPlay);
    }

    // main menu button after win/lose
    public void mainMenu()
    {
        NetworkManager.singleton.StopHost();
        SceneManager.LoadScene("MenuScene");
    }

    // finding current scene
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // check current scene
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // if scene main menu
        if (scene.name == "MenuScene")
        {
            //setupMenuSceneButtons();
            // load button listners
            StartCoroutine("setupMenuSceneButtons");
        }
        else if (scene.name == "WinLoseScene") // if win/lose scene 
        {
            // load button listners
            setupOtherSceneButtons();
        }
    }

    IEnumerable setupMenuSceneButtons()
    {
        // remove and add listners to buttons
        GameObject.Find("TextHowToPlay").GetComponent<Text>().enabled = false;
        yield return new WaitForSeconds(.3f);
        GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.AddListener(startUpHost);

        GameObject.Find("ButtonJoinGame").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonJoinGame").GetComponent<Button>().onClick.AddListener(joinGame);

        GameObject.Find("ButtonHowToPlay").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonHowToPlay").GetComponent<Button>().onClick.AddListener(setHowToPlay);
        StopCoroutine("setupMenuSceneButtons");
    }

    void setupOtherSceneButtons()
    {
        // remove and add listner to buttons
        GameObject.Find("ButtonMainMenu").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonMainMenu").GetComponent<Button>().onClick.AddListener(mainMenu);
    }
}
