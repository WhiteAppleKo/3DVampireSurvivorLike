using System.Collections.Generic;
using UnityEngine;
using Features.Enemy;

namespace Features.Stage
{
    [CreateAssetMenu(fileName = "PureDataStage", menuName = "PureData/Environment/Stage")]
    public class PureDataStage : ScriptableObject
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public List<PureDataEnemy> MonsterList { get; set; } = new List<PureDataEnemy>();
        [field: SerializeField] public PureDataEnemy BossMonster { get; set; }
        [field: SerializeField] public bool IsBossStage { get; set; }
    }
}
