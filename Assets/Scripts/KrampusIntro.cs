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
        Game.MainGameInfo.Krampus.Kontroller.MoveTo(Vector3.zero);
        m_director.Play();
    }

    private void Update() {
        if (Game.Balling) return;
        if (InputSubscribe.Raw.Player.Move.WasPerformedThisFrame()) {
            Game.MainGameInfo.Krampus.Kontroller.MoveTo(m_spawnPoint.position);
            m_vcam.enabled = false;
            m_director.Stop();
            m_spawnPoint.gameObject.SetActive(false);
            Game.MainGameInfo.SetState(MainGameInfo.State.Game);
        }
    }
}
