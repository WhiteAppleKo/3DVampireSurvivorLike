using System;
using System.Collections.Generic;
using System.Threading;
using _02.Scripts.Cotroller;
using Cysharp.Threading.Tasks;
using _02.Scripts.Managers.Spawn;
using _02.Scripts.Managers.Stage;
using Shapes;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RepeatSpawner : MonoBehaviour
{
    public float spawnDelay = 1.0f;
    public int poolSize = 10;
    public Controller player;
    
    private StageData m_StageData;
    private int m_SpawnIndex = 0;
    private float m_ReadDatabaseDelay = 5.0f;
    private List<GameObject> m_EnemyList = new List<GameObject>();
    private List<MonsterData> m_EnemysDatas = new List<MonsterData>();

    private CancellationTokenSource m_Cts;

    private void OnDestroy()
    {
        StopSpawning();
    }

    public void StartSpawning(StageData stageData)
    {
        m_StageData = stageData;
        StopSpawning(); // 기존 작업이 있다면 취소
        m_Cts = new CancellationTokenSource();
        Async_LoadMonster(m_Cts.Token).Forget();
        Async_RepeatSpawn(m_Cts.Token).Forget();
        SetEnemys();
    }

    public void StopSpawning()
    {
        if (m_Cts != null)
        {
            m_Cts.Cancel();
            m_Cts.Dispose();
            m_Cts = null;
        }
        m_EnemyList.Clear();
    }

    private void SetEnemys()
    {
        float currentTime = IMTimer.Instance.ElapsedTime;
        foreach (var data in m_StageData.monsterList)
        {
            if (data.monsterSpawnMinTime <= currentTime && !m_EnemysDatas.Contains(data))
            {
                m_EnemysDatas.Add(data);
                for (int i = 0; i < poolSize; i++)
                {
                    var enemy = Instantiate(data.monsterPrefab, transform);
                    m_EnemyList.Add(enemy);
                    enemy.name = data.monsterName;
                    enemy.GetComponent<EnemyController>().SetTarget(player);
                    enemy.SetActive(false);
                }
                Debug.Log($"[Spawn] {data.monsterName} 몬스터가 목록에 추가되었습니다. (기준 시간: {data.monsterSpawnMinTime}초)");
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = GetEnemy();
        enemy.transform.position = GetSpawnPoint();
        enemy.SetActive(true);
    }

    private Vector3 GetSpawnPoint()
    {
        Camera mainCam = Camera.main;
        // 카메라가 없으면 스포너 위치 반환
        if (mainCam == null) return transform.position;

        float spawnRadius = 0f;

        // 카메라 타입에 따른 화면 대각선 길이 계산 (자동 범위 설정)
        if (mainCam.orthographic)
        {
            float height = mainCam.orthographicSize; // 세로 절반
            float width = height * mainCam.aspect;   // 가로 절반
            // 중심에서 코너까지의 거리 계산 후 여유분(1.2배) 추가
            spawnRadius = Mathf.Sqrt(width * width + height * height) * 1.2f; 
        }
        else // Perspective (원근법)
        {
            // 카메라 높이(Y)를 기준으로 바닥(Y=0)에 비치는 시야 범위 계산
            float distanceToGround = Mathf.Abs(mainCam.transform.position.y);
            // 수직 시야각(FOV)의 절반을 라디안으로 변환
            float halfFovRad = mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad;
            // 바닥에 투영된 화면의 세로 절반 길이 (tan = 높이 / 밑변 -> 밑변 = 높이 * tan)
            float height = distanceToGround * Mathf.Tan(halfFovRad);
            float width = height * mainCam.aspect;
            
            // 중심에서 코너까지의 거리 + 여유분 (원근감 고려하여 1.5배)
            spawnRadius = Mathf.Sqrt(width * width + height * height) * 1.5f; 
        }

        // 반지름 크기의 원 내부 랜덤 좌표를 구한 뒤 정규화(normalized)하여 원 테두리 좌표로 만듦
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;

        // 카메라(플레이어) 위치 기준 오프셋 적용 (Y축은 0으로 고정)
        Vector3 spawnPos = new Vector3(mainCam.transform.position.x + randomCircle.x, 0, mainCam.transform.position.z + randomCircle.y);
        
        return spawnPos;
    }
    private GameObject GetEnemy()
    {
        m_SpawnIndex = Random.Range(0, m_EnemysDatas.Count);
        MonsterData spawnTarget = m_EnemysDatas[m_SpawnIndex];
        // 리스트를 순회하며 비활성화된 몬스터
        foreach (var enemy in m_EnemyList)
        {
            if (enemy.name == spawnTarget.monsterName)
            {
                if (!enemy.activeInHierarchy)
                {
                    return enemy;
                }
            }
        }
        GameObject poolEnemy = Instantiate(spawnTarget.monsterPrefab, transform);
        poolEnemy.name = spawnTarget.monsterName;
        m_EnemyList.Add(poolEnemy);
        poolEnemy.GetComponent<EnemyController>().SetTarget(player);
        return poolEnemy;
    }
    private async UniTaskVoid Async_LoadMonster(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                // 밀리초 단위 변환 (초 * 1000)
                await UniTask.Delay(TimeSpan.FromSeconds(m_ReadDatabaseDelay), cancellationToken: token);
                SetEnemys();
            }
        }
        catch (OperationCanceledException)
        {
            // 작업 취소 시 조용히 종료
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    private async UniTaskVoid Async_RepeatSpawn(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                // 밀리초 단위 변환 (초 * 1000)
                await UniTask.Delay(TimeSpan.FromSeconds(spawnDelay), cancellationToken: token);

                // 데이터가 아직 로드되지 않았으면 스폰 건너뛰기 (에러 방지)
                if (m_EnemysDatas.Count == 0) continue;

                SpawnEnemy();
            }
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
