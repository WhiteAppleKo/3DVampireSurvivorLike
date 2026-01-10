using UnityEngine;

namespace _02.Scripts.Managers.Spawn
{
    public abstract class BaseMonster : ScriptableObject
    {
        public string monsterID;
        public string monsterName;
        public int monsterHp;
        public int monsterMoveSpeed;
        public int monsterAttackDelay;
        public int monsterDamage;
        public int monsterExp;
        public GameObject monsterPrefab;
        public float monsterSpawnMinTime;
    }
}
