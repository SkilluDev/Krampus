using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;

public class detection : MonoBehaviour
{
    private GameObject krampus;
    public Vector3 krampusEncounerPosition;
    characterController krampusController;

    public bool isAlerted = false;
    public bool chaseDetection = false;
    
    public float alertDistance = 30;
    public float runningMultiplier = 2;
    float currentDist = 999;
    
    public GameObject krampusLose;
    private bool lastStateAlerted = false;

    private NavMeshAgent navMeshAgent;

    private 
    void Start()
    {
        krampus = GameObject.FindWithTag("Player");
        krampusController = krampus.GetComponent<characterController>();
        
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    private void Update()
    {
        currentDist = Vector3.SqrMagnitude(krampus.transform.position - gameObject.transform.position);
        if (isAlerted != lastStateAlerted)
        {
            Debug.Log("Changed state from "+lastStateAlerted+" to "+isAlerted);
            lastStateAlerted = isAlerted;
        }
    }

    void FixedUpdate()
    {

        if (currentDist <= alertDistance || ((krampusController.isRunning) && currentDist <= alertDistance * runningMultiplier))
        {
            Vector3 krampusPosition = krampus.transform.position;
            if (!Physics.Raycast(transform.position,(krampusPosition - transform.position).normalized,alertDistance*runningMultiplier,1<<6)) { 
                isAlerted = true;
                if (isAlerted && gameObject.tag == "Parent")
                {
                    if(!chaseDetection)WinCondition.Instance.GameOver(WinCondition.LostGameCase.DetectedByParents);
                    else navMeshAgent.SetDestination(krampusPosition);
                    
                }
                krampusEncounerPosition = krampusPosition;
            }
            

        }
        
    }

    
}
