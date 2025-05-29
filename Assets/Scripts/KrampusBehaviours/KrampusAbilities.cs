using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrampusAbilities : KrampusBehaviour {
    public enum Ability {
        Example
    }
    [SerializeField] private bool m_exampleAbility;

    public bool ExampleAbility => m_exampleAbility;

    public void RegisterAbility(Ability ability) {
        switch (ability) {
            case Ability.Example:
                m_exampleAbility = true;
                break;
        }
    }

    public void UnRegisterAbility(Ability ability) {
        switch (ability) {
            case Ability.Example:
                m_exampleAbility = false;
                break;
        }
    }
}
