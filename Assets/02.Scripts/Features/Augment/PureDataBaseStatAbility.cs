using System.Collections.Generic;
using UnityEngine;

namespace Features.Augment
{
    [CreateAssetMenu(fileName = "PureDataBaseStatAbility", menuName = "PureDataBase/Augment/StatAbility")]
    public class PureDataBaseStatAbility : ScriptableObject
    {
        public List<PureDataStatAbility> AbilityList = new List<PureDataStatAbility>();

        public PureDataStatAbility GetData(string id)
        {
            return AbilityList.Find(x => x.ID == id);
        }
    }
}
