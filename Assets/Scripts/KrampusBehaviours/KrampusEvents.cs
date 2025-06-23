using UnityEngine.Events;

public class KrampusEvents : KrampusBehaviour {
	public UnityEvent<Krampus, Child> onNaughtyChildEaten;
	public UnityEvent<Krampus, Child> onNiceChildEaten;
	public UnityEvent<Krampus, Child> onChildEaten;
	public UnityEvent<Krampus, Nun> onNunStunned;
	public UnityEvent<Krampus, float> onWindUpChanged;
	public UnityEvent<Krampus, float> onTongueLengthChanged;

	public UnityEvent<Krampus, Nun> onKrampusFoundByNun;

	public UnityEvent<Krampus> onDashUsed;
	public UnityEvent<Krampus> onLockIn;
	public UnityEvent<Krampus> onLockOut;


	public UnityEvent<Krampus, Effect> onEffectRegistered;
	public UnityEvent<Krampus, Effect> onEffectUnregistered;


	public UnityEvent<Krampus, Item> onItemActivated;
	public UnityEvent<Krampus, Item> onItemDesactivated;


}
