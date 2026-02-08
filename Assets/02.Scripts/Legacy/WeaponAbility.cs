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
