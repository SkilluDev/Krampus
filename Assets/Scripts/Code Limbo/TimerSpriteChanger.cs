using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerSpriteChanger : MonoBehaviour {
    [SerializeField] private int m_decimalPlace;
    [SerializeField] private Sprite[] m_spriteSheet;
    [SerializeField] private Image m_image;

    [SerializeField] private Color m_normalColor = new Color(0x1B,0xC0,0x00);
    [SerializeField] private Color m_goodColor = new Color(0xFF,0xFF,0xFF);
    [SerializeField] private Color m_badColor = new Color(0xC0,0x00,0x09);
    private Sprite currentSprite;

    void Update() {
	    var index = (int)(Game.MainGameInfo.Timer.GameTime / m_decimalPlace) % 10;
        currentSprite = m_spriteSheet[index<0?0:index];
        m_image.sprite = currentSprite;
    }
}
