using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class StatModifier {
    private KrampusStats.Stat m_stat;
    public KrampusStats.Stat Stat => m_stat;

    private float m_modifier;

    public float Modifier => m_modifier;

    public StatModifier(KrampusStats.Stat stat, float modifier) {
        m_stat = stat;
        m_modifier = modifier;
    }

}
