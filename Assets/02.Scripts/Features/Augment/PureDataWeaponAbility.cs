using UnityEngine;
using _02.Scripts.Augment.BaseAugment;

namespace Features.Augment
{
    [CreateAssetMenu(fileName = "PureDataWeaponAugment", menuName = "PureData/Augment/WeaponAugment")]
    public class PureDataWeaponAbility : PureDataAugment
    {
        [field: SerializeField] public WeaponAbility.e_WeaponStatType TargetStatType { get; set; }
    }
}
