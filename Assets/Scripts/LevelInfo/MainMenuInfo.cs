using Cinemachine;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.Events;
using System;

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


    private void Start() {
        m_motions = new MotionHandle[m_canvases.Length];
        SetState(State.Default);
    }

    public void SetState(State state) {
        if (CurrentState == state) return;
        onStateChanged?.Invoke(CurrentState, state);
        CurrentState = state;
        UpdateGroups();
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
