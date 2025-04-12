using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : MonoBehaviour {
    public bool isBad;
    public Material mat;

    public MeshRenderer ring;
    [SerializeField] private ParticleSystem confirmEatingParticle;
    [SerializeField] private ParticleSystem confirmEatingParticleMistake;


    private void Start() {
        transform.GetChild(0).GetChild(8).GetComponent<Renderer>().material = mat;
        transform.Rotate(transform.rotation.x, Random.Range(0, 360), transform.rotation.z);
    }

    public void Die() { }


    private void OnDestroy() {
        if (gameObject.scene.isLoaded) {
            Instantiate(confirmEatingParticle, transform.position, Quaternion.identity);
        }
    }
}
