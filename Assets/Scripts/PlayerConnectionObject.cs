using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour
{
    // variables
    public GameObject PlayerUnitPrefab;
    //private List<GameObject> spawnPoints;

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer == false)
        {
            // Object belong to another player
            return;
        }

        // Instantiate() create object on local computer
        //Instantiate(PlayerUnitPrefab);
        CmdSpanwMyUnit();
    }

    // Update is called once per frame
    void Update() { }

    //***** COMMANDS *****//

    [Command]
    void CmdSpanwMyUnit()
    {
        // get and make player prefab
        GameObject go = Instantiate(PlayerUnitPrefab);

        // set authority
        go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);

        // spawn on server with authority
        //NetworkServer.Spawn(go);
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }
}
