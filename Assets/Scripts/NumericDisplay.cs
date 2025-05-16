using System;
using System.Linq;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class NumericDisplay : MonoBehaviour {
    [SerializeField] private Sprite[] m_spritesheet;

    [SerializeField] private Image[] m_integralPlaces;
    [SerializeField] private Image[] m_fractionalPlaces;

    [SerializeField] private float m_value;
    [SerializeField] private Color m_color;

    [SerializeField] private float popUpTime = 15;
    [SerializeField] private Vector3 popUpSize  = new Vector3(0.02f, 0.02f);


    private int oldInt = 0;
    public float Value {
        get => m_value;
        set => SetValue(value);
    }

    public Color Color {
        get => m_color;
        set => SetColor(value);
    }

    public void SetColor(Color value) {
        foreach (var w in m_integralPlaces) {
            w.color = value;
        }
        foreach (var w in m_fractionalPlaces) {
            w.color = value;
        }
    }

    private void Awake() {
        SetValue(m_value);
        SetColor(m_color);
    }


    public void SetValue(float value) {
        if (value < 0) value = 0;

        int integral = Mathf.FloorToInt(value);
        int fractional = Mathf.RoundToInt((value - integral) * Mathf.Pow(10, m_fractionalPlaces.Length));

        if (integral >= Mathf.Pow(10, m_integralPlaces.Length)) {
            integral = Mathf.FloorToInt(Mathf.Pow(10, m_integralPlaces.Length) - 1);
            fractional = Mathf.FloorToInt(Mathf.Pow(10, m_fractionalPlaces.Length) - 1);
        }

        for (int i = 0; i < m_integralPlaces.Length; i++) {
            int lsd = integral % 10;
            if (i == 0 && lsd != oldInt && value < popUpTime) {
	            Popup();
	            oldInt = lsd;
            }

            m_integralPlaces[i].sprite = m_spritesheet[lsd];
            integral = ((integral - lsd) / 10);
        }

        if (value < popUpTime) {

        }

        for (int i = 0; i < m_fractionalPlaces.Length; i++) {
            int lsd = fractional % 10;
            m_fractionalPlaces[i].sprite = m_spritesheet[lsd];
            fractional = ((fractional - lsd) / 10);
        }
    }

    void Popup() {
	    for (int i = 0; i < m_integralPlaces.Length; i++) {

		    var img = m_integralPlaces[i];
		    var  digit = img.rectTransform;
		    var oldScale = digit.localScale;


		    LMotion.Create(oldScale, popUpSize, 0.2f).WithEase(Ease.OutElastic).WithOnComplete(
			    ()=>LMotion.Create( popUpSize, oldScale, 0.2f).WithEase(Ease.OutBounce).BindToLocalScale(digit)
		    ).BindToLocalScale(digit);
		    LMotion.Create(m_color, Color.red, 0.2f).WithEase(Ease.OutElastic).WithOnComplete(
			    ()=>LMotion.Create( Color.red, m_color, 0.2f).WithEase(Ease.OutBounce).BindToColor(img)
		    ).BindToColor(img);
	    }
    }
}
