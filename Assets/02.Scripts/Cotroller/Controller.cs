using System;
using System.Collections.Generic;
using _02.Scripts.Augment.BaseAugment;
using _02.Scripts.Managers.Choice;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    public BaseStats baseStats = new BaseStats();
    public AutoAttack autoAttacker;
    public BaseStats FinalStats { get; protected set; }
    
    protected virtual void Awake()
    {
        baseStats.hp = new ClampInt(0, baseStats.maxHp, baseStats.maxHp);
        FinalStats = new BaseStats(baseStats);
    }

    protected virtual void OnEnable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.onDamageEvent += OnDamageReceived;
        }

        FinalStats.hp.Events.onMinReached += Die;
    }

    protected virtual void OnDisable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.onDamageEvent -= OnDamageReceived;
        }
        
        if (FinalStats != null && FinalStats.hp != null)
        {
            FinalStats.hp.Events.onMinReached -= Die;
        }
    }
    
    protected virtual void OnDamageReceived(BattleManager.DamageEventStruct damageEvent)
    {
        
        if (damageEvent.receiver != this) return;
        FinalStats.hp.Decrease(damageEvent.damageAmount);
    }
    
    protected virtual void Die(int prev, int current)
    {
        
    }
}
