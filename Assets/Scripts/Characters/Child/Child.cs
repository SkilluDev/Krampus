using System.Linq;
using KrampUtils;

using SaintsField.Playa;
using Roomgen;
using Sound;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Child : NPC, IKrampable, INoiseReactor {
	public float RunSpeed => m_runSpeed;
	[ShowInInspector] public State CurrentState { get; private set; }

	private State m_lastStateBeforeKilling;
	public State StateBeforeDeath => m_lastStateBeforeKilling;
	public Vector3 InteractionPoint => m_pinTarget.transform.position;
	public UnityAction<Child.State, Child.State> onStateChanged;

	[SerializeField] private ChildAnimator m_animator;
	[Layout("Behaviour", ELayout.FoldoutBox)][SerializeField] private ViewCone m_viewCone;
	[Layout("Behaviour", ELayout.FoldoutBox)][SerializeField] private float m_detectionTime = 0f;
	private float m_currentDetectionTime;
	[Layout("Behaviour", ELayout.FoldoutBox)][SerializeField] private float m_stoppingDistance = 2;
	[Layout("Behaviour", ELayout.FoldoutBox)][SerializeField] private float m_stunDuration = 0.4f;
	[Layout("Behaviour", ELayout.FoldoutBox)][SerializeField] private float m_reportingDuration = 0.4f;
	[Layout("Behaviour", ELayout.FoldoutBox)][SerializeField] private Transform m_pinTarget;
	[SerializeField] private float m_runSpeed = 8;

	private Nun m_selectedNun;

	private Vector3 m_selectedPosition;

	private bool m_hasPositionTarget = false;
	private Room m_selectedRoom;
	private Room m_lastKrampusSpotted;

	private float m_timeout = 0;

	private ChildType m_type;
	private Transform m_modelTransform;
	public bool IsNaughty => Type != Game.MainGameInfo.NiceChildType;




	public ChildType Type => m_type;

	public enum State {
		Idle, // go to a random place
		Shock,
		Stunned,// do nothing.
		InitialPanic, // go to the nearest nun
		Panic, // go to the nearest nun
		Reporting, // talking to another
		Alerted, // go interact with stuff
		Dead,
		Consumed
	}

	public int Priority => IsNaughty ? 5 : -10;


	/* public void Start() {
        SetChildType(Game.MainGameInfo.Types.UnityRandomElement());
    } */

	private void Ready() {
		Game.MainGameInfo.RegisterChild(this);

		m_viewCone.trackedObject = Game.MainGameInfo.Krampus.Kramp.transform;
		m_modelTransform = transform.GetComponentInChildren<Animator>().transform;
	}

	private void Unready() {
		Game.MainGameInfo.UnregisterChild(this);
	}



	public override void OverridePathCosts() {
		NavMesh.SetAreaCost(NavMesh.GetAreaFromName("Kramped"), 99f);
	}

	private void SelectPositionInRoomAwayFromKrampy() {
		if (m_hasPositionTarget) return;
		var passages = Game.MainGameInfo.GetRoomData(CurrentRoom).Passages.OrderBy(w => Vector3.Dot(Game.MainGameInfo.Krampus.transform.position - transform.position, w.transform.position - transform.position));
		m_selectedRoom = passages.First().Other(CurrentRoom);
		m_selectedPosition = m_selectedRoom.GetRandomPointOnFloor().OnNavMesh(5);
		SetDestination(m_selectedPosition);
		m_hasPositionTarget = true;
	}

	private void SelectRandomNun() {
		m_selectedNun = (Nun)Game.MainGameInfo.GetRoomData(CurrentRoom).Characters.FirstOrDefault(w => w is Nun);
		if (m_selectedNun == null) m_selectedNun = (Nun)Game.MainGameInfo.Nuns.UnityRandomElement();
		if (m_selectedNun != null) {
			m_selectedPosition = m_selectedNun.transform.position;
		} else {
			SelectPositionInRoomAwayFromKrampy();
		}
	}

	private void Update() {
		if (!CurrentRoom) return;

		//Debug.Log("MOVING TO:" + m_selectedPosition);

		void SelectNewWanderLocation() {
			if (NavMesh.SamplePosition(MoreMath.RandomInBounds(CurrentRoom.GetBounds()), out var hit, 10, NavMesh.AllAreas)) {
				SetDestination(hit.position);
			} else {
				//Debug.Log("ever considered ending your life");
			}
		}



		switch (CurrentState) {
			case State.Idle: // Child wanders around at random
				if (m_currentPath?.status == NavMeshPathStatus.PathInvalid)
					SelectNewWanderLocation();

				if (NearDestination(m_stoppingDistance) && m_timeout <= 0) {
					// TODO: Magic
					m_timeout = Random.Range(0.1f, 2f);
					SelectNewWanderLocation();
				}

				if (m_viewCone.Detect()) {
					m_currentDetectionTime += Time.deltaTime;
					if (m_currentDetectionTime < m_detectionTime) return;
					m_timeout = m_stunDuration;
					SwitchState(State.Shock);
				} else {
					m_currentDetectionTime = 0f;
				}

				if (m_timeout > 0) {
					m_timeout -= Time.deltaTime;
					SetVelocity(Vector3.zero);
				} else {
					SetVelocity(GetPathDirection() * m_baseMovementSpeed);
					SetFacingDirection(GetPathDirection());
				}

				m_viewCone.SetActive(true);
				break;
			case State.Shock:
				m_viewCone.SetActive(false);
				m_timeout -= Time.deltaTime;
				if (m_timeout <= 0) {
					m_lastKrampusSpotted = CurrentRoom;
					Game.MainGameInfo.GetRoomData(m_lastKrampusSpotted).MarkKramped(true);
					SelectPositionInRoomAwayFromKrampy();
					SwitchState(State.InitialPanic);
				}
				break;
			case State.Stunned:
				m_viewCone.SetActive(false);
				SetVelocity(Vector3.zero);
				m_timeout -= Time.deltaTime;
				if (m_timeout <= 0) {
					m_lastKrampusSpotted = CurrentRoom;
					Game.MainGameInfo.GetRoomData(m_lastKrampusSpotted).MarkKramped(true);
					SelectPositionInRoomAwayFromKrampy();
					SwitchState(State.InitialPanic);
				}
				break;

			case State.InitialPanic: // basically run away franticlly from krampus, however, given the opportunity to go to a nun without turning around, use it
				if (!Game.MainGameInfo.GetRoomData(CurrentRoom).Contains<Krampus>()) {
					SelectRandomNunAndSetDestination();
					SwitchState(State.Panic);
				} else if (NearDestination(m_stoppingDistance)) {
					SelectRandomNunAndSetDestination();
					if (Vector3.Dot(GetPathDirection(), Game.MainGameInfo.Krampus.transform.position - transform.position) > 0f) {
						m_lastKrampusSpotted = CurrentRoom;
						Game.MainGameInfo.GetRoomData(m_lastKrampusSpotted).MarkKramped(true);
						Game.MainGameInfo.RoomGenerator.NavMeshSurface.BuildNavMesh();
						SelectPositionInRoomAwayFromKrampy();
					} else {
						SwitchState(State.Panic);
					}
				} else if (Game.MainGameInfo.GetRoomData(CurrentRoom).Contains<Nun>()) {
					SelectRandomNunAndSetDestination();
					SwitchState(State.Panic);
				}

				SetVelocity(GetPathDirection() * m_runSpeed);
				SetFacingDirection(GetPathDirection());
				break;
			case State.Panic: // regular panic. just go to the nun and report
				if (Game.MainGameInfo.GetRoomData(CurrentRoom).Contains<Nun>()) {
					SelectRandomNun();
				}
				SetVelocity(GetPathDirection() * m_runSpeed);
				SetFacingDirection(GetPathDirection());

				SetDestination(m_selectedPosition);
				if (m_selectedNun != null && m_selectedNun.CurrentState == Nun.State.ChasingKrampus) {
					SelectNewWanderLocation();
					m_selectedNun = null;
					SwitchState(State.Idle);
				}


				if (NearDestination(m_stoppingDistance)) {
					if (m_selectedNun != null && m_selectedNun.CurrentState is Nun.State.Idle or Nun.State.Patrolling or Nun.State.LookingForKrampus) {
						m_selectedNun.ActivateTheBitch(this, m_reportingDuration, m_lastKrampusSpotted);
						m_timeout = m_reportingDuration;
						SwitchState(State.Reporting);
					} else {
						SwitchState(State.Idle);
						m_hasPositionTarget = false;
					}
				}
				break;
			case State.Reporting:
				m_timeout -= Time.deltaTime;
				SetVelocity(Vector3.zero);
				if (m_timeout <= 0) {
					SwitchState(State.Idle);
				}
				break;
		}

		m_viewCone.SetFacing(m_modelTransform.forward); // TODO Smart rotation
	}

	public void SelectRandomNunAndSetDestination() {
		SelectRandomNun();
		SetDestination(m_selectedPosition);
	}
	public void Consume(Krampus krampus, Vector3 position, Quaternion rotation) {
		Game.MainGameInfo.UnregisterChild(this);
		Game.GlobalEvents.onChildEaten.Invoke(krampus, this);

		if (IsNaughty) {
			krampus.KrampusEvents.onNaughtyChildEaten.Invoke(krampus, this);
		} else {
			krampus.KrampusEvents.onNiceChildEaten.Invoke(krampus, this);
		}
		krampus.KrampusEvents.onChildEaten.Invoke(krampus, this);
		SwitchState(State.Consumed);
		Destroy(gameObject);
	}

	public void Hit(Krampus krampus) {
		SwitchState(State.Dead);
	}

	private void SwitchState(State next) {
		//Debug.Log("SWITCHTO:" + next);
		if (next == CurrentState) return;
		m_currentDetectionTime = 0f;
		onStateChanged?.Invoke(CurrentState, next);
		CurrentState = next;
	}

	public void Prepare(Krampus krampus) {
		//  Game.MainGameInfo.UnregisterChild(this);
		m_lastStateBeforeKilling = CurrentState;
	}

	public void AttachToTongue(Krampus krampus, Vector3 position, Quaternion rotation, float progress) {
		transform.position = position - transform.InverseTransformPoint(m_pinTarget.position);
	}


	public void SetChildType(ChildType type) {
		m_type = type;
		m_animator.SetChildType(type);
	}

	public void Alert(RoomData roomData, Vector3 place, ICharacter actor) {
		if (actor is not Krampus || CurrentState != State.Idle) return;
		//Debug.Log("[Child] Child alerted");
		//SwitchState(State.Stunned);
		m_timeout = m_stunDuration;
		SetDestination(place);
		SetFacingDirection(place);
	}

	public void Stun(float duration) {
		Game.MainGameInfo.Krampus.Kamera.DefaultShake.GenerateImpulse();
		m_timeout = duration;
		SwitchState(State.Stunned);
	}
}
