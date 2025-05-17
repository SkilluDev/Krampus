using System.Collections;
using System.Collections.Generic;
using KrampUtils;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using UnityEngine;

public class BlackBars : MonoBehaviour {
    [SerializeField] private float m_duration;

    [SerializeField] private RectTransform m_top, m_bottom;
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

    public void SetVariant(int variant) {
        for (int i = 0; i < m_top.childCount; i++) {
            m_top.GetChild(i).gameObject.SetActive(i == variant);
        }

        for (int i = 0; i < m_bottom.childCount; i++) {
            m_bottom.GetChild(i).gameObject.SetActive(i == variant);
        }
    }
}
