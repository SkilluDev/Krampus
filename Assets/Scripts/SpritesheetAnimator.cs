using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSheetAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] m_spriteSheet;
    [SerializeField] private Image m_image;

    [SerializeField] private float m_fps;

    private float m_timer;

    private int m_counter;

	private void Update() {
        m_timer += Time.deltaTime;
        if (m_timer > 1/m_fps) {
            m_image.sprite = m_spriteSheet[m_counter++ % m_spriteSheet.Length];
            m_timer = 0;
        }

    }
}
