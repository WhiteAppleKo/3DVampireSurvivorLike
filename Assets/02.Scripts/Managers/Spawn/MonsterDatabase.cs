using System.Collections.Generic;
using _02.Scripts.Augment.BaseAugment;
using _02.Scripts.Managers.Spawn;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDatabase", menuName = "Scriptable Objects/MonsterDatabase")]
public class MonsterDatabase : ScriptableObject
{
    public List<MonsterData> monsterDatas = new List<MonsterData>();
}
