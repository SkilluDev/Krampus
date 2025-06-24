using System;
using System.Collections;
using System.Collections.Generic;
using Roomgen;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
	// Update is called once per frame
	private void Update() {

		if (Game.Balling || Game.MainGameInfo.CurrentState == MainGameInfo.State.Paused) {
			if (InputSubscribe.Raw.UI.Pause.triggered) SwitchPause();
		}

		if (Game.MainGameInfo.Lost || Game.MainGameInfo.CurrentState == MainGameInfo.State.Paused) {//if the game is over, or paused, you can

			if (InputSubscribe.Raw.UI.Advance.triggered && Game.PogMan.CanGoToNextLevel) { //go to next level with space
				Game.MainGameInfo.UI.BlackBars.ShowBindings(true);
			}

			if (InputSubscribe.Raw.UI.MenuReturn.triggered) { //go back to menu with M
				Game.PogMan.GoBackToMenu();
			}
			if (InputSubscribe.Raw.UI.Restart.triggered) { //restart with R
				Game.PogMan.LoadFirstLevel(false);
			}

			if (InputSubscribe.Raw.UI.RestartAndRegen.triggered) { //generate and restart with G
				Game.PogMan.LoadFirstLevel(true);
			}
		}

		if (Game.MainGameInfo.Won) {//if the game is won, you can
			/* if (InputSubscribe.Raw.UI.MenuReturn.triggered) { //go back to menu with M
				Game.PogMan.GoBackToMenu();
			} */

			if (InputSubscribe.Raw.UI.Advance.triggered && Game.PogMan.CanGoToNextLevel) { //go to next level with space
				Game.PogMan.LoadNextLevel();
			}

			/* if (InputSubscribe.Raw.UI.Restart.triggered) { //restart with R
				Game.PogMan.LoadFirstLevel(false);
			}

			if (InputSubscribe.Raw.UI.RestartAndRegen.triggered) { //generate and restart with G
				Game.PogMan.LoadFirstLevel(true);
			} */
		}

	}

    public static void SwitchPause() {
        if (!Game.MainGameInfo.Ended) {
            if (Game.MainGameInfo.CurrentState == MainGameInfo.State.Paused) {
                Game.MainGameInfo.SetState(MainGameInfo.State.Game);
                Game.MainGameInfo.UI.SwitchPauseMenu(false);
            } else {
                Game.MainGameInfo.SetState(MainGameInfo.State.Paused);
                Game.MainGameInfo.UI.SwitchPauseMenu(true);
            }
            Time.timeScale = Game.MainGameInfo.CurrentState != MainGameInfo.State.Paused ? 1 : 0;
        }
    }
}
