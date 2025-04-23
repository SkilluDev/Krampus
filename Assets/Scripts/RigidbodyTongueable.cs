
using UnityEngine;

// terrible and temporary

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyTongueable : MonoBehaviour, ITongueable {
    [SerializeField] private float m_forceMultiplier = 80;
    public void DirectHit(Krampus krampus, Vector3 point) { }
    public void Passby(Krampus krampus, Vector3 point, float tongueLength) {
        GetComponent<Rigidbody>().AddExplosionForce(m_forceMultiplier, point, 4);
    }


}
