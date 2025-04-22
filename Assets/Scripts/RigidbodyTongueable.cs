using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyTongueable : MonoBehaviour, ITongueable {
    public void DirectHit(Krampus krampus, Vector3 point) { }
    public void Passby(Krampus krampus, Vector3 point, float tongueLength) {
        Debug.Log("Passby " + name);
        GetComponent<Rigidbody>().AddExplosionForce(80, point, 4);
    }


}
