using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour {
    [SerializeField] private Image m_itemIconImage;

    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private TextMeshProUGUI m_descriptionText;



    public void SetInfo(Sprite icon, string title, string description) {

        m_itemIconImage.sprite = icon;
        m_titleText.text = title;
        m_descriptionText.text = description;

     }
}
