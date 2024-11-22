using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsScirpt : MonoBehaviour
{
    Animator animator;
    bool isOpen = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOpen) { return; }
        if (other.tag == "Player" || other.tag == "Child" || other.tag == "Parent") 
        {

            Vector3 direcion = (other.transform.position - transform.position).normalized;
            OpenDoor(direcion);
            
        }
    }

    void OpenDoor(Vector3 direction) 
    {

        if (direction.x < 0)
        {
            animator.SetTrigger("Open");
        } else { animator.SetTrigger("Open2"); }
        isOpen = true;
    }
}
