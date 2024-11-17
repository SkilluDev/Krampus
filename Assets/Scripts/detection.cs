using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class detection : MonoBehaviour
{
    private GameObject krampus;
    public bool isAlerted = false;
    public Vector3 krampusEncounerPosition;
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
            Vector3 krampusPosition = krampus.transform.position;
            if (!Physics.Raycast(transform.position,(krampusPosition - transform.position).normalized,alertDistance*runningMultiplier,1<<6)) { 
                isAlerted = true;
                if (isAlerted && gameObject.tag == "Parent") { SceneManager.LoadScene("UI Test"); }
                krampusEncounerPosition = krampusPosition;
            }
            

        }

    }
}
