using System.Collections.Generic;
using _02.Scripts.Augment.BaseAugment;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponAbilityDatabase", menuName = "Scriptable Objects/WeaponAbilityDatabase")]
public class WeaponAbilityDatabase : ScriptableObject
{
    public List<WeaponAbility> weaponAbilities = new List<WeaponAbility>();

    public WeaponAbility GetStatAbility(string id)
    {
        WeaponAbility augment = weaponAbilities.Find(a => a.abilityID == id);
        return augment;
    }
}
