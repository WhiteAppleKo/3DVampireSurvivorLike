using System.Collections.Generic;
using _02.Scripts.Augment.BaseAugment;
using UnityEngine;

[CreateAssetMenu(fileName = "StatAbilityDatabase", menuName = "Scriptable Objects/StatAbilityDatabase")]
public class StatAbilityDatabase : ScriptableObject
{
    public List<StatAbility> statAbilities = new List<StatAbility>();

    public StatAbility GetStatAbility(string id)
    {
        StatAbility augment = statAbilities.Find(a => a.abilityID == id);
        return augment;
    }
}
