using System;
using _02.Scripts.Managers.Stage;
using UnityEngine;

namespace _02.Scripts.Managers.Spawn
{
    public class SpawnManager : MonoBehaviour
    {
        private RepeatSpawner m_ReapeatSpawner;
        private PatternSpawner m_PatternSpawner;

        private void Awake()
        {
            m_ReapeatSpawner = GetComponentInChildren<RepeatSpawner>();
            m_PatternSpawner = GetComponentInChildren<PatternSpawner>();
        }

        public void StartNewStage(StageData stageData)
        {
            m_ReapeatSpawner.StartSpawning(stageData);
        }
    }
}
