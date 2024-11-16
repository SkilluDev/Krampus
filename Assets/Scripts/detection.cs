using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class detection : MonoBehaviour
{
    private GameObject krampus;
    public bool isAlerted = false;
    public float alertDistance = 30;
    public float runningMultiplier = 2;
    float currentDist = 999;
    characterController krampusController;

    void Start()
    {
        krampus = GameObject.FindWithTag("Player");
        krampusController = krampus.GetComponent<characterController>();
        
    }

    private void Update()
    {
        currentDist = Vector3.SqrMagnitude(krampus.transform.position - gameObject.transform.position);
        
    }

    void FixedUpdate()
    {

        if (currentDist <= alertDistance || ((krampusController.isRunning) && currentDist <= alertDistance * runningMultiplier))
        {
            if (!Physics.Raycast(transform.position,(krampus.transform.position - transform.position).normalized,alertDistance*runningMultiplier,1<<6)) { 
                isAlerted = true;
                
            }
            

        }

    }
}
