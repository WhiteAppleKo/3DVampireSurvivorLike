using System.Collections.Generic;
using _02.Scripts.Managers.Spawn;

namespace _02.Scripts.Managers.Stage
{
    public class StageData : BaseStage
    {
        public void SetSo(string iD, List<MonsterData> monsterListData, MonsterData bossMonsterID, string isBoss)
        {
            stageID = iD;
            monsterList = monsterListData;
            bossMonster = bossMonsterID;

            switch (isBoss)
            {
                case "TRUE":
                    isBossStage = true;
                    break;
                case "FALSE":
                    isBossStage = false;
                    break;
            }
        }
    }
}
