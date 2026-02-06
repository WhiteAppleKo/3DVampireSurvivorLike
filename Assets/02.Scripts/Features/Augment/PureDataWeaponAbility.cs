using UnityEngine;
using _02.Scripts.Augment.BaseAugment;

namespace Features.Augment
{
    [CreateAssetMenu(fileName = "PureDataWeaponAbility", menuName = "PureData/Augment/WeaponAbility")]
    public class PureDataWeaponAbility : ScriptableObject
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Type { get; set; }
        [field: SerializeField] public int IconNumber { get; set; }
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public WeaponAbility.e_WeaponStatType TargetStatType { get; set; }
        [field: SerializeField] public string ValueType { get; set; } // Fixed or Percentage
        [field: SerializeField] public float ValueAmount { get; set; }
    }
}
