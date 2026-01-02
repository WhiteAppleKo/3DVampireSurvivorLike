using _02.Scripts.Managers.Choice;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipAbility", menuName = "Scriptable Objects/EquipAbility")]
public class WeaponAbility : BaseAbility
{
    public enum e_WeaponStatType
    {
        AttackDelay,
        Damage,
    }
        
    public e_WeaponStatType targetStatType;
    public float amount;
    
    public virtual void Apply(WeaponBaseStats player)
    {
        
    }
}
