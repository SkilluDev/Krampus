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
        currentSprite = m_spriteSheet[(int)(Game.MainGameInfo.Timer.GameTime / m_decimalPlace) % 9];
        m_image.sprite = currentSprite;
    }
}
