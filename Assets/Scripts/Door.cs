using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Sound;
using UnityEngine;
using UnityEngine.VFX;

public class Door : Passage, IInteractable {
    public bool IsOpen { get; private set; }

    [SerializeField] private Animator m_animator;
    [SerializeField] private Collider m_blocking;
    [SerializeField] private Sex m_doorClose;
    [SerializeField] private Sex m_doorOpen;
    [SerializeField] private float m_fastOpenObjectVelocity = 6f;
    [SerializeField] private float m_noiseDistance = 15f;
    [SerializeField][AnimatorParam(nameof(m_animator))] private int m_openProperty, m_openSuddenProperty, m_invertProperty;
    [SerializeField] private Transform m_flap1, m_flap2;
    [SerializeField] private float m_stunDuration;
    [SerializeField] private float m_stunLinger = 0.3f;


    [SerializeField] private VisualEffect doorBurst;

    private float m_stunTime;


    private List<ICharacter> m_charactersInDoor = new List<ICharacter>();

    public IInteractor.Type InteractorMask => IInteractor.Type.Player;

    private bool m_hitRight = false;

    public bool CanInteract(IInteractor interactor) {
        if (interactor.InteractorType != IInteractor.Type.Player) return false;
        m_hitRight = Random.Range(0, 2) == 0;
        return IsOpen;
    }

    public Vector3 InteractionPoint => m_hitRight ? m_flap1.position : m_flap2.position;

    public int Priority => 0;

    private void Start() {
        m_animator.SetBool(m_openProperty, false);
        m_blocking.enabled = true;
    }

    private void Update() {
        if (m_stunTime > 0) m_stunTime -= Time.deltaTime;
    }

    public void Interact(IInteractor interactor) {
        Close(true);
    }

    private void Open(bool swiftly, bool flip, ICharacter actor = null) {
        if (IsOpen || !Game.Balling) return;
        m_animator.SetBool(m_openSuddenProperty, swiftly);
        m_animator.SetBool(m_invertProperty, flip);
        m_animator.SetBool(m_openProperty, true);
        m_blocking.enabled = false;

        if (swiftly) {
            Game.MainGameInfo.GetRoomData(A).MakeNoise(transform.position, m_noiseDistance, actor);
            Game.MainGameInfo.GetRoomData(B).MakeNoise(transform.position, m_noiseDistance, actor);
            m_doorOpen.Play(transform.position, 1f);
            Debug.Log("Open1.0");

        } else {
            Debug.Log("Open0.4");
            m_doorOpen.Play(transform.position, 0.4f);
        }

        IsOpen = true;
    }

    private void Close(bool swiftly, ICharacter actor = null) {
        if (!IsOpen || !Game.Balling) return;

        if (swiftly) {
            m_stunTime = m_stunLinger;
            //doorBurst.transform.localRotation = Quaternion.Euler(0, 90*(m_animator.GetBool(m_invertProperty)?1:-1), 0);
            doorBurst.Play();
            foreach (var w in m_charactersInDoor) {
                if (w is Nun n) n.Stun(m_stunDuration);
            }

            Game.MainGameInfo.GetRoomData(A).MakeNoise(transform.position, m_noiseDistance, actor);
            Game.MainGameInfo.GetRoomData(B).MakeNoise(transform.position, m_noiseDistance, actor);
            m_doorClose.Play(transform.position, 1f);
        } else {
            m_doorClose.Play(transform.position, 0.4f);
        }

        m_animator.SetBool(m_openSuddenProperty, swiftly);
        m_animator.SetBool(m_openProperty, false);
        m_blocking.enabled = true;
        IsOpen = false;
    }

    private void OnTriggerExit(Collider other) {
        if (!other.TryGetComponent<ICharacter>(out var character)) return;

        m_charactersInDoor.Remove(character);
        if (m_charactersInDoor.Any()) return;
        if (character is Nun or Child) {
            if (character.VelocitySqr < m_fastOpenObjectVelocity * m_fastOpenObjectVelocity) {
                Close(false, character);
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (!other.TryGetComponent<ICharacter>(out var character)) return;

        if (character is Nun nun && nun.CurrentState == Nun.State.Stunned) {
            return;
        }
        Open(
            character.VelocitySqr > m_fastOpenObjectVelocity * m_fastOpenObjectVelocity,
            Vector3.Dot(transform.forward, transform.position - other.transform.position) > 0
        );
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent<ICharacter>(out var character)) return;
        m_charactersInDoor.Add(character);

        if (character is Nun nun && m_stunTime > 0) nun.Stun(m_stunDuration);

        Open(
            character.VelocitySqr > m_fastOpenObjectVelocity * m_fastOpenObjectVelocity,
            Vector3.Dot(transform.forward, transform.position - other.transform.position) > 0,
            character
        );
    }

}
