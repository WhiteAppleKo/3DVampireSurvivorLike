using System.Collections.Generic;
using Features.Enemy;
using _02.Scripts.Cotroller;
using UnityEngine;

namespace _02.Scripts.Managers.Spawn
{
    // [V] Visual Layer: Object Pooling & Prefab Instantiation
    public class MonsterSpawner : MonoBehaviour
    {
        [SerializeField] private Controller player;
        
        // 프리팹별 풀링 딕셔너리
        private Dictionary<string, List<GameObject>> m_Pool = new Dictionary<string, List<GameObject>>();

        public GameObject Spawn(PureDataEnemy data, Vector3 position)
        {
            GameObject enemy = GetFromPool(data);
            enemy.transform.position = position;
            enemy.SetActive(true);
            return enemy;
        }

        private GameObject GetFromPool(PureDataEnemy data)
        {
            if (!m_Pool.ContainsKey(data.ID))
            {
                m_Pool[data.ID] = new List<GameObject>();
            }

            foreach (var obj in m_Pool[data.ID])
            {
                if (!obj.activeInHierarchy) return obj;
            }

            // 풀에 없으면 새로 생성
            GameObject newObj = Instantiate(data.Prefab, transform);
            newObj.name = $"{data.MonsterName}_{data.ID}";
            
            // 초기 로직 바인딩 (DLV 로직 시스템 연결)
            var logic = newObj.GetComponent<EnemyLogicSystem>();
            if (logic != null) logic.SetTarget(player);

            m_Pool[data.ID].Add(newObj);
            return newObj;
        }

        public void ClearPool()
        {
            foreach (var list in m_Pool.Values)
            {
                foreach (var obj in list)
                {
                    if (obj != null) Destroy(obj);
                }
            }
            m_Pool.Clear();
        }
    }
}
