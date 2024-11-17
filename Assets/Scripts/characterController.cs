using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterController : MonoBehaviour
{
    public Rigidbody rigidBody;
    private GameObject camera;
    [SerializeField] private WinCondition winCondition;
    
    public float speedMultiplier;
    public float runningMultiplier = 2;

    public bool isRunning;
    
    private Matrix4x4 matrix;

    private float xMovement;
    private float zMovement;
    public bool shouldKrampusMove = true;

    float stepWaitTime = 0.2f;
    float runWaitTime = 0.1f;

    float timer;
    float timer2;
    float timer3;

    Animator animator;

    void Start() 
    {
         rigidBody = GetComponent<Rigidbody>();

        animator = GetComponentInChildren<Animator>();

        camera = GameObject.FindWithTag("MainCamera");
         matrix = Matrix4x4.Rotate(Quaternion.Euler(0, camera.transform.localEulerAngles.y, 0));
    }

    void Update()
    {
        if (!winCondition.isGamePausedValue())
        {
            xMovement = Input.GetAxis("Horizontal");
            zMovement = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.LeftShift))
            {
                isRunning = true;
            }
            else isRunning = false;

            if (rigidBody.velocity.x != 0 || rigidBody.velocity.y != 0)
            {
                if (!isRunning)
                {
                    timer += Time.deltaTime;
                    timer2 += Time.deltaTime;

                    if (timer >= 0.541f)
                    {
                        SoundManager.PlaySound("windup2");
                        timer = 0;
                    }

                    /*   if (timer2 >= 0.541f * 2)
                       {
                           SoundManager.PlaySound("step2");
                           timer2 = 0;
                       }*/

                }
                else
                {
                    timer3 += Time.deltaTime;
                    timer += Time.deltaTime;
                    if (timer >= 0.43f)
                    {
                        SoundManager.PlaySound("windup2");
                        timer = 0;
                    }
                    /*  if (timer3 >= 0.86f)
                      {
                          SoundManager.PlaySound("step2");
                          timer3 = 0;
                      }*/
                }
            }
            else
            {
                /* timer3 = 0.43f;
                 timer2 = 0.541f; */
                timer = 0;
            }
        }
    }

    void FixedUpdate()
    {
        if (!winCondition.isGamePausedValue())
        {
            Vector3 rawinput = new Vector3(xMovement, 0, zMovement) * speedMultiplier *
                               (isRunning ? runningMultiplier : 1) * (shouldKrampusMove ? 1 : 0);
            Vector3 skewedInput = matrix.MultiplyPoint3x4(rawinput);

<<<<<<< Updated upstream
        rigidBody.velocity = skewedInput;
//        Debug.Log(skewedInput.magnitude);
        animator.SetFloat("Speed", (skewedInput.magnitude / (speedMultiplier * runningMultiplier)));
=======
            rigidBody.velocity = skewedInput;
            Debug.Log(skewedInput.magnitude);
            animator.SetFloat("Speed", (skewedInput.magnitude / (speedMultiplier * runningMultiplier)));
        }
>>>>>>> Stashed changes
    }
}
