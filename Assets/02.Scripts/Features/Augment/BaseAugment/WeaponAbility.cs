using _02.Scripts.Managers.Choice;
using UnityEngine;

/// <summary>
/// [D] 무기 스탯 증강 데이터 클래스 (레거시 코드 호환용)
/// </summary>
public class WeaponAbility : BaseAbility
{
    public enum e_WeaponStatType
    {
        AttackDelay,
        Damage,
        AoE,
        WrongType
    }
        
    public e_WeaponStatType targetStatType;
    public int intAmount;
    public float floatAmount;
}
