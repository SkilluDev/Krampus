using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    float time = 3f;
    void Start()
    {
        Destroy(gameObject,time);
    }

    
    
}
