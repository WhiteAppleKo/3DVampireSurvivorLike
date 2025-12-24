using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 투사체를 관리하는 오브젝트 풀링 클래스입니다.
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 모든 무기의 기본 클래스입니다. 증강 시스템과 스탯 관리를 포함합니다.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [Tooltip("무기의 기본 스탯. Inspector에서 설정합니다.")]
    [SerializeField] protected WeaponBaseStats baseStats = new WeaponBaseStats();

    // 증강이 적용된 최종 스탯입니다.
    public WeaponBaseStats finalStats { get; private set; }

    private List<IAugment> m_Augments = new List<IAugment>();
    private List<IAugment> m_GlobalAugments;

    private void Awake()
    {
        // finalStats를 baseStats의 복사본으로 초기화합니다.
        WeaponSettingLogic();
        finalStats = new WeaponBaseStats(baseStats);
    }

    public void SetGlobalAugments(List<IAugment> globalAugments)
    {
        m_GlobalAugments = globalAugments;
        RecalculateStats();
    }

    /// <summary>
    /// 무기에 새로운 증강을 추가합니다.
    /// </summary>
    public void AddAugment(IAugment augment)
    {
        m_Augments.Add(augment);
        RecalculateStats();
    }

    /// <summary>
    /// 무기에서 증강을 제거합니다.
    /// </summary>
    public void RemoveAugment(IAugment augment)
    {
        m_Augments.Remove(augment);
        RecalculateStats();
    }
    

    /// <summary>
    /// 기본 스탯부터 시작하여 모든 증강을 적용해 최종 스탯을 다시 계산합니다.
    /// </summary>
    private void RecalculateStats()
    {
        // 1. 최종 스탯을 기본 스탯으로 초기화
        finalStats = new WeaponBaseStats(baseStats);
        
        // 2. 모든 증강의 스탯 수정치를 순서대로 적용
        foreach (var augment in m_Augments)
        {
            augment.ModifyStats(finalStats);
        }

        foreach (var globalAugment in m_GlobalAugments)
        {
            globalAugment.ModifyStats(finalStats);
        }
        Debug.Log(finalStats.damage);
    }
    
    // 각 무기 타입에 맞는 초기화 로직
    public abstract void WeaponSettingLogic();
    
    // 실제 공격 로직
    public virtual void AttackLogic()
    {
        foreach (var augment in m_GlobalAugments)
        {
            augment.OnAttack(this);
        }
        // 모든 증강의 OnAttack 효과 호출
        foreach (var augment in m_Augments)
        {
            augment.OnAttack(this);
        }
    }
}

