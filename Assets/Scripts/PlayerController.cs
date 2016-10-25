﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    //Variables
    public float            maxSpeed    = 0;
    public float            movSpeed    = 0;
    public float            jumpPower   = 0;
    public bool             isGrounded;

    //Components
    public  Rigidbody       myRigidbody;
    private Animator        myAnimator;
    private TrailRenderer   myTrail;
    private PlayerHandler   playerHandler;

    //Models
    public GameObject       modelDagger;
    public GameObject       modelSlash;
    public GameObject       shieldPuPrefab;

    //Controls
    public KeyCode          jump;
    public KeyCode          slide;
    public KeyCode          usePowerUp;

    //power-ups
    public GameObject       darkBallPrefab;
    public GameObject       dropObstaclePrefab;

    //Power-ups audio
    private AudioSource[]   powerAudios;
    private AudioSource     darkballSfx;
    private AudioSource     dropObstacleSfx;
    private AudioSource     shieldSfx;
    private AudioSource     speedSfx;

    //Start
    void Start()
    {
        //init components/gameobjects
        myRigidbody     = GetComponent<Rigidbody>();
        myAnimator      = GetComponentInChildren<Animator>();
        myTrail         = GetComponent<TrailRenderer>();
        playerHandler   = GetComponent<PlayerHandler>();
        powerAudios     = GameObject.Find("PlayerSFX").GetComponents<AudioSource>();

        //init audio clips
        darkballSfx         = powerAudios[0];
        dropObstacleSfx     = powerAudios[1];
        shieldSfx           = powerAudios[2];
        speedSfx            = powerAudios[3];

        //Disable models
        modelDagger.GetComponent<MeshRenderer>().enabled    = false;
        modelSlash.GetComponent<MeshRenderer>().enabled     = false;

        //Init variables
        movSpeed                = maxSpeed;
        playerHandler.myRole    = PlayerHandler.Role.Runner;
        playerHandler.myPowerUp = PlayerHandler.PowerUp_State.None;
    }
    
    //Update
    void Update()
    {
        //Cheating();
        Controls();
    }

    //Controls
    void Controls()
    {
        //Auto-Run
        myRigidbody.velocity = new Vector3(1 * movSpeed, myRigidbody.velocity.y);

        if (isGrounded)
        {
            //Jump
            if (Input.GetKeyDown(jump))
            {
                myAnimator.SetTrigger("Jump");
                myRigidbody.velocity += Vector3.up * jumpPower;
            }
        }

        //Slide
        if (Input.GetKeyDown(slide) && !myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Sliding"))
        {
            myAnimator.SetTrigger("Slide");
            myAnimator.SetBool("isSliding", true);
            StartCoroutine(slideCoroutine());
        }
        //Slide
        if (Input.GetKeyUp(slide))
        {
            myAnimator.SetBool("isSliding", false);
        }
        //Use powerup
        if (Input.GetKeyDown(usePowerUp))
        {
            switch (playerHandler.myPowerUp)
            {
                case PlayerHandler.PowerUp_State.Darkball:
                    {
                        darkballSfx.Play();
                        PowerUpDarkBall();
                        break;
                    }
                case PlayerHandler.PowerUp_State.DropObstacle:
                    {
                        dropObstacleSfx.Play();
                        PowerUpDropObstacle();
                        break;
                    }
                case PlayerHandler.PowerUp_State.Shield:
                    {
                        shieldSfx.Play();
                        StartCoroutine(PowerUpShield());
                        break;
                    }
                case PlayerHandler.PowerUp_State.Speed:
                    {
                        speedSfx.Play();
                        StartCoroutine(PowerUpSpeed());
                        break;
                    }
            }
        }
    }

    //Ground checks
    void OnCollisionStay(Collision col)
    {
        //Check if grounded
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
        
    }

    //POWER UPS///////////////////////////////////////////////////////
    //drop obs
    void PowerUpDropObstacle()
    {
        Instantiate(dropObstaclePrefab, new Vector3(transform.position.x - 2.5f, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        playerHandler.myPowerUp = PlayerHandler.PowerUp_State.None;
    }
    void PowerUpDarkBall()
    {
        Instantiate(darkBallPrefab, new Vector3(transform.position.x + 3f, transform.position.y + 1.5f, transform.position.z), Quaternion.identity);
        playerHandler.myPowerUp = PlayerHandler.PowerUp_State.None;
    }
    
    //slide(delay)
    IEnumerator slideCoroutine()
    {
        movSpeed -= 2f;
        yield return new WaitForSeconds(1);
        movSpeed += 2f;
    }

    //Shield
    IEnumerator PowerUpShield()
    {
        playerHandler.isShielded = true;
        yield return new WaitForSeconds(2f);
        playerHandler.isShielded = false;
        playerHandler.myPowerUp = PlayerHandler.PowerUp_State.None;
    }

    //Speed
    IEnumerator PowerUpSpeed()
    {
        float prevSpeed = movSpeed;
        movSpeed += 5f;
        yield return new WaitForSeconds(2f);
        movSpeed = prevSpeed;
        playerHandler.myPowerUp = PlayerHandler.PowerUp_State.None;
    }
}