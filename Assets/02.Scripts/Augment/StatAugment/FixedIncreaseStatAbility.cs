using _02.Scripts.Managers.Choice;
using UnityEngine;

namespace _02.Scripts.Augment.BaseAugment
{
    [CreateAssetMenu(fileName = "FixedIncreaseStatAbility", menuName = "Scriptable Objects/FixedIncreaseStatAbility")]
    public class FixedIncreaseStatAbility : StatAbility
    {
        public override void Apply(BaseStats player)
        {
            switch (targetStatType)
            {
                case e_StatType.Health:
                    player.hp.Increase(intAmount);
                    break;
                case e_StatType.MaxHp:
                    player.hp.IncreaseMaxValue(intAmount);
                    break;
            }
        }
    }
}
