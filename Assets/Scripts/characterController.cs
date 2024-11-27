using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterController : MonoBehaviour
{
    public Rigidbody rigidBody;
    private GameObject camera;

    public float speedMultiplier;
    public float runningMultiplier = 2;

    public bool isRunning;

    bool isDead = false;
    private Matrix4x4 matrix;

    private float xMovement;
    private float zMovement;
    public bool shouldKrampusMove = true;

    readonly float stepRunSpeed = 0.416f;
    readonly float stepWalkSpeed = 0.5f;
    readonly float stepIdleSpeed = float.MaxValue;

    readonly float windUpRunSpeed = 0.41f;
    readonly float windUpWalkSpeed = 0.5f;
    readonly float windUpIdleSpeed = 1.041f;

    float timerWindUp1;
    float timerWindUp2;
    
    float timerStep1;
    float timerStep2;
    
    float windUpSpeed;
    float stepSpeed;

    enum State
    {
        running,walking,idle
    }

    private State currentState = State.idle;
    private State previousState = State.idle;

    Animator animator;

    void Start()
    {
        timerWindUp1 = 0f;
        timerWindUp2 = -windUpIdleSpeed;
        
        timerStep1 = 0f;
        timerStep2 = 0f;
        
        rigidBody = GetComponent<Rigidbody>();

        animator = GetComponentInChildren<Animator>();

        camera = GameObject.FindWithTag("MainCamera");
        matrix = Matrix4x4.Rotate(Quaternion.Euler(0, camera.transform.localEulerAngles.y, 0));
    }

    void Update()
    {
        previousState = currentState;
        if (!WinCondition.Instance.isGamePausedValue())
        {
            xMovement = Input.GetAxis("Horizontal");
            zMovement = Input.GetAxis("Vertical");
            
            timerWindUp1 += Time.deltaTime;
            timerWindUp2 += Time.deltaTime;
            
            timerStep1 += Time.deltaTime;
            timerStep2 += Time.deltaTime;
            
            if (rigidBody.velocity.x == 0 && rigidBody.velocity.z == 0) 
            { 
                currentState = State.idle;
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    currentState = State.running;
                }
                else
                {
                    currentState = State.walking;
                }
            }

            switch (currentState)
            {
                case State.running:
                    isRunning = true;
                    
                    windUpSpeed = windUpRunSpeed;
                    stepSpeed = stepRunSpeed;
                    
                    break;
                case State.walking:
                    isRunning = false;
                    
                    windUpSpeed = windUpWalkSpeed;
                    stepSpeed = stepWalkSpeed;
                    
                    break;
                case State.idle:
                    isRunning = false;
                    
                    windUpSpeed = windUpIdleSpeed;
                    stepSpeed = stepIdleSpeed;
                    timerStep1 = 0;
                    timerStep2 = 0;
                    
                    break;
            }
            
            if (previousState != currentState)
            {
                timerWindUp1 = 0;
                timerWindUp2 = -windUpSpeed;
                timerStep1 = 0;
                timerStep2 = -stepSpeed;
            }
            
            //winding up sounds playing
            if (timerWindUp1 >= windUpSpeed)
            {
                SoundManager.PlaySound("windup1");
                timerWindUp1 = -windUpSpeed;
            }
            if (timerWindUp2 >= windUpSpeed)
            {
                SoundManager.PlaySound("windup2");
                timerWindUp2 = -windUpSpeed;
            }
            //steps sounds playing
            if (timerStep1 >= stepSpeed)
            {
                SoundManager.PlaySound("step1");
                timerStep1 = -stepSpeed;
            }
            if (timerStep2 >= stepSpeed)
            {
                SoundManager.PlaySound("step2");
                timerStep2 = -stepSpeed;
            }
            
            
        }
    }

    void FixedUpdate()
    {
        if (!WinCondition.Instance.isGamePausedValue())
        {
            Vector3 rawinput = new Vector3(xMovement, 0, zMovement).normalized * speedMultiplier *
                               (isRunning ? runningMultiplier : 1) * (shouldKrampusMove ? 1 : 0);
            Vector3 skewedInput = matrix.MultiplyPoint3x4(rawinput);

            rigidBody.velocity = skewedInput;
            animator.SetFloat("Speed", (skewedInput.magnitude / (speedMultiplier * runningMultiplier)));
        }
    }

    public void Die() 
    {
        if (!isDead)
        {
            animator.SetTrigger("Death");
            isDead = true;
            shouldKrampusMove = false;
        }
    }
}
