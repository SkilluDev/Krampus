using UnityEngine;

public class Gadget : MonoBehaviour
{
   [SerializeField] protected string m_gadgetName = "Item Name";
   	public string GadgetName => m_gadgetName;

    [SerializeField] protected int m_windupCost;
    public int WindUpCost => m_windupCost;

    [SerializeField] protected float m_cooldown;
    public float Cooldown => m_cooldown;

    protected float m_cd;


    protected virtual void Update() {
        if(m_cd > 0) {
            m_cd-=Time.deltaTime;
        }
    }

    public virtual void UseGadget(Krampus krampus) {
        
    }

    public virtual void PayCost(Krampus krampus) {
        krampus.Kramp.Kontroller.SpendWindUpPoints(m_windupCost);
    }

    public virtual bool CanBeUsed(Krampus krampus) {
        return krampus.Kontroller.WindUpPoints == m_windupCost && m_cd <= 0;
    }

    protected void SetCD(float cd) {
        m_cd = cd;
    }

}
        