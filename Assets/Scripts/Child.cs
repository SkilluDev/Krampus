using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : MonoBehaviour
{
    public bool isBad;
    public Material mat;
    [SerializeField] ParticleSystem confirmEatingParticle;
    [SerializeField] ParticleSystem confirmEatingParticleMistake;


    private void Start()
    {
        transform.GetChild(0).GetChild(8).GetComponent<Renderer>().material = mat;
    }

    public void Die() { }


    private void OnDestroy()
    {
        if (isBad)
        {
            Instantiate(confirmEatingParticle, transform.position, Quaternion.identity);

        }
        else 
        {
            Instantiate(confirmEatingParticleMistake, transform.position, Quaternion.identity);
        }
        

    }
}
