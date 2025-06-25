using Cinemachine;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.Events;
using System.Collections;

public class MainMenuInfo : LevelInfo {
    public new enum State {
        Default,
        Transitioning,
        GameModes,
        Settings,
        Credits,
    }

    public State CurrentState { get; private set; } = State.Transitioning;

    public UnityAction<MainMenuInfo.State, MainMenuInfo.State> onStateChanged;
    [SerializeField] private CinemachineVirtualCamera[] m_cameras;
    [SerializeField] private CanvasGroup[] m_canvases;

    private MotionHandle[] m_motions;

    private void Update() {
        if (InputSubscribe.Raw.UI.MenuReturn.triggered || InputSubscribe.Raw.UI.Pause.triggered) {
            SetState(0);
        }
    }

    private void Ready() {
		if (Game.BootFromMainGame) {
			Game.BootFromMainGame = false;
			Game.PogMan.StartNewGame(PogMan.Difficulty.Normal);
			return;
		}
		m_motions = new MotionHandle[m_canvases.Length];
		foreach (var c in m_canvases) c.alpha = 0;
		SetState(State.Default);
		//Debug.Log("Set state default!");
	}

    private void Unready() {
		//Debug.Log("Kill motions");
		if (m_motions == null) return;
        for (int i = 0; i < m_motions.Length; i++) {
			m_motions[i].TryCancel();
		}
    }

    public void SetState(State state) {
        if (CurrentState == state) return;
        onStateChanged?.Invoke(CurrentState, state);
        CurrentState = state;
        UpdateGroups();
    }

    public void LoadGameScene(PogMan.Difficulty difficulty) {
        IEnumerator Internal() {
            SetState(State.Transitioning);
            yield return new WaitForSecondsRealtime(1.2f);
            Game.PogMan.StartNewGame(difficulty);
        }

        StartCoroutine(Internal());
    }
    public void LoadGameEnd() {
        IEnumerator Internal() {
            SetState(State.Transitioning);
            yield return new WaitForSecondsRealtime(1.2f);
            Application.Quit();
        }

        StartCoroutine(Internal());
    }

    public void TutorialLevel() {
		if(CurrentState == State.Transitioning ) return;
        LoadGameScene(PogMan.Difficulty.Normal);
     }

    public void NotTutorialLevel() {
		if(CurrentState == State.Transitioning ) return;
        LoadGameScene(PogMan.Difficulty.Hard);
     }

    // bad code ahead!
    private void UpdateGroups() {
        for (int i = 0; i < m_cameras.Length; i++) {
            int ij = i;
            m_cameras[i].gameObject.SetActive(i == (int)CurrentState);

            if (i >= m_canvases.Length) continue;
            if (m_motions[i].IsActive()) m_motions[i].Cancel();
            if (i == (int)CurrentState) m_canvases[i].gameObject.SetActive(true);
            m_motions[i] = LMotion.Create(m_canvases[i].alpha, ij == (int)CurrentState ? 1f : 0f, 0.5f)
                            .WithOnComplete(() => m_canvases[ij].gameObject.SetActive(ij == (int)CurrentState))
                            .BindToAlpha(m_canvases[ij]);
        }
    }
    public void SetState(int state) => SetState((State)state);
}
