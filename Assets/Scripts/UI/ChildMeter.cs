using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildMeter : MonoBehaviour {
	[SerializeField] private Vector2 m_maxMinZRotation;
	[SerializeField] private RectTransform m_arrow;
	[SerializeField] private int m_maxChildrenPerMinute = 12;
	[SerializeField] private float m_score;
	[SerializeField] private NumericDisplay m_numericDisplay;
	[SerializeField] private Image m_bloodRenderer;
	[SerializeField] private Sprite[] m_bloodSprites;

	public float Score {
		get => m_score; private set =>
			SetScore(value);
	}


	private void Update() {
		m_arrow.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(m_maxMinZRotation.x, m_maxMinZRotation.y, m_score / m_maxChildrenPerMinute));
		m_numericDisplay.Value = m_score;

		m_bloodRenderer.sprite = m_bloodSprites[(int)Mathf.Clamp(0f,m_bloodSprites.Length-1, Mathf.FloorToInt((m_score / m_maxChildrenPerMinute * m_bloodSprites.Length)))];
	}

	public void SetScore(float obj) {
		m_score = obj;
	}
}
