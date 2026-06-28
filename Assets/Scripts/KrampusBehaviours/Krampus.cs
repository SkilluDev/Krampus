using System;
using Roomgen;
using UnityEngine;

[ExecuteAlways]
public class Krampus : KrampusBehaviour, IInteractor, ICharacter {
    public IInteractor.Type InteractorType => IInteractor.Type.Player;

    public CameraController Kamera => m_camera;
    [SerializeField] private CameraController m_camera;

    public KrampusController Kontroller => m_controller;
    [SerializeField] private KrampusController m_controller;

    public KrampusAnimator Animator => m_animator;
    [SerializeField] private KrampusAnimator m_animator;

    public KrampusTongue Tongue => m_tongue;

    public Room CurrentRoom { get; set; }

    [SerializeField] private KrampusTongue m_tongue;

    public KrampusChildSensor ChildSensor => m_childSensor;

    [SerializeField] private KrampusAbilities m_abilities;

    public KrampusAbilities Abilities => m_abilities;

    [SerializeField] private KrampusStats m_stats;

    public KrampusStats Stats => m_stats;


    [SerializeField] private KrampusEvents m_krampusEvents;
    public KrampusEvents KrampusEvents => m_krampusEvents;


    [SerializeField] private Gadget m_gadget;
	public Gadget Gadget => m_gadget;

    



    public Vector3 VelocityVector => Kontroller.VelocityVector;
    public float Velocity => Kontroller.Velocity;
    public float VelocitySqr => Kontroller.VelocitySqr;

    [SerializeField] private KrampusChildSensor m_childSensor;


    private void Awake() {
        foreach (var w in gameObject.GetComponentsInChildren<KrampusBehaviour>()) {
            typeof(KrampusBehaviour).GetProperty(nameof(Kramp)).SetValue(w, this); // what a lack of `friend` does to a mf
        }
    }
}
