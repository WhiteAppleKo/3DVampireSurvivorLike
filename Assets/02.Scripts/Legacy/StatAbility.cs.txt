using System;
using System.Collections.Generic;
using _02.Scripts.Managers.Choice;
using UnityEditor;

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

        public void SetSo(string id, string name, string typeOfAbility, int numberOfIcon, string descriptions, e_StatType statType, string valueTypes, string valueAmount, string istemporary)
        {
            abilityID = id;
            abilityName = name;
            abilityType = typeOfAbility;
            iconNumber = numberOfIcon;
            description = descriptions;
            targetStatType = statType;
            valueType = valueTypes;

            switch (istemporary)
            {
                case "TRUE":
                    isTemporary = true;
                    break;
                case "FALSE":
                    isTemporary = false;
                    break;
            }
            

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
