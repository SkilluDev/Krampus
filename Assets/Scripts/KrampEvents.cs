using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum KrampEvent {

    ChildEaten,
    NunStunned,

    GameStart

 }

public class KrampEvents : MonoBehaviour {

    public UnityEvent<Child> onChildEaten;
    public UnityEvent<Krampus, Nun> onNunStunned;




    


}
