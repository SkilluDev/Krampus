using Settings;
using NaughtyAttributes;
using Roomgen;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

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
    public static RoomGenInfo RoomGenInfo {
        get {
            if (m_instance.m_roomGenInfo == null) Debug.LogError("RoomGenInfo was not assigned in " + m_instance.gameObject.name);
            return m_instance.m_roomGenInfo;
        }
    }
    [SerializeField] private RoomGenInfo m_roomGenInfo;

    public enum State {
        MainMenu = 0,
        MainGame = 1,
        Loading
    }

    private const int LOADER_SCENE = 2;

    public static Game.State CurrentState { get; private set; }
    public static Game.State SourceState { get; private set; }
    public static Game.State DestinationState { get; private set; }

    public static LevelInfo Info {
        get {
            if (m_instance.m_info == null) m_instance.m_info = GameObject.FindObjectOfType<LevelInfo>();
            if (m_instance.m_info == null) Debug.LogError("No level info found!");
            return m_instance.m_info;
        }
    }
    [SerializeField][ReadOnly] private LevelInfo m_info;

    public static MusicMan MusicMan {
        get {
            if (m_instance.m_musicMan == null) Debug.LogError("MusicMan was not assigned in " + m_instance.gameObject.name);
            return m_instance.m_musicMan;
        }
    }
    [SerializeField] private MusicMan m_musicMan;


    [SerializeField] PogMan m_pogMan;
public static PogMan PogMan {
        get {
            if (m_instance.m_pogMan == null) Debug.LogError("MusicMan was not assigned in " + m_instance.gameObject.name);
            return m_instance.m_pogMan;
        }
    }



    public static Settings.SetMan SetMan {
        get {
            if (m_instance.m_setMan == null) Debug.LogError("SetMan was not assigned in " + m_instance.gameObject.name);
            return m_instance.m_setMan;
        }
    }
    [SerializeField] private Settings.SetMan m_setMan;

    public static MainGameInfo MainGameInfo => (MainGameInfo)Info;
    public static MainMenuInfo MainMenuInfo => (MainMenuInfo)Info;

    public static bool Balling => CurrentState == State.MainGame && MainGameInfo.Ballin;

    public static bool IsLoading => CurrentState == State.Loading;
    public static bool RequireFullReload { get; private set; }

    private static Game m_instance;

    private static void PrepareCurrentState() {
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

    private void Awake() {
        if (Game.m_instance != null) {
            Destroy(gameObject);
            return;
        }
        m_instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
