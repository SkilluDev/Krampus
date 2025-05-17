using System;
using System.Collections;
using System.Collections.Generic;
using Roomgen;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

	// Update is called once per frame
	private void Update()
    {

        if (InputSubscribe.Raw.UI.Pause.triggered) {
            if (Game.Balling || Game.MainGameInfo.CurrentState == MainGameInfo.State.Paused) SwitchPause();
        }
        if (Game.MainGameInfo.Ended || Game.MainGameInfo.CurrentState == MainGameInfo.State.Paused) {//if the game is over, won, or paused, you can
            if (InputSubscribe.Raw.UI.MenuReturn.triggered) { //go back to menu with M
                Game.MainGameInfo.SetState(MainGameInfo.State.Game);
                Game.LoadState(Game.State.MainMenu);
            }
            if (InputSubscribe.Raw.UI.Restart.triggered) { //restart with R
                Game.RoomGenInfo.Regenerate = RoomGenerationType.Old;
                Game.MainGameInfo.SetState(MainGameInfo.State.Game);
                Game.LoadState(Game.State.MainGame);
            }

            if (InputSubscribe.Raw.UI.RestartAndRegen.triggered) { //generate and restart with G
                Game.RoomGenInfo.Regenerate = RoomGenerationType.New;
                Game.MainGameInfo.SetState(MainGameInfo.State.Game);
                Game.LoadState(Game.State.MainGame);
            }
        }
    }

    private void SwitchPause() {
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
