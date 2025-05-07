using UnityEngine;

[ExecuteAlways]
public class Krampus : KrampusBehaviour, IInteractor {
    public IInteractor.Type InteractorType => IInteractor.Type.Player;

    public CameraController Kamera => m_camera;
    [SerializeField] private CameraController m_camera;

    public KrampusController Kontroller => m_controller;
    [SerializeField] private KrampusController m_controller;

    public KrampusAnimator Animator => m_animator;
    [SerializeField] private KrampusAnimator m_animator;

    public KrampusTongue Tongue => m_tongue;
    [SerializeField] private KrampusTongue m_tongue;

    public ChildSensor ChildSensor => m_childSensor;
    [SerializeField] private ChildSensor m_childSensor;


    private void Awake() {
        foreach (var w in gameObject.GetComponentsInChildren<KrampusBehaviour>()) {
            typeof(KrampusBehaviour).GetProperty(nameof(Kramp)).SetValue(w, this); // what a lack of `friend` does to a mf
        }
    }
}
