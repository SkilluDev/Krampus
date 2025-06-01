public class Effect
{
    public enum Type
    {
        Permanent,
        Expirable
    }
    private Type m_type;
    public Type ModifierDuration => m_type;

    private bool m_isExpired = false;
    public bool IsExpired => m_isExpired;
    protected float m_timer;

    private StatModifier m_statModifier;

    public StatModifier StatModifier => m_statModifier;



    public Effect(KrampusStats.Stat stat, float modifier) {
        m_statModifier = new StatModifier(stat, modifier);
        m_type = Type.Permanent;
    }

    public Effect(KrampusStats.Stat stat, float modifier, float duration)
    {
        m_statModifier = m_statModifier = new StatModifier(stat, modifier);
        m_timer = duration;
        m_type = Type.Expirable;

    }

    private void UpdateTimer(float deltaTime)
    {
        if (m_timer > 0)
        {
            m_timer -= deltaTime;
        }
        else
        {
            m_isExpired = true;
        }
    }

    public virtual void UpdateStat(float deltaTime)
    {
        if (m_type == Type.Expirable && !IsExpired) UpdateTimer(deltaTime);
        if (m_isExpired)
        {
            OnDrop();
        }

    }
    
    public virtual void OnPickup() {
        Game.MainGameInfo.Krampus.Stats.RegisterEffect(this);
    }


	public virtual void OnDrop() {
        Game.MainGameInfo.Krampus.Stats.UnregisterEffect(this);
    }
}