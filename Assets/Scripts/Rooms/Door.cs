using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Sound;
using UnityEngine;
using UnityEngine.VFX;

public class Door : Passage, IInteractable {
	public bool IsOpen { get; private set; }

	private bool IsClosingSwiftly => m_animator.GetBool("IsClosingSwiftly");

	[BoxGroup("Animator")][SerializeField] private Animator m_animator;
	[BoxGroup("Collider")][SerializeField] private Collider m_blocking;
	[BoxGroup("Sound")][SerializeField] private Sex m_doorClose;
	[BoxGroup("Sound")][SerializeField] private Sex m_doorOpen;
	[SerializeField] private float m_fastOpenObjectVelocity = 6f;
	[SerializeField] private float m_noiseDistance = 15f;
	[SerializeField][AnimatorParam(nameof(m_animator))] private int m_openProperty, m_openSuddenProperty, m_invertProperty;
	[SerializeField] private Transform m_flap1, m_flap2;
	[BoxGroup("Stun")][SerializeField] private float m_stunDuration;
	[BoxGroup("Stun")][SerializeField] private float m_stunLinger = 0.3f;
	[BoxGroup("Box")][SerializeField] private float m_boxSpeedDamp = 2f;
	[BoxGroup("VFX")][SerializeField] private VisualEffect m_doorBurst;

	private List<ICharacter> m_charactersInDoor = new List<ICharacter>();
	public IInteractor.Type InteractorMask => IInteractor.Type.Player;
	private bool m_hitRight = false;

	public bool CanInteract(IInteractor interactor) {
		if (interactor.InteractorType != IInteractor.Type.Player) return false;
		m_hitRight = Random.Range(0, 2) == 0;
		return true;
	}

	public Vector3 InteractionPoint => m_hitRight ? m_flap1.position : m_flap2.position;

	[BoxGroup("Priority")][SerializeField] private static int m_basePriority = 2;
	public int Priority => m_charactersInDoor.Any(w => w is Nun) ? 20 : m_basePriority;

	private void Start() {
		m_animator.SetBool(m_openProperty, false);
		m_blocking.enabled = true;
	}

	public void Interact(IInteractor interactor) {
		Close(true);
	}

	private bool ShouldFlip(Vector3 position) {
		return Vector3.Dot(transform.forward, transform.position - position) > 0;
	}

	public void Open(bool swiftly, Vector3 position, ICharacter actor = null) {
		if (IsOpen || !Game.Balling) return;
		IsOpen = true;
		m_blocking.enabled = false;

		m_animator.SetBool(m_openSuddenProperty, swiftly);
		m_animator.SetBool(m_invertProperty, ShouldFlip(position));
		m_animator.SetBool(m_openProperty, true);

		if (swiftly) {
			Game.roundInfo.GetRoomData(A).MakeNoise(transform.position, m_noiseDistance, actor);
			Game.roundInfo.GetRoomData(B).MakeNoise(transform.position, m_noiseDistance, actor);
			m_doorOpen.Play(transform.position, 1f);

			foreach (var w in m_charactersInDoor) {
				if (ShouldFlip(w.GameObject.transform.position) == ShouldFlip(position)) continue;
				if (w is Nun n) n.Stun(m_stunDuration);
				if (w is Child c) c.Stun(m_stunDuration); // TODO: extract a Stunnable or make Npc stunnable
			}
		} else {
			m_doorOpen.Play(transform.position, 0.4f);
		}
	}

	public void Close(bool swiftly, ICharacter actor = null) {
		if (!IsOpen || !Game.Balling) return;
		IsOpen = false;
		m_blocking.enabled = true;

		if (swiftly) {
			m_doorBurst.Play();
			StunCharactersInDoor();
			Game.roundInfo.GetRoomData(A).MakeNoise(transform.position, m_noiseDistance, actor);
			Game.roundInfo.GetRoomData(B).MakeNoise(transform.position, m_noiseDistance, actor);
			m_doorClose.Play(transform.position, 1f);
		} else {
			m_doorClose.Play(transform.position, 0.4f);
		}

		m_animator.SetBool(m_openSuddenProperty, swiftly);
		m_animator.SetBool(m_openProperty, false);
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

		if (character is Child child && child.CurrentState == Child.State.Stunned) {
			return;
		}

		Open(
			character.VelocitySqr > m_fastOpenObjectVelocity * m_fastOpenObjectVelocity,
			other.transform.position
		);
	}

	private void OnTriggerEnter(Collider other) {
		if (!other.TryGetComponent<ICharacter>(out var character)) return;
		m_charactersInDoor.Add(character);
		if (IsClosingSwiftly) StunCharacter(character);

		if (character is Nun nun && nun.CurrentState == Nun.State.Stunned) {
			return;
		}

		if (character is Child child && child.CurrentState == Child.State.Stunned) {
			return;
		}

		Open(
			character.VelocitySqr > m_fastOpenObjectVelocity * m_fastOpenObjectVelocity,
			other.transform.position,
			character
		);
	}

	private void StunCharactersInDoor() {
		foreach (var w in m_charactersInDoor) {
			StunCharacter(w);
		}
	}

	private void StunCharacter(ICharacter w) {
		if (w is Nun n) n.Stun(m_stunDuration);
		if (w is Child c) c.Stun(m_stunDuration);
	}
}
