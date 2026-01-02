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

    #region 증강
    private List<StatAbility> m_Augments = new List<StatAbility>();
    /// <summary>
    /// 새로운 증강을 추가합니다.
    /// </summary>
    public void AddAugment(StatAbility augment)
    {
        m_Augments.Add(augment);
        RecalculateStats();
    }

    /// <summary>
    /// 증강을 제거합니다.
    /// </summary>
    public void RemoveAugment(StatAbility augment)
    {
        m_Augments.Remove(augment);
        RecalculateStats();
    }
    

    /// <summary>
    /// 기본 스탯부터 시작하여 모든 증강을 적용해 최종 스탯을 다시 계산합니다.
    /// </summary>
    protected void RecalculateStats()
    {
        // 1. 최종 스탯을 기본 스탯으로 초기화
        FinalStats = new BaseStats(baseStats);
        
        // 2. 모든 증강의 스탯 수정치를 순서대로 적용
        foreach (var augment in m_Augments)
        {
            augment.Apply(FinalStats);
        }
    }
    #endregion
}
