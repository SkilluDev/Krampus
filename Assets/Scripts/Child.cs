using System;
using KrampUtils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Child : NPC, IEdible {
    public enum State {
        Idle, // go to a random place
        Stunned, // do nothing.
        Panic, // go to the nearest nun
        Alerted, // go interact with stuff
        Dead
    }

    public UnityAction<Child.State, Child.State> onStateChanged;
    [SerializeField] private float m_interactionDistance = 8;


	[SerializeField] private Sprite[] shapes = new Sprite[5];

	[SerializeField] private SpriteRenderer kidShape;


    public void Start() {
	    setChildColor(Random.Range(0,4));

    }

    public State CurrentState { get; private set; }


    private void OnEnable() {
        Game.MainGameInfo.RegisterChild(this);
    }

    private void OnDisable() {
        Game.MainGameInfo.UnregisterChild(this);
    }

    private void SelectNewWanderLocation() {
        if (NavMesh.SamplePosition(Game.MainGameInfo.RoomGenerator.Rooms.UnityRandomElement().GetMidPoint(), out var hit, 10, NavMesh.AllAreas)) {
            SetDestination(hit.position);
        } else {
            Debug.Log("ever considered ending your life");
        }
    }

    private void Update() {
        switch (CurrentState) {
            case State.Idle:
                if (m_currentPath?.status == NavMeshPathStatus.PathInvalid || NearDestination(m_interactionDistance)) {
                    SelectNewWanderLocation();
                }

                SetVelocity(GetPathDirection());

                break;
        }

        HandleRoomRegistration();
    }

    public void Consume(Krampus krampus) {
        Destroy(gameObject);
    }
    public void Hit(Krampus krampus) {
        SwitchState(State.Dead);
    }

    private void SwitchState(State previous) {
        if (previous == CurrentState) return;
        Debug.Log(onStateChanged);
        onStateChanged?.Invoke(CurrentState, previous);
        CurrentState = previous;
    }

    public void Prepare(Krampus krampus) {
        Game.MainGameInfo.UnregisterChild(this);
    }

    public void ReelIn(Krampus krampus, Vector3 position, float progress) {
        transform.position = position;
    }





    public  void setChildColor(int type) {
	   var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

	   Color color = Color.white;
	   switch (type) {
		   case 0:
			   color = new Color(0.8f, 0.8f, 0.8f);  //szary - koło
			   break;
		   case 1:
			   color = new Color(0.7529f, 0.4314f, 0f); //Pomarańczowy? - wklęsły
			   break;
		   case 2:
			   color = new Color(0.2118f, 0.8549f, 0f);  //zielony
			   break;
		   case 3: color =  new Color(0.3412f, 0f, 0.5020f);  //purple guy omg is that a Fnaf reference  omg Mimic
			   break;
		   case 4: color = new Color(1.0f, 0.0f, 0.3333f); //Czerwony
			   break;
		   default:
			   break;


	   };

	    foreach ( var s in skinnedMeshRenderers) {
		    Material materialInstance =  s.material;

		    materialInstance.SetColor("_Color", color);
	    }

	    kidShape.sprite = shapes[type];
	    kidShape.color = color;

    }

}
