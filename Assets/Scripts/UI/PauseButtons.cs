using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtons : MonoBehaviour {
	public void GoBackToMenu() {
		Game.PogMan.GoBackToMenu();
	}

	public void Restart() {
		Game.PogMan.LoadFirstLevel(false);
	}

	public void RestartAndRegen() {
		Game.PogMan.LoadFirstLevel(true);
	}

	public void GoBackToGame() {
		Debug.Log("Going back to game");
		InputHandler.SwitchPause();
	}
}
