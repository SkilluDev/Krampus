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
    private Vector2 m_jawMovementMultiplier;
    private Vector2 m_initialJawVector;
    private Vector2 m_wantedTopPos;
    private Vector2 m_wantedBottomPos;

    private void Awake() {
        m_initialJawVector = new Vector2(m_topKramp.anchoredPosition.y, m_bottomKramp.anchoredPosition.y);
        m_jawMovementMultiplier = new Vector2(m_initialJawVector.x / m_lowTime, m_initialJawVector.y / m_lowTime);
    }
    private void Update() {
        float currentTime = Game.MainGameInfo.Timer.GameTime;
        if (Game.Balling) {
            m_wantedTopPos = new Vector2(0f, currentTime * m_jawMovementMultiplier.x);
            m_wantedBottomPos = new Vector2(0f, currentTime * m_jawMovementMultiplier.y);
        }
    }
    private void FixedUpdate() {
        if (Game.Balling) {
            m_topKramp.anchoredPosition = Vector2.Lerp(m_topKramp.anchoredPosition, m_wantedTopPos, m_jawSpeed);
            m_bottomKramp.anchoredPosition = Vector2.Lerp(m_bottomKramp.anchoredPosition, m_wantedBottomPos, m_jawSpeed);
        }
    }
}
