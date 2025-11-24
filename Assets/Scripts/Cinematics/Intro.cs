	using Cinemachine;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;

public class Intro : MonoBehaviour {

    [SerializeField] private PlayableDirector m_director;
    [SerializeField] private CinemachineVirtualCamera m_vcam;
    [SerializeField] private Transform m_spawnPoint;

    private void Ready() {
        Game.MainGameInfo.Krampus.Kamera.Rendering.GetComponent<CinemachineBrain>().ActiveBlend = null;
        Game.MainGameInfo.Krampus.Kontroller.MoveTo(m_spawnPoint.position);
        Game.MainGameInfo.Krampus.Animator.SetTargetView(m_spawnPoint.forward);
//
        m_director.Play();
    }

    private void Update() {
        if (Game.Balling || Game.roundInfo.Ended) {
            Game.MainGameInfo.Krampus.Kamera.Rendering.GetComponent<CinemachineBrain>().ActiveBlend = null;
            Destroy(this);
            return;
        }

        if (!Game.IsLoading && InputSubscribe.Raw.UI.QuitTutorial.WasPerformedThisFrame()) {
	        if (Game.MainGameInfo.CurrentState == RoundInfo.State.Intro) {
		        Game.MainGameInfo.SetState(RoundInfo.State.WaitingToStart);
		        Game.MainGameInfo.Krampus.Animator.SetEnableModel(true);
		        Game.MainGameInfo.UI.HideBlackBars();
	        }

	        m_director.Stop();

            m_spawnPoint.gameObject.SetActive(false);
            m_vcam.enabled = false;
        }
    }

    public void SetSecondPartOfIntro() {
	    Game.MainGameInfo.SetState(RoundInfo.State.WaitingToStart);
	    Game.MainGameInfo.Krampus.Animator.SetEnableModel(true);
	    Game.MainGameInfo.UI.HideBlackBars();




    }
}
