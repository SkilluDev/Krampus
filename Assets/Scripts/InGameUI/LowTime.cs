using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX.Utility;

public class LowTime : MonoBehaviour {

    [BoxGroup("Sprite")][SerializeField] private RectTransform m_topKramp;
    [BoxGroup("Sprite")][SerializeField] private RectTransform m_bottomKramp;
    [BoxGroup("Time")][SerializeField] private float m_lowTime;
    [BoxGroup("Time")][SerializeField] private float m_jawSpeed;
    [BoxGroup("Time")][SerializeField] private float m_upwardMultiplier;
    private Vector2 m_jawMovementMultiplier;
    private Vector2 m_initialJawHeights;
    private Vector2 m_initialTopVector;
    private Vector2 m_initialBottomVector;
    private Vector2 m_wantedTopPos;
    private Vector2 m_wantedBottomPos;

    private void Awake() {
        m_initialTopVector = m_topKramp.anchoredPosition;
        m_initialBottomVector = m_bottomKramp.anchoredPosition;
        m_initialJawHeights = new Vector2(m_initialTopVector.y, m_initialBottomVector.y);
        m_jawMovementMultiplier = new Vector2(m_initialJawHeights.x / m_lowTime, m_initialJawHeights.y / m_lowTime);
    }
    private void Update() {
        float currentTime = Game.MainGameInfo.Timer.GameTime;
        if (Game.Balling) {
            if (currentTime > m_lowTime) {
                m_wantedTopPos = m_initialTopVector;
                m_wantedBottomPos = m_initialBottomVector;
            } else {
                m_wantedTopPos = new Vector2(m_initialTopVector.x, currentTime * m_jawMovementMultiplier.x);
                m_wantedBottomPos = new Vector2(m_initialBottomVector.x, currentTime * m_jawMovementMultiplier.y);
            }
        }
    }
    private void FixedUpdate() {
        if (Game.Balling) {
            if (m_topKramp.anchoredPosition != m_wantedTopPos || m_bottomKramp.anchoredPosition != m_wantedBottomPos) {
                float desiredSpeed = m_jawSpeed * (m_wantedTopPos.y < m_initialTopVector.y ? 1 : m_upwardMultiplier);
                m_topKramp.anchoredPosition = Vector2.Lerp(m_topKramp.anchoredPosition, m_wantedTopPos, desiredSpeed);
                m_bottomKramp.anchoredPosition = Vector2.Lerp(m_bottomKramp.anchoredPosition, m_wantedBottomPos, desiredSpeed);
            }

        }
    }
}
