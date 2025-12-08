using System;
using UnityEngine;



[Serializable]
public class Challange : ScriptableObject
{
    

    [SerializeField] private int m_goldReward;
    public int GoldReward => m_goldReward;

    [SerializeField] protected int value;
    public int Value => value;
    [SerializeField] protected  int currentValue = 0;

    [SerializeField] protected string m_desc;
    public string Description => m_desc;
    
    protected bool m_stillActive = true;


    public void Claim() {
        if(m_stillActive){
        Game.PogMan.AddGold(m_goldReward);
        }
    }

    public void Reset () {
        currentValue = 0;
        m_stillActive = true;
    }

    public virtual void Register() {}


    public void Fail() {
        m_stillActive = false;
    }

}
