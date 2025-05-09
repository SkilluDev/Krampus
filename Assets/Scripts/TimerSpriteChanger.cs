using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerSpriteChanger : MonoBehaviour {
    [SerializeField] private int m_decimalPlace;
    [SerializeField] private Sprite[] m_spriteSheet;
    [SerializeField] private Image m_image;
    private Sprite currentSprite;

    void Update() {
	    var index = (int)(Game.MainGameInfo.Timer.GameTime / m_decimalPlace) % 10;
        currentSprite = m_spriteSheet[index<0?0:index];
        m_image.sprite = currentSprite;
    }
}
