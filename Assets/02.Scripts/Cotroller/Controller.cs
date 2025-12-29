using System;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    public BaseStats baseStats = new BaseStats();
    
    public AutoAttack autoAttacker;
    public BaseStats FinalStats { get; private set; }

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
        FinalStats.hp.Events.onMinReached -= Die;
    }
    
    protected virtual void OnDamageReceived(BattleManager.DamageEventStruct damageEvent)
    {
        
        if (damageEvent.receiver != this) return;
        Debug.Log($"데미지 받음 {gameObject.name}");
        FinalStats.hp.Decrease(damageEvent.damageAmount);
    }
    
    protected virtual void Die(int prev, int current)
    {
        Debug.Log($"{gameObject.name} 사망");
    }
}
