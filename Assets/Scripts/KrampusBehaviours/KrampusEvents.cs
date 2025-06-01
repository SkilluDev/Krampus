using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class KrampusEvents : KrampusBehaviour
{

    public UnityEvent<Child> onChildEaten;
    public UnityEvent<Nun> onNunStunned;

}
