using UnityEngine;
using UnityEngine.SceneManagement;

public class Game {

    [RuntimeInitializeOnLoadMethod]
    private static void InitializeGame() {
        DestinationState = State.Loading;
        SourceState = State.Loading;
        CurrentState = (State)SceneManager.GetActiveScene().buildIndex;
        if (CurrentState == State.Loading) {
            Debug.LogError("We don't want you in a load loop! Booting main menu instead.");
            CurrentState = State.MainMenu;
            SceneManager.LoadScene((int)State.MainMenu);
        }
        PrepareCurrentState();

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
    public static bool RequireFullReload { get; private set; }

    public static void PrepareCurrentState() {
        SourceState = CurrentState;
        DestinationState = CurrentState;
        CurrentState = State.Loading;
        RequireFullReload = false;
        SceneManager.LoadScene(LOADER_SCENE, LoadSceneMode.Additive);
    }

    public static void LoadState(State state) {
        SourceState = CurrentState;
        DestinationState = state;
        CurrentState = State.Loading;
        RequireFullReload = true;
        SceneManager.LoadScene(LOADER_SCENE, LoadSceneMode.Additive);
    }

    public static void FinishedLoading() {
        if (CurrentState != State.Loading) return;
        CurrentState = DestinationState;
        DestinationState = State.Loading;
        SourceState = State.Loading;
    }
}
