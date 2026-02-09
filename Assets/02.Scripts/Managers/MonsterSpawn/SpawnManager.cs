using System;
using _02.Scripts.Managers.Stage;
using Features.Stage;
using UnityEngine;

namespace _02.Scripts.Managers.Spawn
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private MonsterSpawnSystem spawnSystem;

        private void Awake()
        {
            if (spawnSystem == null) spawnSystem = GetComponent<MonsterSpawnSystem>();
        }

        public void StartNewStage(PureDataStage stageData)
        {
            if (spawnSystem != null)
            {
                spawnSystem.StartStage(stageData);
            }
        }
    }
}
