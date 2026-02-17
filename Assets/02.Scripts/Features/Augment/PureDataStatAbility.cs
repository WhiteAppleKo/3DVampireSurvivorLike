using UnityEngine;
using _02.Scripts.Augment.BaseAugment;

namespace Features.Augment
{
    [CreateAssetMenu(fileName = "PureDataStatAugment", menuName = "PureData/Augment/StatAugment")]
    public class PureDataStatAbility : PureDataAugment
    {
        [field: SerializeField] public StatAbility.e_StatType TargetStatType { get; set; }
        [field: SerializeField] public bool IsTemporary { get; set; }

        public override void Apply()
        {
            var player = SubscribeManager.Instance.playerController;
            if (player == null || player.Model == null) return;

            switch (TargetStatType)
            {
                case StatAbility.e_StatType.Health:
                    player.Model.Heal((int)ValueAmount);
                    break;
                case StatAbility.e_StatType.MaxHp:
                    player.Model.AddMaxHpModifier((int)ValueAmount);
                    break;
                case StatAbility.e_StatType.MoveSpeed:
                    player.Model.AddMoveSpeedModifier(ValueAmount);
                    break;
            }
            
            Debug.Log($"[StatAugment] {TargetStatType} 적용: {ValueAmount}");
        }
    }
}
