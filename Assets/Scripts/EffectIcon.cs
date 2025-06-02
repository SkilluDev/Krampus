using System.Collections;
using System.Collections.Generic;
using LitMotion;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using LitMotion.Extensions;

public class EffectIcon : MonoBehaviour {

    [SerializeField] private Image m_icon;
    [SerializeField] private Image m_fillImage;

    [SerializeField] private TextMeshProUGUI m_stackText;




    public void SetIcon(Sprite icon = null) {
        m_icon.sprite = icon;
        m_stackText.text = "";
     }

    public void SetIcon(Sprite icon, float duration) {

        SetIcon(icon);
        LMotion.Create(0, 1.0f, duration).WithOnComplete(() => Destroy(gameObject)).BindToFillAmount(m_fillImage);

        m_stackText.text = "";

    }

}
