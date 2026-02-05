using UnityEngine;

namespace _02.Scripts.AutoAttack
{
    // [D] PureData Layer - 가이드라인에 맞춰 Immutable 속성 적용
    public abstract class BaseWeaponData : ScriptableObject
    {
        [field: SerializeField] public string weaponID { get; protected set; }
        [field: SerializeField] public string weaponName { get; protected set; }
        [field: SerializeField] public string weaponType { get; protected set; }
        [field: SerializeField] public float attackDelay { get; protected set; }
        [field: SerializeField] public int weaponDamage { get; protected set; }
        [field: SerializeField] public float effectRange { get; protected set; }
        [field: SerializeField] public int projectileCount { get; protected set; }
        [field: SerializeField] public GameObject weaponPrefab { get; protected set; }
        [field: SerializeField] public Sprite icon { get; protected set; }
        [field: SerializeField] public int iconNumber { get; protected set; }
        [field: SerializeField] public string weaponDescription { get; protected set; }
        [field: SerializeField] public AudioClip AttackSound { get; protected set; }
    }
}