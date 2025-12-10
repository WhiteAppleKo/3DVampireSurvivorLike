using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 투사체를 관리하는 오브젝트 풀링 클래스입니다.
/// </summary>
public class ProjectileWeapon : Weapon
{
    
    [Tooltip("풀링할 투사체 프리팹")]
    public GameObject projectilePrefab;
    
    [Tooltip("투사체 관련 옵션")]
    public int poolSize = 20;
    public float attackDelay = 0.5f;
    public int projectileCount = 1;
    public float findTargetRange = 5.0f;
    

    // 생성된 투사체들을 저장하는 리스트
    private List<GameObject> m_PooledProjectiles;
    private Collider[] m_FindTargetResults = new Collider[100];
    private GameObject m_CurrentTarget;

    void Start()
    {
        m_PooledProjectiles = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            // 투사체를 생성하고 비활성화 상태로 둔 뒤 리스트에 추가
            GameObject obj = Instantiate(projectilePrefab);
            obj.GetComponent<Projectile>().projectilePoolManager = this;
            obj.SetActive(false);
            m_PooledProjectiles.Add(obj);
        }
    }

    /// <summary>
    /// 풀에서 비활성화된 투사체를 찾아 반환합니다.
    /// </summary>
    /// <returns>사용 가능한 투사체 게임 오브젝트</returns>
    public GameObject GetProjectile()
    {
        // 리스트를 순회하며 비활성화된 투사체를 찾는다
        foreach (var projectile in m_PooledProjectiles)
        {
            if (!projectile.activeInHierarchy)
            {
                return projectile;
            }
        }

        // 만약 사용 가능한 투사체가 없다면, 새로 생성 (풀 크기를 동적으로 늘림)
        GameObject newObj = Instantiate(projectilePrefab);
        m_PooledProjectiles.Add(newObj);
        return newObj;
    }

    public void SetTarget(GameObject target)
    {
        m_CurrentTarget = target;
        for (int i = 0; i < projectileCount; i++)
        {
            GameObject obj = GetProjectile();
            obj.SetActive(true);
            obj.transform.position = transform.position;
            obj.GetComponent<Projectile>().SetTarget(target);
        }
    }

    private GameObject FindTarget()
    {
        if (m_FindTargetResults.Length == 0)
        {
            for (int i = 0; i < findTargetRange; i++)
            {
                var size = Physics.OverlapSphereNonAlloc(transform.position, i, m_FindTargetResults);
                if (size != 0)
                {
                    return m_FindTargetResults[0].gameObject;
                }
            }
        }
        return null;
    }
}
