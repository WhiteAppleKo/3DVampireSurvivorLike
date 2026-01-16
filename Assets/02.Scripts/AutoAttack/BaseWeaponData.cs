using UnityEngine;

namespace _02.Scripts.AutoAttack
{
    public abstract class BaseWeaponData : ScriptableObject
    {
        public string weaponID;
        public string weaponName;
        public string weaponType;
        public float attackDelay;
        public int weaponDamage;
        public float effectRange;
        public int projectileCount;
        public GameObject weaponPrefab;
    }
}
