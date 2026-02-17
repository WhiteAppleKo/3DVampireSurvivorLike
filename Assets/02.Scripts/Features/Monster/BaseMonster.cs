using UnityEngine;

namespace _02.Scripts.Managers.MonsterSpawn
{
    /// <summary>
    /// [D] 모든 몬스터의 베이스 데이터 클래스 (레거시 코드 호환용)
    /// </summary>
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
        public int spawnWeight;
    }
}
