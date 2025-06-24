using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg : MonoBehaviour
{
   [SerializeField] private int  m_oncePer;
    void Start() {
        int o = Random.Range(0, m_oncePer);
        Debug.Log(o);
        if (o == 1) {
            gameObject.SetActive(true);
        } else {
            gameObject.SetActive(false);
        }
    }

   
}
