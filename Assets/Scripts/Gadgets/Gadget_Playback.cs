using UnityEngine;

public class Gadget_Playback : Gadget   
{

    [SerializeField] private GameObject m_markPrefabe;
    private GameObject m_mark;
    private bool m_isMarkDeployed;

    [SerializeField] private float m_markDuration = 4f;
    public float MarkDuration => m_markDuration;

    private float m_markCountdown;

	protected override void Update() {
        base.Update();

        if(m_markCountdown > 0) {
            m_markCountdown -= Time.deltaTime;
            if(m_markCountdown <=0) {
                DestroyMark();
            }
        }
    }

	public override void UseGadget(Krampus krampus) {
        
       if(!m_isMarkDeployed) {
            if(!CanBeUsed(krampus)) return;     
            
            m_mark = Instantiate(m_markPrefabe, krampus.gameObject.transform.position, Quaternion.identity);
            m_isMarkDeployed = true;
            SetCD(1f);
            m_markCountdown = m_markDuration;
        }else {

            if(m_cd >= 0 ) return;
            
            krampus.Kontroller.MoveTo(m_mark.transform.position);
           DestroyMark();
            
        }
    }

    private void DestroyMark() {
        SetCD(Cooldown);
        Destroy(m_mark);
        m_mark = null;
        m_isMarkDeployed = false;
    }
}
