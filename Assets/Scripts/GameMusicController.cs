using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicController : MonoBehaviour
{
    private MusicMan m_musicMan;

    [SerializeField] private float m_minDistance;

    [SerializeField] private float m_resetSpeed = 1;
    private float m_distanceToClosest;

    public void Start() {
	    m_musicMan = Game.MusicMan;
    }

    private void Update() {
		if(!Game.Balling) return;
	    m_distanceToClosest = Mathf.Sqrt(Game.MainGameInfo.Krampus.ChildSensor.Dist);
	    if (m_distanceToClosest < m_minDistance) {
		    m_musicMan.GameMusicLayer2Volume = Mathf.Lerp(0,1,Mathf.Clamp((m_minDistance-m_distanceToClosest)/m_minDistance,0,1));

	    } else {
		    if (m_musicMan.GameMusicLayer2Volume > 0) {
			    m_musicMan.GameMusicLayer2Volume -= Time.deltaTime * m_resetSpeed;
		    }
	    }
    }

}
