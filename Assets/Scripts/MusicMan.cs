using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicMan : MonoBehaviour {
	[SerializeField] private AudioSource gameMusicLayer1;
	[SerializeField] private AudioSource gameMusicLayer2;

	public void Ready() {
		Debug.Log("received broadcast");
		if (Game.CurrentState==Game.State.MainGame) {

			gameMusicLayer1.Play();
		}
	}

}
