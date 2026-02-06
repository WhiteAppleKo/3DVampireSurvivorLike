using System;
using System.Collections.Generic;
using System.Threading;
using _02.Scripts.Cotroller;
using Cysharp.Threading.Tasks;
using _02.Scripts.Managers.Spawn;
using _02.Scripts.Managers.Stage;
using Features.Enemy;
using Features.Stage;
using Shapes;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RepeatSpawner : MonoBehaviour
{
    public float spawnDelay = 1.0f;
    public int poolSize = 10;
    public Controller player;
    
    private PureDataStage m_StageData;
    private int m_SpawnIndex = 0;
    private float m_ReadDatabaseDelay = 5.0f;
    private List<GameObject> m_EnemyList = new List<GameObject>();
    private List<PureDataEnemy> m_EnemysDatas = new List<PureDataEnemy>();

    private CancellationTokenSource m_Cts;
    private int m_TotalEnemySpawnWeight = 0;

    private void OnDestroy()
    {
        StopSpawning();
    }

    public void StartSpawning(PureDataStage stageData)
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
        if (m_StageData == null) return;

        float currentTime = IMTimer.Instance.ElapsedTime;
        foreach (var data in m_StageData.MonsterList)
        {
            // Note: PureDataEnemy currently doesn't have spawnMinTime. 
            // If needed, it should be added to PureDataEnemy or a wrapper.
            // For now, adding all monsters in the list.
            if (!m_EnemysDatas.Contains(data))
            {
                m_EnemysDatas.Add(data);
                SetSpawnWeight();
                for (int i = 0; i < poolSize; i++)
                {
                    if (data.Prefab == null)
                    {
                        Debug.LogError($"[RepeatSpawner] {data.MonsterName}의 프리팹이 PureData에 할당되지 않았습니다. 임포트를 다시 진행하세요.");
                        continue;
                    }
                    var enemy = Instantiate(data.Prefab, transform);
                    m_EnemyList.Add(enemy);
                    enemy.name = data.MonsterName;
                    var logic = enemy.GetComponent<EnemyLogicSystem>();
                    if (logic != null) logic.SetTarget(player);
                    enemy.SetActive(false);
                }
                Debug.Log($"[Spawn] {data.MonsterName} 몬스터가 목록에 추가되었습니다.");
            }
        }
    }

    private void SetSpawnWeight()
    {
        m_TotalEnemySpawnWeight = 0;
        foreach (var data in m_EnemysDatas)
        {
            // Note: PureDataEnemy currently doesn't have spawnWeight. 
            // Using default 10 for weight if not present.
            m_TotalEnemySpawnWeight += 10; 
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = GetEnemy();
        if (enemy == null)
        {
            return;
        }
        enemy.transform.position = GetSpawnPoint();
        enemy.SetActive(true);
    }

    private Vector3 GetSpawnPoint()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return transform.position;

        float spawnRadius = 0f;

        if (mainCam.orthographic)
        {
            float height = mainCam.orthographicSize;
            float width = height * mainCam.aspect;
            spawnRadius = Mathf.Sqrt(width * width + height * height) * 1.2f; 
        }
        else
        {
            float distanceToGround = Mathf.Abs(mainCam.transform.position.y);
            float halfFovRad = mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad;
            float height = distanceToGround * Mathf.Tan(halfFovRad);
            float width = height * mainCam.aspect;
            spawnRadius = Mathf.Sqrt(width * width + height * height) * 1.5f; 
        }

        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = new Vector3(mainCam.transform.position.x + randomCircle.x, 0, mainCam.transform.position.z + randomCircle.y);
        
        return spawnPos;
    }
    private GameObject GetEnemy()
    {
        if (m_TotalEnemySpawnWeight <= 0) return null;

        m_SpawnIndex = Random.Range(0, m_TotalEnemySpawnWeight);
        int cumulativeWeight = 0;
        PureDataEnemy spawnTarget = null;
        foreach (var data in m_EnemysDatas)
        {
            cumulativeWeight += 10; // Default weight
            if (m_SpawnIndex <= cumulativeWeight)
            {
                spawnTarget = data;
                break;
            }
        }

        if (spawnTarget == null || spawnTarget.Prefab == null) return null;

        foreach (var enemy in m_EnemyList)
        {
            if (enemy.name == spawnTarget.MonsterName)
            {
                if (!enemy.activeInHierarchy)
                {
                    return enemy;
                }
            }
        }
        
        GameObject poolEnemy = Instantiate(spawnTarget.Prefab, transform);
        poolEnemy.name = spawnTarget.MonsterName;
        m_EnemyList.Add(poolEnemy);
        var logic = poolEnemy.GetComponent<EnemyLogicSystem>();
        if (logic != null) logic.SetTarget(player);
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
