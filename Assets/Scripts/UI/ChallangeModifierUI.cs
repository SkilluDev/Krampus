using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChallangeModifierUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI m_descriptionText;
    [SerializeField] private TextMeshProUGUI m_titleText;



    public void SetInfo(string title, string desc) {
        m_titleText.text = title;
        m_descriptionText.text = desc;
     }

    public void Select() {
        this.GetComponent<Outline>().enabled = true;
    }

    public void Deselect() {
        this.GetComponent<Outline>().enabled = false;
     }
}
