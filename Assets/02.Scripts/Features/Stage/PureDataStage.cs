using System.Collections.Generic;
using UnityEngine;
using Features.Enemy;

namespace Features.Stage
{
    [System.Serializable]
    public struct WaveData
    {
        public float startTime;
        public float endTime;
        public List<PureDataEnemy> monsters;
        public float spawnInterval;
        public int maxCount; // 해당 웨이브의 최대 유지 몬스터 수
    }

    [System.Serializable]
    public struct PatternEvent
    {
        public float triggerTime;
        public PureDataEnemy monster;
        public string patternType; // "Circle", "Wall", "X" 등
        public int count;
    }

    [CreateAssetMenu(fileName = "PureDataStage", menuName = "PureData/Environment/Stage")]
    public class PureDataStage : ScriptableObject
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public List<WaveData> Waves { get; set; } = new List<WaveData>();
        [field: SerializeField] public List<PatternEvent> PatternEvents { get; set; } = new List<PatternEvent>();
        [field: SerializeField] public PureDataEnemy BossMonster { get; set; }
        [field: SerializeField] public bool IsBossStage { get; set; }
    }
}
