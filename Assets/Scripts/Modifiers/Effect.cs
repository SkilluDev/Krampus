public class Effect {
    public enum Type {
        Permanent,
        Temporary
        
    }

    public string m_id;

    private Type m_type;
    public Type EffectType => m_type;



    private bool m_isExpired = false;
    public bool IsExpired { get => m_isExpired; set => m_isExpired = value; }
    protected float m_timer;

    private StatModifier m_statModifier;

    public StatModifier StatModifier => m_statModifier;



    public Effect(KrampusStats.Stat stat, float modifier, string id) {
        m_statModifier = new StatModifier(stat, modifier);
        m_type = Type.Permanent;
        m_id = id;
    }

    public Effect(KrampusStats.Stat stat, float modifier, float duration,string id) {
        m_statModifier = m_statModifier = new StatModifier(stat, modifier);
        m_timer = duration;
        m_type = Type.Temporary;
        m_id = id;
    }

    public void UpdateTimer(float deltaTime) {
        if (m_timer > 0) {
            m_timer -= deltaTime;
        } else {
            m_isExpired = true;
        }
    }
    
}