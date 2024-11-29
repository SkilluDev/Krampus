using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class ChildDetection : MonoBehaviour
{

    [SerializeField] GameObject arrowMesh;


   

   [SerializeField] bool isActive = false;
    [SerializeField] float timeActivation = 10f;


    Animator animator;




    private void Start()
    {
        isActive = false;
        arrowMesh.SetActive(false);

        animator = arrowMesh.GetComponentInChildren<Animator>();
    }

    private void Update()
    {

        

        if (isActive)
        {
            if (WinCondition.Instance.getTimer() >= timeActivation)
            {
                isActive = false;
                arrowMesh.SetActive(false);
                return;
            }

            Vector3 direction = (GetRandomChild().position - transform.position).normalized;

            Quaternion rot = Quaternion.LookRotation(direction);

            arrowMesh.transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
        }
        else 
        {
            if (WinCondition.Instance.getTimer() < timeActivation)
            {
                ActivateDetection();
            }
        }
    }

    void ActivateDetection() 
    {
        
        if(GetRandomChild() != null) 
        {
            isActive = true;
           
            arrowMesh.SetActive(true);


            animator.SetTrigger("App");
        }
    }

    Transform GetRandomChild() 
    {

        List<GameObject> kids = GameObject.FindGameObjectsWithTag("Child").ToList<GameObject>();

        int o = 0;
        for (int i = 1; i < kids.Count; i++) 
        {
            if (kids[i].GetComponent<Child>().isBad==false) 
            {
                kids.RemoveAt(i);
                i--;
                continue;
            }
            if (Vector3.Distance(kids[o].transform.position, transform.position) > (Vector3.Distance(kids[i].transform.position, transform.position)))
            {

                o = i;

            }


            
        }
      

        if(kids.Count == 0) 
        {
            return null;
        }

      

      



        return kids[o].transform;


        
    }
}
