using System.Collections.Generic;
using _02.Scripts.Managers.Spawn;
using UnityEngine;

namespace _02.Scripts.Managers.Stage
{
    public abstract class BaseStage : ScriptableObject
    {
        public string stageID;
        public List<MonsterData> monsterList;
        public MonsterData bossMonster;
        public bool isBossStage;
    }
}
