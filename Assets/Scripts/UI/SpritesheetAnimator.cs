using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;


public class SpriteSheetAnimator : MonoBehaviour {
    public enum ColorChange {
        None,
        NiceChild,
        NaughtyChild
    }
    [SerializeField] private Sprite[] m_spriteSheet;
    [SerializeField] private Image m_image;

    [SerializeField] private float m_fps;

    [SerializeField] private ColorChange m_colorChange = ColorChange.None;

    private Color[] m_colors;

    private float m_frameTimer;
    private float m_colorTimer;

    private int m_counter;
    private int m_colorCounter;

    private void OnEnable() {
        
        switch (m_colorChange) {
            case ColorChange.NiceChild:
                m_colors = new Color[] { Game.MainGameInfo.NiceChildType.color };
                break;
            case ColorChange.NaughtyChild:
                m_colors = Game.MainGameInfo.NaughtyChildTypes.Select((c) => c.color).ToArray();
                break;
            case ColorChange.None:
                return;
        }
        m_image.color = m_colors[0];
        m_image.sprite = m_spriteSheet[0];
    }

    private void Update() {
        m_frameTimer += Time.deltaTime;
        if (m_frameTimer > 1 / m_fps) {
            m_image.sprite = m_spriteSheet[(++m_counter) % m_spriteSheet.Length];
            m_frameTimer = 0;
        }

        if (m_colorChange!=ColorChange.NaughtyChild) return;

        m_colorTimer += Time.deltaTime/2;
        if (m_colorTimer > 1 / m_fps) {
            Debug.Log("dlugosc" + m_colors.Length);
            m_image.color = m_colors[(m_colorCounter++) % m_colors.Length];
            m_colorTimer = 0;
        }

    }
}
