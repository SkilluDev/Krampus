using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/FastKill", fileName = "FastKill")]
public class FastKill : Item {

    [SerializeField] private float m_speedValue;
    [SerializeField] private float m_duration;

    const string m_effectId = "FK_S";


    public override void RegisterEvents(KrampusEvents events) {
        events.onNaughtyChildEaten.AddListener(MovementBuff);
        events.onNiceChildEaten.AddListener(MovementBuff);
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onNaughtyChildEaten.RemoveListener(MovementBuff);
         events.onNiceChildEaten.RemoveListener(MovementBuff);
    }

    private void MovementBuff(Krampus krampus, Child child) {
        Debug.Log("kILL KILL KILL");
        krampus.Stats.RegisterEffect(new Effect(KrampusStats.Stat.Speed, m_speedValue, m_duration,m_effectId));
        Game.MainGameInfo.UI.DisplayEffect(m_duration, ItemIcon, ItemName,m_effectId);
    }
}
