using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerIDController : NetworkBehaviour
{
    // variables
    [SyncVar] public string playerUniqueIdentity;
    private NetworkInstanceId playerNetID;
    private Transform myTransform;
    private PlayersAliveController pac;

    public override void OnStartLocalPlayer()
    {
        // get and set user names
        getNetIdentity();
        setIdentity();
    }

    private void Awake()
    {
        //get current player
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        // if player name is blank or player(clone)
        if (myTransform.name == "" || myTransform.name == "player(Clone)")
        {
            // set name
            setIdentity();
        }
    }

    [Client]
    void getNetIdentity()
    {
        // get id
        playerNetID = GetComponent<NetworkIdentity>().netId;
        // call MakeUniqueIdentity
        CmdTellServerMyIdentity(MakeUniqueIdentity());
    }

    void setIdentity()
    {
        // if player is your player
        if (isLocalPlayer)
        {
            // new name and tag
            myTransform.name = playerUniqueIdentity;
            myTransform.tag = "Player";
        }
        else
        {
            myTransform.name = MakeUniqueIdentity();
            myTransform.tag = "Player";
        }
    }

    string MakeUniqueIdentity()
    {
        //get id and add to Player and return new name
        string uniqueName = "Player " + playerNetID.ToString();
        return uniqueName;
    }

    [Command]
    void CmdTellServerMyIdentity(string name)
    {
        playerUniqueIdentity = name;
    }
}
