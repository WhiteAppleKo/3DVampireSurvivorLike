using System.Collections.Generic;
using UnityEngine;

namespace Features.Enemy
{
    [CreateAssetMenu(fileName = "PureDataBaseEnemy", menuName = "PureDataBase/Entity/Enemy")]
    public class PureDataBaseEnemy : ScriptableObject
    {
        public List<PureDataEnemy> MonsterList = new List<PureDataEnemy>();

        public PureDataEnemy GetData(string id)
        {
            return MonsterList.Find(x => x.ID == id);
        }
    }
}
