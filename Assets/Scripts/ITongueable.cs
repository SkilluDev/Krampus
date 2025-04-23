using UnityEngine;

public interface ITongueable {
    public GameObject GameObject => ((MonoBehaviour)this).gameObject;

    public void TonguePassBy(Krampus krampus, Vector3 point, float tongueLength);
    public void TongueHit(Krampus krampus, Vector3 point);
}
