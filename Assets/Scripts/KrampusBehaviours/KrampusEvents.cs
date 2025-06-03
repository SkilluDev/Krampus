using UnityEngine.Events;

public class KrampusEvents : KrampusBehaviour {
    public UnityEvent<Krampus, Child> onNaughtyChildEaten;
    public UnityEvent<Krampus, Child> onNiceChildEaten;
    public UnityEvent<Krampus, Child> onChildEaten;
    public UnityEvent<Krampus, Nun> onNunStunned;

    public UnityEvent<Krampus, Nun> onKrampusFoundByNun;

    public UnityEvent<Krampus> onStingerUsed;

}
