using System.Collections.Generic;
using UnityEngine;

namespace Features.Stage
{
    [CreateAssetMenu(fileName = "PureDataBaseStage", menuName = "PureDataBase/Environment/Stage")]
    public class PureDataBaseStage : ScriptableObject
    {
        public List<PureDataStage> StageList = new List<PureDataStage>();

        public PureDataStage GetData(string id)
        {
            return StageList.Find(x => x.ID == id);
        }
    }
}
