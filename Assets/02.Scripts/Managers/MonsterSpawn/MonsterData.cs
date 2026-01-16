using System;
using UnityEngine;

namespace _02.Scripts.Managers.Spawn
{
    public class MonsterData : BaseMonster
    {
        public void SetSo(string iD, string name, string hp, string moveSpeed, string attackDelay, 
            string damage, string exp, string minTime, GameObject prefab)
        {
            monsterID = iD;
            monsterName = name;
            monsterHp = int.Parse(hp);
            monsterMoveSpeed = int.Parse(moveSpeed);
            monsterAttackDelay = Int32.Parse(attackDelay);
            monsterDamage = Int32.Parse(damage);
            monsterExp = Int32.Parse(exp);
            monsterSpawnMinTime = float.Parse(minTime);
            monsterPrefab = prefab;
        }
    }
}
