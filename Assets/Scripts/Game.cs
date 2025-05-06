using System;
using Unity.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Game {

    [RuntimeInitializeOnLoadMethod]
    private static void InitializeGame() {
        DestinationState = State.Loading;
        SourceState = State.Loading;
        CurrentState = (State)SceneManager.GetActiveScene().buildIndex;
        // LoadState(CurrentState);
    }

    public enum State {
        MainMenu = 0,
        MainGame = 1,
        Credits = 2,
        Loading
    }

    private const int LOADER_SCENE = 3;

    public static Game.State CurrentState { get; private set; }
    public static Game.State SourceState { get; private set; }

    public static Game.State DestinationState { get; private set; }

    public static LevelInfo Info {
        get {
            if (m_info == null) m_info = GameObject.FindObjectOfType<LevelInfo>();
            if (m_info == null) Debug.LogError("No level info found!");
            return m_info;
        }
    }
    private static LevelInfo m_info;

    public static MainGameInfo MainGameInfo => (MainGameInfo)Info;
    public static MainMenuInfo MainMenuInfo => (MainMenuInfo)Info;

    public static bool IsLoading => CurrentState == State.Loading;

    public static void LoadState(State state) {
        SourceState = CurrentState;
        DestinationState = state;
        CurrentState = State.Loading;
        SceneManager.LoadScene(LOADER_SCENE, LoadSceneMode.Additive);
    }

    public static void FinishedLoading() {
        if (CurrentState != State.Loading) return;
        DestinationState = State.Loading;
        SourceState = State.Loading;
        CurrentState = DestinationState;
    }
}
