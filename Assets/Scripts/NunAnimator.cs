using NaughtyAttributes;
using Sound;
using UnityEngine;

public class NunAnimator : MonoBehaviour {
	[SerializeField] private Nun m_nun;
	[SerializeField] private Animator m_animator;
	[SerializeField] private Transform m_modelTransform;
	[SerializeField] private float m_turningSpeed = 5f;
	[SerializeField] private float m_minimalVelocity = 0.5f;
	[BoxGroup("Animator Properties")][SerializeField][AnimatorParam(nameof(m_animator))] private int m_propertySpeed, m_propertyListening, m_propertyAttack, m_propertyStunned, m_propertyShocked;
	[BoxGroup("State Sprites")][SerializeField] private StatusSprite m_spriteRenderer;
	[BoxGroup("State Sprites")][SerializeField] private Sprite m_spriteLookingForKrampus, m_spriteListening, m_spriteStunned, m_spriteChasingKrampus;
	[BoxGroup("Sounds")][SerializeField] private Sex m_soundListen, m_soundFoundKrampus, m_soundPatrol, m_soundNotFoundKrampus;

	private void Start() {
		m_nun.onStateChanged += MovementStateChanged;
		m_nun.onAttack += OnNunAttack;
	}

	private void Update() {
		if (!Game.Balling) return;
		m_modelTransform.rotation = Quaternion.Slerp(m_modelTransform.rotation, Quaternion.AngleAxis(m_nun.FacingAngle, Vector3.up), Time.deltaTime * 5f);

		m_animator.SetFloat(m_propertySpeed, m_nun.Velocity / m_nun.RunSpeed);
		m_animator.SetBool(m_propertyListening, m_nun.CurrentState == Nun.State.Listening);
		m_animator.SetBool(m_propertyStunned, m_nun.CurrentState == Nun.State.Stunned);

		if (Random.Range(0, 10000 / 2) == 0) m_soundPatrol.Play(transform.position);
	}

	private void MovementStateChanged(Nun.State previous, Nun.State current) {
		switch (previous, current) {
			case (_, Nun.State.ChasingKrampus):
				m_animator.SetBool(m_propertyShocked, false);
				m_spriteRenderer.SetSprite(m_spriteChasingKrampus, 2);
				break;
			case (_, Nun.State.Patrolling):
				m_spriteRenderer.ClearSprite();
				break;
			case (Nun.State.LookingForKrampus, Nun.State.Idle):
				m_spriteRenderer.ClearSprite();
				m_soundNotFoundKrampus.Play(transform.position);
				break;
			case (_, Nun.State.Idle):
				m_spriteRenderer.ClearSprite();
				break;
			case (_, Nun.State.Listening):
				m_spriteRenderer.SetSprite(m_spriteListening);
				m_soundListen.Play(transform.position);
				break;
			case (_, Nun.State.Stunned):
				m_spriteRenderer.SetSprite(m_spriteStunned, 2);
				break;
			case (_, Nun.State.LookingForKrampus):
				m_spriteRenderer.SetSprite(m_spriteLookingForKrampus);
				break;
			case (_, Nun.State.FoundKrampus):
				m_spriteRenderer.SetSprite(m_spriteChasingKrampus, 2);
				m_soundFoundKrampus.Play(transform.position);
				m_animator.SetBool(m_propertyShocked, true);
				break;


		}
	}

	private void OnNunAttack(Nun.State state) {
		m_animator.SetTrigger(m_propertyAttack);
	}
}


