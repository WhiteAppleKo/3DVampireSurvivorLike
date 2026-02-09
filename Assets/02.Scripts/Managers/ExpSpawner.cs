using System.Collections.Generic;
using _02.Scripts.Cotroller;
using UnityEngine;

namespace _02.Scripts.Managers.Spawn
{
    // [V] Visual Layer: Object Pooling for ExpCristals
    public class ExpSpawner : MonoBehaviour
    {
        [SerializeField] private Controller playerController;
        [SerializeField] private GameObject defaultExpPrefab;
        
        // 프리팹 또는 ID별 풀링 딕셔너리
        private Dictionary<int, List<GameObject>> m_Pool = new Dictionary<int, List<GameObject>>();

        public GameObject Spawn(int amount, Vector3 position)
        {
            GameObject expObj = GetFromPool();
            expObj.transform.position = position;
            expObj.SetActive(true);

            var expLogic = expObj.GetComponent<ExpCristal>();
            if (expLogic != null)
            {
                expLogic.SetValue(amount);
                expLogic.SetTarget(playerController);
            }

            return expObj;
        }

        private GameObject GetFromPool()
        {
            // 현재는 단일 프리팹이므로 간단하게 구현 (향후 확장 가능)
            if (!m_Pool.ContainsKey(0)) m_Pool[0] = new List<GameObject>();

            foreach (var obj in m_Pool[0])
            {
                if (!obj.activeInHierarchy) return obj;
            }

            GameObject newObj = Instantiate(defaultExpPrefab, transform);
            m_Pool[0].Add(newObj);
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
