using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalEvents : MonoBehaviour {
	public UnityAction<Child> onChildEaten;
	public event Action<float> onNextUpdate;

	private void Update() {
		onNextUpdate?.Invoke(Time.deltaTime);	
	}
}
