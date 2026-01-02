using System.Collections.Generic;
using _02.Scripts.Managers.Choice;

namespace _02.Scripts.Augment.BaseAugment
{
    public class StatAbility : BaseAbility
    {
        public enum e_StatType
        {
            Health,
            MaxHp,
            MoveSpeed,
            WrongType
        }
        
        public e_StatType targetStatType;
        public int intAmount;
        public float floatAmount;

        public virtual void Apply(BaseStats player)
        {
            
        }
        public void SetSo(string id, string name, string typeOfAbility, int numberOfIcon, string descriptions, e_StatType statType, string valueTypes, string valueAmount)
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
}
