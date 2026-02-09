using UnityEngine;

namespace Features.Weapon
{
    [CreateAssetMenu(fileName = "PureDataWeapon", menuName = "PureData/Combat/Weapon")]
    public class PureDataWeapon : Features.Augment.PureDataAugment
    {
        [field: SerializeField] public float AttackDelay { get; set; }
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField] public float EffectRange { get; set; }
        [field: SerializeField] public int ProjectileCount { get; set; }
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField] public AudioClip AttackSound { get; set; }
    }
}