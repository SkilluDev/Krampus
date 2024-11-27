using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : MonoBehaviour
{
    public bool isBad;
    public Material mat;
    [SerializeField] ParticleSystem confirmEatingParticle;
    [SerializeField] ParticleSystem confirmEatingParticleMistake;
    public WinCondition WinCon;


    private void Start()
    {
        transform.GetChild(0).GetChild(8).GetComponent<Renderer>().material = mat;
        WinCon = GameObject.Find("Win Condition").GetComponent<WinCondition>();
    }

    public void Die() { }


    private void OnDestroy()
    {
        Instantiate(confirmEatingParticle, transform.position, Quaternion.identity);
        
        if (!isBad)
        {
            WinCon.SubtractTime(15);
        }
        

    }
}
