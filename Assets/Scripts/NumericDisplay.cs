using UnityEngine;
using UnityEngine.UI;

public class NumericDisplay : MonoBehaviour {
    [SerializeField] private Sprite[] m_spritesheet;

    [SerializeField] private Image[] m_integralPlaces;
    [SerializeField] private Image[] m_fractionalPlaces;

    private float m_value;

    public float Value {
        get => m_value;
        set => SetValue(value);
    }

    private void Awake() {
        SetValue(69.24f);
    }


    public void SetValue(float value) {
        int integral = Mathf.FloorToInt(value);
        int fractional = Mathf.RoundToInt((value - integral) * Mathf.Pow(10, m_fractionalPlaces.Length));

        if (integral >= Mathf.Pow(10, m_integralPlaces.Length)) {
            integral = Mathf.FloorToInt(Mathf.Pow(10, m_integralPlaces.Length) - 1);
            fractional = Mathf.FloorToInt(Mathf.Pow(10, m_fractionalPlaces.Length) - 1);
        }

        for (int i = 0; i < m_integralPlaces.Length; i++) {
            int lsd = integral % 10;
            m_integralPlaces[i].sprite = m_spritesheet[lsd];
            integral = ((integral - lsd) / 10);
        }

        for (int i = 0; i < m_fractionalPlaces.Length; i++) {
            int lsd = fractional % 10;
            m_fractionalPlaces[i].sprite = m_spritesheet[lsd];
            fractional = ((fractional - lsd) / 10);
        }
    }
}
