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
     }

    public void SetIcon(Sprite icon, float duration) {

        SetIcon(icon);
        LMotion.Create(1.0f,0,(float)duration).WithDelay(0.7f).BindToFillAmount(m_fillImage);

    }

}
