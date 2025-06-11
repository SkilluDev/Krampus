using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalEvents : MonoBehaviour {
	public UnityEvent<Krampus, Child> onChildEaten;
	public UnityEvent<MainGameInfo.State, MainGameInfo.State> onLevelStateChanged;
}
