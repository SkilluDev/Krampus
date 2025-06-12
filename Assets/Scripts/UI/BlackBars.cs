using System.Collections;
using System.Collections.Generic;
using KrampUtils;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class BlackBars : MonoBehaviour {
    [SerializeField] private float m_duration;

    [SerializeField] private RectTransform m_top, m_bottom;
    [SerializeField] private TMP_Text m_textTop, m_textBottom, m_textSideTop, m_textTimerTop;
    private float m_yDistanceTop, m_yDistanceBottom;

    private void Awake() {
        m_yDistanceTop = m_top.sizeDelta.y;
        m_yDistanceBottom = m_bottom.sizeDelta.y;
    }


    [Button("Show")]
    public void Show() {
        LMotion.Create(0, m_yDistanceTop, m_duration).WithEase(Ease.OutExpo).BindToSizeDeltaY(m_top);
        LMotion.Create(0, m_yDistanceBottom, m_duration).WithEase(Ease.OutExpo).BindToSizeDeltaY(m_bottom);
    }

    [Button("Hide")]
    public void Hide() {
        LMotion.Create(m_yDistanceTop, 0, m_duration).WithEase(Ease.InOutCubic).BindToSizeDeltaY(m_top);
        LMotion.Create(m_yDistanceBottom, 0, m_duration).WithEase(Ease.InOutCubic).BindToSizeDeltaY(m_bottom);

    }

    public void HideInstant() {
        m_top.sizeDelta = m_top.sizeDelta.WithY(0);
        m_bottom.sizeDelta = m_bottom.sizeDelta.WithY(0);
    }

    public void ShowInstant() {
        m_top.sizeDelta = m_top.sizeDelta.WithY(m_yDistanceTop);
        m_bottom.sizeDelta = m_bottom.sizeDelta.WithY(m_yDistanceBottom);
    }

    public void SetBottomBarText(string text) {
        m_textBottom.SetText(text);
    }

	public void SetTopBarSideText(string text) {
        m_textSideTop.SetText(text);
    }

	public void SetTopTimerText(string text) {
        m_textTimerTop.SetText(text);
    }

    public void SetTopBarText(string text) {
		m_textTop.SetText(text);
	}
}
