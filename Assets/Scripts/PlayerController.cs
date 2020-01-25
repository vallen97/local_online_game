using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class PlayerController : NetworkBehaviour
{
    //player variables
    public float maxSpeed = 10f;
    public float jumpSpeed = 450f;
    public bool charFaceRight;
    [SyncVar] public int health = 3;
    private bool grounded = false;
    private bool jump = false;
    private bool ableToJump = false;

    //rigidbody 
    private Rigidbody2D rb;

    //player var android
    private float andMove;
    private float gyroX = 0;
    private bool andSpacePressed;

    //player var pc 
    private float pcMove;
    private bool pcSpacePressed;

    //check if android
    public bool isAndroid;
    //ckeck if pc
    public bool isPC;

    //player alive
    public bool isAlive = true;

    // spawnpoints for server
    private NetworkStartPosition[] spawnPoints;

    // Use this for initialization
    void Start()
    {
        if (hasAuthority == false)
        {
            return;
        }

        //check if device is a Desktop
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            isPC = true;
        }
        else
        {
            //lock screen in Landscape
            Screen.orientation = ScreenOrientation.Landscape;
            isAndroid = true;
        }

        // get locations of spawnpoints
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();

        //set variables
        charFaceRight = true;

        if (isPC == true)
        {
            // Hide UI buttons
            GameObject.Find("ButtonJump").SetActive(false);
            GameObject.Find("ButtonShoot").SetActive(false);
        }
        else
        {
            // make event triggers
            EventTrigger trigger = GameObject.Find("ButtonJump").GetComponent<EventTrigger>();
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
            trigger.triggers.Add(entryDown);

            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => { OnPointerUpDelegate((PointerEventData)data); });
            trigger.triggers.Add(entryUp);
        }

        //get rigidbody
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority == false)
        {
            return;
        }

        // TODO: look into atributes under unity network
        // if(server.active){ move player }

        //Handles character direction
        if (Input.GetKeyDown(KeyCode.D) || gyroX < 0)
        {
            //set facing right to true
            charFaceRight = true;
            //Get position of character
            Vector3 spriteScale = transform.localScale;
            //if X on character is negative switch character to positive
            if (spriteScale.x < 0)
            {
                spriteScale.x *= -1;
                transform.localScale = spriteScale;
            }
        }

        //Handles idle to running backwards animation
        if (Input.GetKeyDown(KeyCode.A) || gyroX > 0)
        {
            //set facing right to false
            charFaceRight = false;
            //Get position on character
            Vector3 spriteScale = transform.localScale;
            //if X is positive switch to negative
            if (spriteScale.x > 0)
            {
                spriteScale.x *= -1;
                transform.localScale = spriteScale;
            }
        }
    }

    void FixedUpdate()
    {
        if (hasAuthority == false)
        {
            return;
        }

        //Get Input from user
        pcMove = Input.GetAxis("Horizontal");
        pcSpacePressed = Input.GetKeyDown(KeyCode.W);

        //check if player pressed space and is grounded
        if (pcSpacePressed && grounded || andSpacePressed && grounded)
        {
            //set space and jump to true
            ableToJump = true;
            jump = true;
        }

        if (isPC)
        {
            //move character
            rb.velocity = new Vector2(pcMove * maxSpeed, rb.velocity.y);
        }
        else if (isAndroid)
        {
            // use android gyroscope
            Input.gyro.enabled = true;
            // get gyroscope x
            gyroX = Input.gyro.rotationRateUnbiased.x;

            // move based on direction of x
            if (gyroX > -1 && gyroX < 1)
            {
                rb.velocity = new Vector2(gyroX * maxSpeed, rb.velocity.y);
            }
        }

        if (ableToJump && jump)
        {
            //add jump to character
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpSpeed);
            rb.AddForce(Vector2.up * jumpSpeed);
            ableToJump = false;
        }
    }

    public override void OnStartLocalPlayer()
    {
        //Camera.main.GetComponent<CameraController>().setTarget(gameObject.transform);

        // Player is always white 
        Color newColor = new Color(255, 255, 255, 1.0f);
        this.GetComponent<SpriteRenderer>().color = newColor;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //check for collision
        if (collision.gameObject.tag == "floor")
        {
            //touching floor set jump to true
            jump = true;
            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //check if player is not touching floor
        if (collision.gameObject.tag == "floor")
        {
            //set jump to false
            jump = false;
            grounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "respawn" || collision.gameObject.name == "Respawn")
        {
            RpcRespawn();
        }

        //if an enemy projectile hits the player
        if (collision.gameObject.tag == "enemyProjectile" || collision.gameObject.name == "enemyProjectile")
        {
            //destroy projectile and lose life
            Destroy(collision.gameObject);
        }
    }

    //UI buttons
    public void jumpChar(bool jumpY)
    {
        //set new jump speed
        jumpSpeed = 225f;
        andSpacePressed = jumpY;
    }

    // event triggers
    public void OnPointerDownDelegate(PointerEventData data)
    {
        Debug.Log("OnPointerDownDelegate called.");
        jumpChar(true);
    }

    public void OnPointerUpDelegate(PointerEventData data)
    {
        Debug.Log("OnPointerDownDelegate called.");
        jumpChar(false);
    }

    // deduct health
    public void deductHealth(int hlt)
    {
        if (!isServer)
        {
            return;
        }
        health -= hlt;

        // if health is 0 hide player and set isAlive to false
        if (health <= 0)
        {
            //Destroy(this.gameObject);
            this.gameObject.GetComponent<Renderer>().enabled = false;
            isAlive = false;
        }
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            // Set the spawn point to origin as a default value
            Vector3 spawnPoint = Vector3.zero;

            // If there is a spawn point array and the array is not empty, pick one at random
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            // Set the player’s position to the chosen spawn point
            transform.position = spawnPoint;
        }
    }
}
