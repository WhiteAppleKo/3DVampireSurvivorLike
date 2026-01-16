using System.Collections.Generic;
using _02.Scripts.Managers.Spawn;
using UnityEngine;

namespace _02.Scripts.AutoAttack
{
    [CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Scriptable Objects/WeaponDatabase")]
    public class WeaponDatabase : ScriptableObject
    {
        public List<WeaponData> weaponDatas = new List<WeaponData>();
        
        public WeaponData GetWeaponData(string id)
        {
            WeaponData augment = weaponDatas.Find(a => a.weaponID == id);
            return augment;
        }
    }
}
