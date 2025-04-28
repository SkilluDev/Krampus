using Cinemachine;
using UnityEngine;

public class MainMenuInfo : LevelInfo {
    public new enum State {
        Default,
        Transitioning,
        Credits,
        Settings
    }

    public State CurrentState { get; private set; } = State.Transitioning;

    [SerializeField] private CinemachineVirtualCamera[] m_cameras;

    private void Start() {
        SetState(State.Default);
    }

    public void SetState(State state) {
        if (CurrentState == state) return;
        CurrentState = state;
        for (int i = 0; i < m_cameras.Length; i++)
            m_cameras[i].gameObject.SetActive(i == (int)state);
    }

    public void SetState(int state) => SetState((State)state);
}
