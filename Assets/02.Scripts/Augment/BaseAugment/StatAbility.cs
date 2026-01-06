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

        public virtual void Apply(BaseStats player)
        {
            // [디버깅] 적용하려는 스탯 정보 출력
            // Debug.LogWarning($"[StatAbility] Apply 호출됨. 타입: {targetStatType}, 수치: {intAmount}");

            switch (targetStatType)
            {
                case e_StatType.Health:
                    player.hp.Increase(intAmount);
                    break;
                case e_StatType.MaxHp:
                    // Debug.LogWarning($"[StatAbility] MaxHp 증가 시도! 현재 Max: {player.hp.Max} -> +{intAmount}");
                    player.hp.IncreaseMaxValue(intAmount);
                    break;
                case e_StatType.MoveSpeed:
                    break;
                case e_StatType.WrongType:
                    break;
            }
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
