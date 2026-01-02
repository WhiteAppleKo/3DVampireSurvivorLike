using UnityEngine;

namespace _02.Scripts.Managers.Choice
{
    [CreateAssetMenu(fileName = "StatAbility", menuName = "Scriptable Objects/StatAbility")]
    public class StatAbility : BaseAbility
    {
        public enum e_StatType
        {
            MoveSpeed,
            Health,
            MaxHp
        }
        
        public e_StatType targetStatType;
        public float amount;
        
        public override void Apply(Controller player)
        {
            throw new System.NotImplementedException();
        }
    }
}
