using System;
using NaughtyAttributes;
using UnityEngine;

public class StatusSprite : MonoBehaviour {
    [SerializeField] private SpriteRenderer m_рендерер;
    [SerializeField] private Animator m_аниматор;
    [SerializeField][AnimatorParam(nameof(m_аниматор))] private int m_стартПарам, m_корторый;
    private Sprite m_настоящий;

    private void Awake() {
        ClearSprite();
    }

    public void SetSprite(Sprite sprite, int which = 1) {
		if (m_рендерер == null) return;
        if (sprite == m_настоящий) {
			return;
		}
        m_настоящий = sprite;
        m_рендерер.sprite = m_настоящий;
        m_аниматор.SetTrigger(m_стартПарам);
        m_аниматор.SetInteger(m_корторый, which);
        m_рендерер.enabled = true;
    }

    public void ClearSprite() {
        m_настоящий = null;
        m_рендерер.enabled = false;
    }

    private void Update() {
        transform.forward = Game.MainGameInfo.Krampus.Kamera.transform.forward;
    }
}
