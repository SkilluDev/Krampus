using Cinemachine;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.Events;

public class MainMenuInfo : LevelInfo {
    public new enum State {
        Default,
        Transitioning,
        Credits,
        Settings
    }

    public State CurrentState { get; private set; } = State.Transitioning;

    public UnityAction<MainMenuInfo.State, MainMenuInfo.State> onStateChanged;
    [SerializeField] private CinemachineVirtualCamera[] m_cameras;
    [SerializeField] private CanvasGroup[] m_canvases;

    private MotionHandle[] m_motions;


    private void Ready() {
        m_motions = new MotionHandle[m_canvases.Length];
        SetState(State.Default);
        Debug.Log("Set state default!");
    }

    private void Unready() {
        foreach (var motion in m_motions)
            if (motion.IsActive()) motion.Cancel();
    }

    public void SetState(State state) {
        if (CurrentState == state) return;
        onStateChanged?.Invoke(CurrentState, state);
        CurrentState = state;
        UpdateGroups();
    }

    public void LoadGameScene() {
        Game.LoadState(Game.State.MainGame);
    }

    // bad code ahead!
    private void UpdateGroups() {
        for (int i = 0; i < m_cameras.Length; i++) {
            int ij = i;
            m_cameras[i].gameObject.SetActive(i == (int)CurrentState);

            if (i >= m_canvases.Length) continue;
            if (m_motions[i].IsActive()) m_motions[i].Cancel();
            if (i == (int)CurrentState) m_canvases[i].gameObject.SetActive(true);
            m_motions[i] = LMotion.Create(m_canvases[i].alpha, ij == (int)CurrentState ? 1f : 0f, 1f)
                .WithOnComplete(
                    () => m_canvases[ij].gameObject.SetActive(ij == (int)CurrentState)
                ).BindToAlpha(m_canvases[ij]);
        }
    }
    public void SetState(int state) => SetState((State)state);
}
