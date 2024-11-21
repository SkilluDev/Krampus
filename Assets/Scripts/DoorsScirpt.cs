using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsScirpt : MonoBehaviour
{
    Animator animator;


    private void Start()
    {
        animator = GetComponent<Animator>();

        animator.SetTrigger("Open");
    }
}
