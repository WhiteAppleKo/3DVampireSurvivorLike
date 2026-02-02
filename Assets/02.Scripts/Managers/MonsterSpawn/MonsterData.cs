using System;
using _02.Scripts.Managers.MonsterSpawn;
using UnityEngine;

namespace _02.Scripts.Managers.Spawn
{
    public class MonsterData : BaseMonster
    {
        public void SetSo(string iD, string name, string hp, string moveSpeed, string attackDelay, 
            string damage, string exp, string minTime, GameObject prefab, string SpawnWeight)
        {
            monsterID = iD;
            monsterName = name;
            monsterHp = int.Parse(hp);
            monsterMoveSpeed = int.Parse(moveSpeed);
            monsterAttackDelay = int.Parse(attackDelay);
            monsterDamage = int.Parse(damage);
            monsterExp = int.Parse(exp);
            monsterSpawnMinTime = float.Parse(minTime);
            monsterPrefab = prefab;
            spawnWeight = int.Parse(SpawnWeight);
        }
    }
}
