using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability {
    private KrampusAbilities.Ability m_ability;

    public Ability(KrampusAbilities.Ability ability)
    {
        m_ability = ability;
    }
    public virtual void OnPickup() {
        Game.MainGameInfo.Krampus.Abilities.RegisterAbility(m_ability);
    }

    public virtual void OnDrop() {
        Game.MainGameInfo.Krampus.Abilities.UnRegisterAbility(m_ability);
    }


}
