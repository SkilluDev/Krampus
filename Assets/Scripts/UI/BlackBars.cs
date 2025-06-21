using System.Collections;
using System.Collections.Generic;
using KrampUtils;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlackBars : MonoBehaviour {
    [SerializeField] private float m_duration;

    [SerializeField] private RectTransform m_top, m_bottom;
    [SerializeField] private TMP_Text m_textTop, m_textBottom, m_resultText, m_textTimerTop;



    [Header("======[Map]=====")]
    [BoxGroup("Map")][SerializeField] private RectTransform m_mapContainer;
    [BoxGroup("Map")][SerializeField] private Image m_mapButtonPref;
    [BoxGroup("Map")][SerializeField] private Sprite m_currentLevelSprite;
    [BoxGroup("Map")][SerializeField] private Sprite m_doneLevelSprtie;
    [BoxGroup("Map")][SerializeField] private Sprite m_futureLevelSprtie;


//============================================================================

    [BoxGroup("Animation")][SerializeField] private RectTransform m_resultContainer;
    [BoxGroup("Animation")][SerializeField] private Vector3 m_popScale;
    
    [BoxGroup("Animation")][SerializeField] private float m_popTimer;
    [BoxGroup("Animation")][SerializeField] private float m_shakeIntensity;
    [BoxGroup("Animation")][SerializeField] private float m_shakeDuration;
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
        m_resultText.SetText(text);
    }

    public void AnimateResultText(bool hasWon, string text) {

        int currntLevel = Game.PogMan.CurrentLevel;
        if (hasWon) {

            m_resultText.SetText(currntLevel + "/<levels> Cleared!");
            m_resultText.color = Color.green;
            Vector3 oldScale = m_resultContainer.localScale;
            LMotion.Create(oldScale, m_popScale, m_popTimer / 2).WithDelay(0.35f).WithEase(Ease.OutElastic)
            .WithOnComplete(() => m_resultText.SetText((currntLevel + 1) + "/<levels> Cleared!"))
            .WithOnComplete(
                   () => LMotion.Create(m_popScale, oldScale, m_popTimer / 2).WithEase(Ease.InBounce).WithOnComplete(() => m_resultText.color = Color.white).BindToLocalScale(m_resultContainer)
               ).BindToLocalScale(m_resultContainer);
            return;

        }
        m_resultText.SetText(currntLevel + "/<levels> Failed!");
        //LMotion.Shake.Create(m_resultContainer.localPosition, Vector3.one * m_shakeIntensity, m_shakeDuration).WithDelay(3).WithEase(Ease.InOutQuad).BindToLocalPosition(m_resultContainer);
          m_resultText.color = Color.red;

        
      }

    public void SetTopTimerText(string text) {
        m_textTimerTop.SetText(text);
    }

    public void SetTopBarText(string text) {
        m_textTop.SetText(text);
    }
    public void GenerateMap() {

        int currentLevel = Game.PogMan.CurrentLevel;
        for (int i = 1; i <= currentLevel; i++) {

           Image b = Instantiate(m_mapButtonPref);
            b.transform.SetParent(m_mapContainer, false);
            b.GetComponentInChildren<Image>().sprite = m_doneLevelSprtie;
            b.enabled = false;

        }

        for (int j = m_CurrentLevel + 1; j <= maxLevel; j++) {
             Image b = Instantiate(m_mapButtonPref);
            b.transform.SetParent(m_mapContainer, false);
             b.GetComponentInChildren<Image>().sprite = m_doneLevelSprtie;
            b.enabled = false;
        }


    }
     
}
