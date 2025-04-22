using UnityEngine;

public interface ITongueable {
    public GameObject GameObject => ((MonoBehaviour)this).gameObject;

    public void Passby(Krampus krampus, Vector3 point, float tongueLength);
    public void DirectHit(Krampus krampus, Vector3 point);
}
