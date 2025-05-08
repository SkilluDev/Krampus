using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable {
    public bool IsOpen { get; private set; }

    [SerializeField] private Animator m_animator;
    [SerializeField] private Collider m_blocking;
    [SerializeField] private Collider m_interactionLeft;
    [SerializeField] private Collider m_interactionRight;
    [SerializeField] private float m_fastOpenObjectVelocity = 6f;
    [SerializeField][AnimatorParam(nameof(m_animator))] private int m_openProperty, m_openSuddenProperty, m_invertProperty;

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


    private void Start() {
        m_animator.SetBool(m_openProperty, false);
        m_blocking.enabled = true;
        m_interactionLeft.enabled = false;
        m_interactionRight.enabled = false;
    }

    public void Interact(IInteractor interactor) {
        Close();
    }

    // TODO: redo
    private void Open(bool swiftly, bool flip) {
        if (IsOpen) return;
        m_animator.SetBool(m_openSuddenProperty, swiftly);
        m_animator.SetBool(m_invertProperty, flip);
        m_animator.SetBool(m_openProperty, true);
        m_blocking.enabled = false;
        m_interactionLeft.enabled = true;
        m_interactionRight.enabled = true;
        IsOpen = true;
    }

    private void Close() {
        if (!IsOpen) return;
        m_animator.SetBool(m_openProperty, false);
        m_blocking.enabled = true;
        m_interactionLeft.enabled = false;
        m_interactionRight.enabled = false;
        IsOpen = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent<ICharacter>(out var character)) return;
        Open(
            character.VelocitySqr > m_fastOpenObjectVelocity * m_fastOpenObjectVelocity,
            Vector3.Dot(transform.forward, transform.position - other.transform.position) > 0
        );
    }

}
