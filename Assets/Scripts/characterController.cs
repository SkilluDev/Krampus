using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Security;
using UnityEngine;

public class characterController : MonoBehaviour
{

    public Rigidbody rigidBody;
    public GameObject camera;
    public float speedMultiplier;
    public float runningMultiplier = 2;
    public bool isRunning;


    private float xMovement;
    private float zMovement;

    void Start() 
    {
         rigidBody = GetComponent<Rigidbody>();
         camera = GameObject.FindWithTag("MainCamera");
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
        Vector3 rawinput = new Vector3(xMovement, 0, zMovement) * speedMultiplier * (isRunning ? runningMultiplier : 1);
        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0,camera.transform.localRotation.y,0));
        Vector3 skewedInput = matrix.MultiplyPoint3x4(rawinput);
        Debug.Log()

        rigidBody.velocity = skewedInput;

    }
}
