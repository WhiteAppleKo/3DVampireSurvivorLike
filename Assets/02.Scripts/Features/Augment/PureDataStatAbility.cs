using UnityEngine;
using _02.Scripts.Augment.BaseAugment;

namespace Features.Augment
{
    [CreateAssetMenu(fileName = "PureDataStatAugment", menuName = "PureData/Augment/StatAugment")]
    public class PureDataStatAbility : PureDataAugment
    {
        [field: SerializeField] public StatAbility.e_StatType TargetStatType { get; set; }
        [field: SerializeField] public bool IsTemporary { get; set; }
    }
}
