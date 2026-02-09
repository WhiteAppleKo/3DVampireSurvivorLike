using System;
using System.Collections.Generic;
using Features.Stage;
using Features.Enemy;
using Shapes;
using UnityEngine;

namespace _02.Scripts.Managers.Spawn
{
    // [L] Logic Layer: Decision making for spawning
    public class MonsterSpawnSystem : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PureDataStage currentStage;
        
        [Header("Visual References")]
        [SerializeField] private MonsterSpawner spawner;

        private float m_ElapsedTime = 0f;
        private HashSet<int> m_TriggeredEvents = new HashSet<int>();
        private Dictionary<int, float> m_WaveSpawnTimers = new Dictionary<int, float>();

        public void StartStage(PureDataStage stage)
        {
            currentStage = stage;
            m_ElapsedTime = 0f;
            m_TriggeredEvents.Clear();
            m_WaveSpawnTimers.Clear();
            if (spawner != null) spawner.ClearPool();
            
            if (currentStage != null)
                Debug.Log($"[SpawnSystem] 스테이지 시작: {currentStage.ID}, 웨이브 수: {currentStage.Waves.Count}");
            else
                Debug.LogError("[SpawnSystem] 시작하려는 스테이지 데이터가 Null입니다!");
        }

        private void Update()
        {
            if (currentStage == null) return;
            
            if (Shapes.IMTimer.Instance == null)
            {
                // 매우 드문 경우지만 타이머가 없을 때 로그 출력 (1번만)
                if (Time.frameCount % 100 == 0) Debug.LogWarning("[SpawnSystem] IMTimer.Instance가 Null입니다!");
                return;
            }

            m_ElapsedTime = Shapes.IMTimer.Instance.ElapsedTime;

            // 1. 일반 웨이브 체크
            ProcessWaves();

            // 2. 패턴 이벤트 체크
            ProcessPatternEvents();
        }

        private void ProcessWaves()
        {
            for (int i = 0; i < currentStage.Waves.Count; i++)
            {
                var wave = currentStage.Waves[i];
                // 로그: 현재 시간과 웨이브 시간 비교 (테스트용)
                // Debug.Log($"[SpawnSystem] Wave {i} 체크 - Time: {m_ElapsedTime}, Start: {wave.startTime}, End: {wave.endTime}");

                if (m_ElapsedTime >= wave.startTime && m_ElapsedTime <= wave.endTime)
                {
                    if (!m_WaveSpawnTimers.ContainsKey(i)) m_WaveSpawnTimers[i] = 0f;

                    m_WaveSpawnTimers[i] += Time.deltaTime;
                    if (m_WaveSpawnTimers[i] >= wave.spawnInterval)
                    {
                        m_WaveSpawnTimers[i] = 0f;
                        Debug.Log($"[SpawnSystem] Wave {i} 조건 충족! 몬스터 스폰 시도.");
                        SpawnWaveMonster(wave);
                    }
                }
            }
        }

        private void SpawnWaveMonster(WaveData wave)
        {
            if (wave.monsters == null || wave.monsters.Count == 0) return;
            
            // 랜덤하게 몬스터 선택
            var data = wave.monsters[UnityEngine.Random.Range(0, wave.monsters.Count)];
            Vector3 spawnPos = CalculateRandomSpawnPoint();
            
            spawner.Spawn(data, spawnPos);
        }

        private void ProcessPatternEvents()
        {
            for (int i = 0; i < currentStage.PatternEvents.Count; i++)
            {
                var evt = currentStage.PatternEvents[i];
                if (!m_TriggeredEvents.Contains(i) && m_ElapsedTime >= evt.triggerTime)
                {
                    m_TriggeredEvents.Add(i);
                    TriggerPattern(evt);
                }
            }
        }

        private void TriggerPattern(PatternEvent evt)
        {
            // [Logic] 패턴 타입에 따른 좌표 계산 분기
            switch (evt.patternType)
            {
                case "Circle":
                    SpawnCirclePattern(evt);
                    break;
                // "Wall", "X" 등 추가 가능
                default:
                    Debug.LogWarning($"[SpawnSystem] 지원하지 않는 패턴 타입: {evt.patternType}");
                    break;
            }
        }

        private void SpawnCirclePattern(PatternEvent evt)
        {
            float radius = 10f; // 기본값
            Vector3 center = Camera.main.transform.position;
            center.y = 0;

            for (int i = 0; i < evt.count; i++)
            {
                float angle = i * (360f / evt.count) * Mathf.Deg2Rad;
                Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                spawner.Spawn(evt.monster, pos);
            }
        }

        private Vector3 CalculateRandomSpawnPoint()
        {
            // 기존 RepeatSpawner의 로직을 수학적으로 정제하여 적용
            Camera mainCam = Camera.main;
            float distanceToGround = Mathf.Abs(mainCam.transform.position.y);
            float halfFovRad = mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad;
            float height = distanceToGround * Mathf.Tan(halfFovRad);
            float width = height * mainCam.aspect;
            float spawnRadius = Mathf.Sqrt(width * width + height * height) * 1.5f;

            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;
            return new Vector3(mainCam.transform.position.x + randomCircle.x, 0, mainCam.transform.position.z + randomCircle.y);
        }
    }
}
