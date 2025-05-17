using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildMeter : MonoBehaviour {
	[SerializeField] private Vector2 maxMinZRotation;
	[SerializeField] private RectTransform arrow;
	[SerializeField] private int maxChildrenPerMinute = 12;
	[SerializeField] private NumericDisplay numericDisplay;
	public float score;


	private void Update() {


	arrow.localRotation = Quaternion.Euler(0,0,Mathf.Lerp(maxMinZRotation.x, maxMinZRotation.y, score/maxChildrenPerMinute));
	numericDisplay.Value = score;

	}
}
