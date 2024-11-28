using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpImage : MonoBehaviour
{

    public GameObject[] panels;

    int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;

        foreach (GameObject p in panels) 
        {
            p.SetActive(false);
        }
        panels[counter].SetActive(true);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.G)) 
        {
         NextPanel();
        }
    }

    void NextPanel() 
    {
        panels[counter].gameObject.SetActive(false);
        if (counter >= panels.Length - 1)
        {
            PopupEnd();
        }
        panels[++counter].SetActive(true);

        
    }






    void PopupEnd() 
    {
        Time.timeScale =1;
        Destroy(this.gameObject);
    }
}
