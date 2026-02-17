using _02.Scripts.Managers.Choice;

namespace _02.Scripts.Augment.BaseAugment
{
    /// <summary>
    /// [D] 플레이어 스탯 증강 데이터 클래스 (레거시 코드 호환용)
    /// </summary>
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
    }
}
