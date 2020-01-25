using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayersAliveController : NetworkBehaviour
{
    // Variables
    private List<PlayerController> pc;
    private GameObject[] players;
    private int tempNumberPlayers = 0;
    private int numberPlayers = 0;
    private bool isWinner = false;
    static private string winnersName = "";

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Find gameobjects with tag
        players = GameObject.FindGameObjectsWithTag("Player");

        // check if there are any new players
        if (players.Length != tempNumberPlayers)
        {
            // make new lists
            pc = new List<PlayerController>();
            numberPlayers = 0;
            // loop through players
            for (int p = 0; p < players.Length; p++)
            {
                //add to lists
                pc.Add(new PlayerController());
                numberPlayers++;
            }
            // set numbers of players into temp for checking
            tempNumberPlayers = players.Length;
        }

        // If there are more than one player
        if (players.Length > 1)
        {
            for (int i = 0; i < numberPlayers; i++)
            {
                pc[i] = players[i].GetComponent<PlayerController>();

                // If there is only one player alive
                if (numberPlayers == 1)
                {
                    isWinner = true;
                    winnersName = pc[i].name;
                    RpcSendWinner(pc[i].name);
                }

                //Check if player is alive
                if (pc[i].isAlive == false)
                {
                    pc.RemoveAt(i);
                    numberPlayers--;
                }
            }
        }

        // if current scene is WinLoseScene
        if (SceneManager.GetActiveScene().name == "WinLoseScene")
        {
            // get player prefs
            string wn = PlayerPrefs.GetString("win");
            // set winners name
            GameObject.Find("TextWinnersName").GetComponent<Text>().text = wn + " is the winner";
        }
    }

    [ClientRpc]
    private void RpcSendWinner(string winName)
    {
        // save currnet winner
        PlayerPrefs.SetString("win", winName);
        // load winLoseScene
        SceneManager.LoadScene("WinLoseScene");
    }

}


