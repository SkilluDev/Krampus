using UnityEngine;

[ExecuteAlways]
public class Krampus : KrampusBehaviour {
    public CameraController Kamera => m_camera;
    [SerializeField] private CameraController m_camera;

    public KrampusController Kontroller => m_controller;
    [SerializeField] private KrampusController m_controller;


    private void Awake() {
        foreach (var w in gameObject.GetComponentsInChildren<KrampusBehaviour>()) {
            typeof(KrampusBehaviour).GetProperty(nameof(Kramp)).SetValue(w, this); // what a lack of `friend` does to a mf
        }
    }
}
