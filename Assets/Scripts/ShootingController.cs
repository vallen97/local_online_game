using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class ShootingController : NetworkBehaviour
{
    //Variables
    //bullet speed
    public float speed = 10.0f;
    //bullet spawnPoint
    public Transform bulletSpawnPoint;
    //able to shoot
    private bool ableShoot;
    //time between shots
    private float shootInterval = .7f;
    //check if facing right
    public bool faceRight;
    //Player Controller
    [SerializeField]
    private PlayerController pc;
    //Check if pc or android is shooting
    private bool pcShootDown;
    private bool andShootDown;
    //check device
    private bool isAndroid;
    private bool isPC;


    // Use this for initialization
    void Start()
    {
        if (hasAuthority == false)
        {
            return;
        }

        //set facing right to true
        faceRight = true;
        //set able to shoot
        ableShoot = true;
        //get Player controller from player gameObject
        pc = GetComponent<PlayerController>();

        //set if android or pc
        isAndroid = pc.isAndroid;
        isPC = pc.isPC;

        //if not pc make event trigger 
        if (!isPC)
        {
            EventTrigger trigger = GameObject.Find("ButtonShoot").GetComponent<EventTrigger>();
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
            trigger.triggers.Add(entryDown);

            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => { OnPointerUpDelegate((PointerEventData)data); });
            trigger.triggers.Add(entryUp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority == false)
        {
            return;
        }
        // get player controller
        pc = GetComponent<PlayerController>();

        //set if facing right
        faceRight = pc.charFaceRight;

        // user is using a PC
        if (isPC)
        {
            // look for input
            pcShootDown = Input.GetKey(KeyCode.Mouse0);
        }

        //mouse right click or android button pressed
        if (pcShootDown || andShootDown)
        {
            //if able to shoot
            if (ableShoot)
            {
                // tell client to bullet spawn position, facing direction, and player name
                CltSpawnProjectile(bulletSpawnPoint.position, faceRight, this.name);

                //set able to shoot to false
                ableShoot = false;
                //start coroutine
                StartCoroutine("Shoot");
            }
        }
    }

    IEnumerator Shoot()
    {
        //wait shootinterval seconds before setting able to shoot to truw
        yield return new WaitForSecondsRealtime(shootInterval);
        ableShoot = true;
        //stop coroutine 
        StopCoroutine("Shoot");
    }

    //UI button
    public void shootBullet(bool shootPressed)
    {
        andShootDown = shootPressed;
    }

    // event triggers
    public void OnPointerDownDelegate(PointerEventData data)
    {
        Debug.Log("OnPointerDownDelegate called.");
        shootBullet(true);
    }

    public void OnPointerUpDelegate(PointerEventData data)
    {
        Debug.Log("OnPointerDownDelegate called.");
        shootBullet(false);
    }


    [Client]
    public void CltSpawnProjectile(Vector2 pos, bool faceDir, string charName)
    {
        // tell server information
        CmdSpawnProjectile(pos, faceDir, charName);

    }

    [Command]
    void CmdSpawnProjectile(Vector2 pos, bool faceDir, string charName)
    {
        // make bullet
        GameObject bullet = Instantiate(Resources.Load("Projectile"), pos, Quaternion.identity) as GameObject;

        // if facing left
        if (faceDir == false)
        {
            //rotate bullet 180 degrees
            bullet.transform.Rotate(new Vector3(0, 180, 0));
            // move bullet left
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.left * 3.0f;
        }
        else
        {
            // move bullet right
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.right * 3.0f;
        }

        //set active to true
        bullet.SetActive(true);

        // tell server to spawn object
        NetworkServer.Spawn(bullet);

        // tell bullet player name
        bullet.GetComponent<ProjectileController>().whoShot = charName;
    }
}
