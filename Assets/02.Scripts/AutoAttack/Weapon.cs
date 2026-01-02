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
    public WeaponBaseStats FinalStats { get; private set; }

    private List<IWeaponAugment> m_Augments = new List<IWeaponAugment>();
    private List<IWeaponAugment> m_GlobalAugments;

    private void Awake()
    {
        // finalStats를 baseStats의 복사본으로 초기화합니다.
        WeaponSettingLogic();
        FinalStats = new WeaponBaseStats(baseStats);
    }

    public void SetGlobalAugments(List<IWeaponAugment> globalAugments)
    {
        m_GlobalAugments = globalAugments;
        RecalculateStats();
    }

    /// <summary>
    /// 무기에 새로운 증강을 추가합니다.
    /// </summary>
    public void AddAugment(IWeaponAugment augment)
    {
        m_Augments.Add(augment);
        RecalculateStats();
    }

    /// <summary>
    /// 무기에서 증강을 제거합니다.
    /// </summary>
    public void RemoveAugment(IWeaponAugment augment)
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
        FinalStats = new WeaponBaseStats(baseStats);
        
        // 2. 모든 증강의 스탯 수정치를 순서대로 적용
        foreach (var augment in m_Augments)
        {
            augment.ModifyStats(FinalStats);
        }

        foreach (var globalAugment in m_GlobalAugments)
        {
            globalAugment.ModifyStats(FinalStats);
        }
        Debug.Log(FinalStats.damage);
    }
    
    // 각 무기 타입에 맞는 초기화 로직
    public abstract void WeaponSettingLogic();
    
    // 실제 공격 로직
    public virtual void AttackLogic()
    {
        foreach (var augment in m_GlobalAugments)
        {
            
        }
        // 모든 증강의 OnAttack 효과 호출
        foreach (var augment in m_Augments)
        {
            
        }
    }
}

