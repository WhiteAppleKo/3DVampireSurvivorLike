using _02.Scripts.Managers.Choice;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipAbility", menuName = "Scriptable Objects/EquipAbility")]
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
    
    public virtual void Apply(WeaponBaseStats.WeaponModifier weaponModifier)
    {
        switch (targetStatType)
        {
            case e_WeaponStatType.AttackDelay:
                weaponModifier.percentAttackDelay += floatAmount;
                break;
            case e_WeaponStatType.Damage:
                switch (valueType)
                {
                    case "Fixed":
                        weaponModifier.fixedDamageIncrease += intAmount;
                        break;
                    case "Percent":
                        weaponModifier.percentDamageIncreadse += floatAmount;
                        break;
                }
                break;
            case e_WeaponStatType.AoE:
                break;
            default:
                break;
        }
    }
    
    public void SetSo(string id, string name, string typeOfAbility, int numberOfIcon, string descriptions, e_WeaponStatType statType, string valueTypes, string valueAmount)
    {
        abilityID = id;
        abilityName = name;
        abilityType = typeOfAbility;
        iconNumber = numberOfIcon;
        description = descriptions;
        targetStatType = statType;
        valueType = valueTypes;

        switch (valueType)
        {
            case "Fixed":
                intAmount = int.Parse(valueAmount);
                break;
            case "Percentage":
                floatAmount = float.Parse(valueAmount);
                break;
        }
    }
}
