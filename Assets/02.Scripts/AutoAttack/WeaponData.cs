using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _02.Scripts.AutoAttack
{
    public class WeaponData : BaseWeaponData{
        public void SetSo(string id, string name, string type, string delay, string damage, string findRange, string projectileCount,
            GameObject prefab, int iconSpriteNumber, string description)
        {
            weaponID = id;
            weaponName = name;
            weaponType = type;
            attackDelay = float.Parse(delay);
            weaponDamage = int.Parse(damage);
            effectRange = float.Parse(findRange);
            this.projectileCount = int.Parse(projectileCount);
            weaponPrefab = prefab;
            iconNumber = iconSpriteNumber;
            weaponDescription = description;

            if (weaponPrefab == null)
            {
                Debug.Log("프리팹 비어있음");
            }
            weaponPrefab.GetComponent<Weapon>().baseStats.WeaponDataLoadLogic(this);
        }
    }
}
