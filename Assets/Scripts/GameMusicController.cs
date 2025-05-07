using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicController : MonoBehaviour
{
    private MusicMan m_musicMan;

    [SerializeField] private DistanceToChild m_closestChild;
    [SerializeField] private float m_minDistance;

    [SerializeField] private float m_resetSpeed = 1;
    private float m_distanceToClosest;

    public void Start() {
	    m_musicMan = Game.MusicMan;
    }

    private void Update() {
	    m_distanceToClosest = Mathf.Sqrt(m_closestChild.Dist);
	    Debug.Log(m_distanceToClosest);
	    if (m_distanceToClosest < m_minDistance) {
		    m_musicMan.GameMusicLayer2Volume = (m_minDistance - m_distanceToClosest) / 20;
	    } else {
		    if (m_musicMan.GameMusicLayer2Volume > 0) {
			    m_musicMan.GameMusicLayer2Volume -= Time.deltaTime * m_resetSpeed;
		    }
	    }
    }

}
