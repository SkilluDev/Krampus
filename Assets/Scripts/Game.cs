using UnityEngine;

public class Game {
    public enum State {
        MainMenu = 0,
        MainGame = 1,
        Credits = 2
    }

    public static Game.State CurrentState { get; private set; }
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
}
