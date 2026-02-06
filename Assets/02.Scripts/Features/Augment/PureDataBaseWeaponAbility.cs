using System.Collections.Generic;
using UnityEngine;

namespace Features.Augment
{
    [CreateAssetMenu(fileName = "PureDataBaseWeaponAbility", menuName = "PureDataBase/Augment/WeaponAbility")]
    public class PureDataBaseWeaponAbility : ScriptableObject
    {
        public List<PureDataWeaponAbility> AbilityList = new List<PureDataWeaponAbility>();

        public PureDataWeaponAbility GetData(string id)
        {
            return AbilityList.Find(x => x.ID == id);
        }
    }
}
