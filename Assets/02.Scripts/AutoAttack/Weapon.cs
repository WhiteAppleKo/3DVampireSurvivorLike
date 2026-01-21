using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 무기의 기본 클래스입니다. 증강 시스템과 스탯 관리를 포함합니다.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    public WeaponBaseStats baseStats = new WeaponBaseStats();
    public AudioSource  audioSource;
    
    private WeaponBaseStats.WeaponModifier m_GlobalAugmentsModifier;
    // 증강이 적용된 최종 스탯입니다.
    public WeaponBaseStats FinalStats { get; private set; }

    private List<WeaponAbility> m_Augments = new List<WeaponAbility>();

    public List<string> GetWeaponLocalAugmentsID()
    {
        List<string> IDList = new List<string>();
        foreach (var augmnet in m_Augments)
        {
            IDList.Add(augmnet.abilityID);
        }
        return IDList;
    }
    public List<WeaponAbility> GetWeaponLocalAugments()
    {
        return m_Augments;
    }
    private void Awake()
    {
        // finalStats를 baseStats의 복사본으로 초기화합니다.
        WeaponSettingLogic();
        FinalStats = new WeaponBaseStats(baseStats);
        m_GlobalAugmentsModifier = new WeaponBaseStats.WeaponModifier(1, 1, 1);
        audioSource = GetComponent<AudioSource>();
    }

    
    public void SetGlobalAugments(WeaponBaseStats.WeaponModifier globalAugmentsModifier)
    {
        m_GlobalAugmentsModifier = globalAugmentsModifier;
        RecalculateStats();
    }

    /// <summary>
    /// 무기에 새로운 증강을 추가합니다.
    /// </summary>
    public void AddAugment(WeaponAbility augment)
    {
        m_Augments.Add(augment);
        RecalculateStats();
    }

    /// <summary>
    /// 무기에서 증강을 제거합니다.
    /// </summary>
    public void RemoveAugment(WeaponAbility augment)
    {
        m_Augments.Remove(augment);
        RecalculateStats();
    }
    

    /// <summary>
    /// 기본 스탯부터 시작하여 모든 증강을 적용해 최종 스탯을 다시 계산합니다.
    /// </summary>
    protected virtual void RecalculateStats()
    {
        // 1. 최종 스탯을 기본 스탯으로 초기화
        if (FinalStats == null)
        {
            FinalStats = new WeaponBaseStats(baseStats);
        }
        else
        {
            FinalStats.ResetTo(baseStats);
        }
        
        // 2. 모든 증강의 스탯 수정치를 순서대로 적용
        // >>>>> 최종 글로벌 증가값을 AutoAttack에서 받아오는걸로 바뀜
        FinalStats.attackDelay = FinalStats.attackDelay / m_GlobalAugmentsModifier.percentAttackDelay;
        FinalStats.damage = FinalStats.damage + m_GlobalAugmentsModifier.fixedDamageIncrease;
        FinalStats.damage = (int)(FinalStats.damage * m_GlobalAugmentsModifier.percentDamageIncreadse);
        
        foreach (var augment in m_Augments)
        {
            
        }
    }
    
    // 각 무기 타입에 맞는 초기화 로직
    public abstract void WeaponSettingLogic();
    
    // 실제 공격 로직
    public virtual void AttackLogic()
    {
        // 모든 증강의 OnAttack 효과 호출
        foreach (var augment in m_Augments)
        {
            
        }
    }
}