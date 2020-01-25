using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProjectileController : NetworkBehaviour
{
    public string whoShot;
    // Use this for initialization
    void Start() { }

    // Update is called once per frame
    void Update() { }

    void OnBecameInvisible()
    {
        //if bullet goes off screen
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasAuthority == false)
        {
            return;
        }
        //if bullet hits enemy bullet
        if (collision.gameObject.tag == "floor")
        {
            Destroy(this.gameObject);
        }

        //Check if this is this person and who it hit
        //Player Shot
        if (collision.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
            // get collision of gameobjects name
            string uidentity = collision.gameObject.name;

            //Tell server who was shot
            CmdWhoWasShot(uidentity, 1);
        }
    }

    [Command]
    void CmdWhoWasShot(string uidentity, int dmg)
    {
        // get gameobject name of who was shot
        GameObject go = GameObject.Find(uidentity);
        // call their controller and deduct health
        go.GetComponent<PlayerController>().deductHealth(dmg);
    }
}
