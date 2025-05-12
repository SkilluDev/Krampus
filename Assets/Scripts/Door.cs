using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Door : Passage, IInteractable {
    public bool IsOpen { get; private set; }

    [SerializeField] private Animator m_animator;
    [SerializeField] private Collider m_blocking;
    [SerializeField] private Collider m_interactionLeft;
    [SerializeField] private Collider m_interactionRight;
    [SerializeField] private SoundBite m_doorClose;
    [SerializeField] private SoundBite m_doorOpen;
    [SerializeField] private float m_fastOpenObjectVelocity = 6f;
    [SerializeField][AnimatorParam(nameof(m_animator))] private int m_openProperty, m_openSuddenProperty, m_invertProperty;

    [SerializeField] private float m_stunDuration;


    private List<ICharacter> m_charactersInDoor = new List<ICharacter>();

    public IInteractor.Type InteractorMask => IInteractor.Type.Player;

    private bool m_hitRight = false;

    public bool CanInteract(IInteractor interactor) {
        if (interactor.InteractorType != IInteractor.Type.Player) return false;
        m_hitRight =
            Vector3.SqrMagnitude(interactor.AsPlayer().Tongue.HitPoint - m_interactionLeft.ClosestPoint(interactor.AsPlayer().Tongue.HitPoint)) >
            Vector3.SqrMagnitude(interactor.AsPlayer().Tongue.HitPoint - m_interactionRight.ClosestPoint(interactor.AsPlayer().Tongue.HitPoint));
        return true;
    }

    public Vector3 InteractionPoint => m_hitRight ? m_interactionRight.transform.position + m_interactionRight.transform.up : m_interactionLeft.transform.position + m_interactionLeft.transform.up;


    [System.Obsolete]
    private float CalculateVolumeOverride() {
        return Mathf.InverseLerp(50, 20, Vector3.Distance(Game.MainGameInfo.Krampus.transform.position, transform.position)) * 0.5f;
    }

    private void Start() {
        m_animator.SetBool(m_openProperty, false);
        m_blocking.enabled = true;
        m_interactionLeft.enabled = false;
        m_interactionRight.enabled = false;
    }

    public void Interact(IInteractor interactor) {
        Close(true);
    }

    // TODO: redo
    private void Open(bool swiftly, bool flip) {
        if (IsOpen) return;
        m_doorOpen.SetVolume(CalculateVolumeOverride());
        m_doorOpen.Play(transform.position, 1f);
        m_animator.SetBool(m_openSuddenProperty, swiftly);
        m_animator.SetBool(m_invertProperty, flip);
        m_animator.SetBool(m_openProperty, true);
        m_blocking.enabled = false;
        m_interactionLeft.enabled = true;
        m_interactionRight.enabled = true;
        IsOpen = true;
    }

    private void Close(bool swiftly) {
        if (!IsOpen) return;
        m_doorClose.SetVolume(CalculateVolumeOverride());

        if (swiftly) {
            foreach (var w in m_charactersInDoor) {
                if (w is Nun n) n.Stun(m_stunDuration);
            }
        }

        m_doorClose.Play(transform.position, 1f);
        m_animator.SetBool(m_openSuddenProperty, swiftly);
        m_animator.SetBool(m_openProperty, false);
        m_blocking.enabled = true;
        m_interactionLeft.enabled = false;
        m_interactionRight.enabled = false;
        IsOpen = false;
    }

    private void OnTriggerExit(Collider other) {
        if (!other.TryGetComponent<ICharacter>(out var character)) return;
        m_charactersInDoor.Remove(character);
        if (character is Nun or Child) {
            if (character.VelocitySqr < m_fastOpenObjectVelocity * m_fastOpenObjectVelocity) {
                Close(false);
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (!other.TryGetComponent<Nun>(out var character)) return;
        if (character.CurrentState != Nun.State.Stunned) Open(
            character.VelocitySqr > m_fastOpenObjectVelocity * m_fastOpenObjectVelocity,
            Vector3.Dot(transform.forward, transform.position - other.transform.position) > 0
        );
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent<ICharacter>(out var character)) return;
        m_charactersInDoor.Add(character);

        Open(
            character.VelocitySqr > m_fastOpenObjectVelocity * m_fastOpenObjectVelocity,
            Vector3.Dot(transform.forward, transform.position - other.transform.position) > 0
        );
    }

}
