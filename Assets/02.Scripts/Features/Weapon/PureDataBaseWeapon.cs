using System.Collections.Generic;
using UnityEngine;

namespace Features.Weapon
{
    [CreateAssetMenu(fileName = "PureDataBaseWeapon", menuName = "PureDataBase/Combat/Weapon")]
    public class PureDataBaseWeapon : ScriptableObject
    {
        public List<PureDataWeapon> WeaponList = new List<PureDataWeapon>();

        public PureDataWeapon GetData(string id)
        {
            return WeaponList.Find(x => x.ID == id);
        }
    }
}
