using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeTrigger : MonoBehaviour
{
    public GameObject popUp;
    private bool activated = false;
    private GameObject UI;

    void Start(){
        UI = GameObject.Find("Canvas");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated || !UI.GetComponent<UIManager>().getShowTutorials()) return;
        if (other.gameObject.tag == "Player")
        {
            Instantiate(popUp);

            this.enabled = false;
            activated = true;
        }
    }
}
