using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
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

	[SerializeField] private CanvasGroup m_bindings, m_map;

	private bool m_showBindings;



	[Header("======[Map]=====")]
	[BoxGroup("Map")][SerializeField] private RectTransform m_mapContainer;
	[BoxGroup("Map")][SerializeField] private TwoSidedImage m_mapButtonPref;

	[BoxGroup("Map")][SerializeField] private Sprite m_doneLevelSprite;
	[BoxGroup("Map")][SerializeField] private Sprite m_currentLevelSprite;
	[BoxGroup("Map")][SerializeField] private Sprite m_futureLevelSprite;
	[BoxGroup("Map")][SerializeField] private Sprite m_failedLevelSprite;


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

	public void AnimateResultText(bool hasWon) {
		GenerateMap(hasWon);
	}


	public void SetTopTimerText(string text) {
		m_textTimerTop.SetText(text);
	}

	public void SetTopBarText(string text) {
		m_textTop.SetText(text);
	}


	public void GenerateMap(bool hasWon) {

		int currentLevel = Game.PogMan.CurrentLevel;
		int maxLevel = Game.PogMan.GetMaxLevel();
		int levelIndex = 0;

		Queue<TwoSidedImage> imageSeq1 = new Queue<TwoSidedImage>();
		Queue<TwoSidedImage> imageSeq2 = new Queue<TwoSidedImage>();
		Queue<TwoSidedImage> imageSeq3 = new Queue<TwoSidedImage>();

		for (; levelIndex < currentLevel; levelIndex++) {
			TwoSidedImage beaten = Instantiate(m_mapButtonPref);
			beaten.transform.SetParent(m_mapContainer, false);
			beaten.FrontSprite = m_doneLevelSprite;
			beaten.BackSprite = m_doneLevelSprite;
			beaten.SetToFrontSprite();
			beaten.FlipDuration = 0.5f;
			imageSeq1.Enqueue(beaten);
		}

		TwoSidedImage current = Instantiate(m_mapButtonPref);
		current.transform.SetParent(m_mapContainer, false);
		current.FrontSprite = m_currentLevelSprite;
		current.BackSprite = hasWon ? m_doneLevelSprite : m_failedLevelSprite;
		current.SetToFrontSprite();
		current.FlipDuration = 1f;
		imageSeq2.Enqueue(current);


		levelIndex++;

		if (levelIndex >= maxLevel) return; //TODO Big win?
		if (hasWon) {
			TwoSidedImage next = Instantiate(m_mapButtonPref);
			next.transform.SetParent(m_mapContainer, false);
			next.FrontSprite = m_futureLevelSprite;
			next.BackSprite = m_currentLevelSprite;
			next.SetToFrontSprite();
			next.FlipDuration = 1f;
			imageSeq2.Enqueue(next);
			levelIndex++;
		}

		for (; levelIndex < maxLevel; levelIndex++) {
			TwoSidedImage future = Instantiate(m_mapButtonPref);
			future.transform.SetParent(m_mapContainer, false);
			future.FrontSprite = m_futureLevelSprite;
			future.BackSprite = m_futureLevelSprite;
			future.FlipDuration = 0.35f;
			imageSeq3.Enqueue(future);
		}

		TwoSidedImage.FlipImagesSpaced(
			imageSeq1, 1f, 0.2f,
			() => TwoSidedImage.FlipImagesSequential(
				imageSeq2,
				() => {
					if (imageSeq3.Count != 0 && hasWon)
						TwoSidedImage.FlipImagesSimultaneous(imageSeq3, 0.1f);
				}));
	}

	public void ShowBindings(bool show) {
		if (m_showBindings == show) return;
		if (show) {
			ShowBottomBarText(true);
			Debug.Log("SHOW");
			KrampMotions.ShowHideAlpha(m_bindings, m_map, 2f, Ease.InOutExpo);
		} else {
			KrampMotions.ShowHideAlpha(m_map, m_bindings, 2f, Ease.InOutExpo);
		}

		m_showBindings = show;
	}

	public void ShowBottomBarText(bool show) {
		m_textBottom.gameObject.SetActive(show);
		m_showBindings = show;
	}
}
