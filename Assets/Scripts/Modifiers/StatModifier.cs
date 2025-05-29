using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class StatModifier {

    public enum Duration {
        Permanent,
        Expirable
    }
    private Duration m_duration;
    public Duration ModifierDuration => m_duration;

    private KrampusStats.Stat m_stat;
    public KrampusStats.Stat Stat => m_stat;
    private bool m_isExpired = false;
    public bool IsExpired => m_isExpired;
    protected float m_timer;

    private float m_modifier;

    public float Modifier => m_modifier;

    public StatModifier(KrampusStats.Stat stat, float modifier) {
        m_stat = stat;
        m_modifier = modifier;
        m_duration = Duration.Permanent;
    }

    public StatModifier(KrampusStats.Stat stat, float modifier, float duration) {
        m_stat = stat;
        m_modifier = modifier;
        m_timer = duration;
        m_duration = Duration.Expirable;
    }
    public virtual void OnPickup() {
        Game.MainGameInfo.GlobalEvents.onNextUpdate += UpdateStat;
        Game.MainGameInfo.Krampus.Stats.RegisterStatModifier(this);
    }


	public virtual void OnDrop() {
        Game.MainGameInfo.Krampus.Stats.UnRegisterStatModifier(this);
    }


    public virtual void UpdateStat(float deltaTime) {
        if (m_duration == Duration.Expirable && !IsExpired) UpdateTimer(deltaTime);
        if (m_isExpired) {
            OnDrop();
        }
        
    }

	private void UpdateTimer(float deltaTime){
        if (m_timer > 0) {
            m_timer -= deltaTime;
        } else {
            m_isExpired = true;
        }
    }
}
