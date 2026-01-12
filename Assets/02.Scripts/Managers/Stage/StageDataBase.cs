using System.Collections.Generic;
using _02.Scripts.Augment.BaseAugment;
using _02.Scripts.Managers.Spawn;
using _02.Scripts.Managers.Stage;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDataBase", menuName = "Scriptable Objects/StageDataBase")]
public class StageDataBase : ScriptableObject
{
    public List<StageData> stageDatas = new List<StageData>();
}
