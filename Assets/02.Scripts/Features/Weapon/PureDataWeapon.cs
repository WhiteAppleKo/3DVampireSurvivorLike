using UnityEngine;

namespace Features.Weapon
{
    [CreateAssetMenu(fileName = "PureDataWeapon", menuName = "PureData/Combat/Weapon")]
    public class PureDataWeapon : ScriptableObject
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Type { get; set; }
        [field: SerializeField] public float AttackDelay { get; set; }
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField] public float EffectRange { get; set; }
        [field: SerializeField] public int ProjectileCount { get; set; }
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField] public Sprite Icon { get; set; }
        [field: SerializeField] public int IconNumber { get; set; }
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public AudioClip AttackSound { get; set; }
    }
}