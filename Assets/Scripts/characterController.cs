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
    
    private Matrix4x4 matrix;

    private float xMovement;
    private float zMovement;
    public bool shouldKrampusMove = true;


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

        xMovement = Input.GetAxis("Horizontal");
        zMovement = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        else isRunning = false;

        

    }

    void FixedUpdate()
    {
        Vector3 rawinput = new Vector3(xMovement, 0, zMovement) * speedMultiplier * (isRunning ? runningMultiplier : 1)*(shouldKrampusMove?1:0);
        Vector3 skewedInput = matrix.MultiplyPoint3x4(rawinput);

        rigidBody.velocity = skewedInput;
        Debug.Log(skewedInput.magnitude);
        animator.SetFloat("Speed", (skewedInput.magnitude/(speedMultiplier*runningMultiplier)));
    }
}
