using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpImage : MonoBehaviour {

    public GameObject[] panels;

    private int counter = 0;

    // Start is called before the first frame update
    private void Start() {
        Time.timeScale = 0;

        foreach (GameObject p in panels) {
            p.SetActive(false);
        }
        panels[counter].SetActive(true);
    }

    private void Update() {
        if (WinCondition.Instance.inputSubscribe.AdvanceInput) { //Input.GetKeyUp(KeyCode.G) //Old input
            NextPanel();
        }
    }

    private void NextPanel() {
        panels[counter].gameObject.SetActive(false);
        if (counter >= panels.Length - 1) {
            PopupEnd();
            return;
        }
        panels[++counter].SetActive(true);


    }






    private void PopupEnd() {
        Time.timeScale = 1;
        Destroy(this.gameObject);
    }
}
