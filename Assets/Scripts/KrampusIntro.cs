using Cinemachine;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;

public class KrampusIntro : MonoBehaviour {

    [SerializeField] private PlayableDirector m_director;
    [SerializeField] private CinemachineVirtualCamera m_vcam;
    [SerializeField] private Transform m_spawnPoint;

    private void Ready() {
        Game.MainGameInfo.Krampus.Kamera.Rendering.GetComponent<CinemachineBrain>().ActiveBlend = null;
        Game.MainGameInfo.Krampus.Kontroller.MoveTo(m_spawnPoint.position);
        Game.MainGameInfo.Krampus.Animator.SetTargetView(m_spawnPoint.forward);

        m_director.Play();
    }

    private void Update() {
        if (Game.Balling || Game.MainGameInfo.CurrentState == MainGameInfo.State.Over) {
            Game.MainGameInfo.Krampus.Kamera.Rendering.GetComponent<CinemachineBrain>().ActiveBlend = null;
            Destroy(this);
            return;
        }

        if (InputSubscribe.Raw.Player.Move.WasPressedThisFrame()) {
            m_director.Stop();
            Game.MainGameInfo.Krampus.Animator.SetEnableModel(true);
            m_spawnPoint.gameObject.SetActive(false);
            m_vcam.enabled = false;
            Game.MainGameInfo.SetState(MainGameInfo.State.Game);
        }
    }

    public void SetSecondPartOfIntro() {
	    Game.MainGameInfo.SetState(MainGameInfo.State.WaitingToStart);

	    Game.MainGameInfo.UI.HideBlackBars();
	    Debug.Log( "Siema zacznij grac"+MainGameInfo.State.WaitingToStart);



    }
}
