using UnityEngine;


public class JackedChild : Child
{

    [SerializeField]
    private int m_maxHP = 3;
    private int m_currentHP;
    public override bool m_CanBeConsumed => CanBeEaten();

    [SerializeField] private GameObject[] m_healthBar = new GameObject[3];

	protected override void Ready() {
        base.Ready();

        m_currentHP = m_maxHP;
    }
	public void Start() {
		m_currentHP = m_maxHP;
        UpdateHealthBar();
	}

	public override void Stun(float duration) {
       
        m_currentHP--;
         Debug.Log("Jacked Kid HIT");
        if(m_currentHP< 0) {
            m_currentHP =0;
        }
        UpdateHealthBar();

        if(m_currentHP == 0){

            Game.MainGameInfo.Krampus.Kamera.DefaultShake.GenerateImpulse();
            m_timeout = duration;
            SwitchState(State.Stunned);
        }else {
            
            m_lastKrampusSpotted = CurrentRoom;
            Game.MainGameInfo.GetRoomData(m_lastKrampusSpotted).MarkKramped(true);
            SelectPositionInRoomAwayFromKrampy();
        }
	}

	protected override void StunOut() {
        base.StunOut();
        m_currentHP = m_maxHP;  
        UpdateHealthBar();
    }
    private void UpdateHealthBar() {
        foreach(var hb in m_healthBar)
        {
            hb.SetActive(false);
        }
        for(int i =0; i<m_currentHP;i++) {
            m_healthBar[i].SetActive(true);
        }
    }

    private bool CanBeEaten() {
        Debug.Log("Sprawdazanie ooooo");
        return CurrentState == State.Stunned;
    }
    
}
