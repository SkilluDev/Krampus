using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeTrigger : MonoBehaviour
{
    public GameObject popUp;
    bool activbated = false;


    private void OnTriggerEnter(Collider other)
    {
        if (activbated) return;
        if (other.gameObject.tag == "Player")
        {
            Instantiate(popUp);

            this.enabled = false;
            activbated = true;
        }
    }
}
