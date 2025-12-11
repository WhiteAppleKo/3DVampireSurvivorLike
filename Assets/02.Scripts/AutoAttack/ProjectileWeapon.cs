using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 투사체를 관리하는 오브젝트 풀링 클래스입니다.
/// </summary>
public class ProjectileWeapon : Weapon
{
    [Tooltip("타겟으로 지정할 레이어")]
    public LayerMask targetLayer;
    
    [Tooltip("풀링할 투사체 프리팹")]
    public GameObject projectilePrefab;
    
    [Tooltip("투사체 관련 옵션")]
    public int poolSize = 20;
    public int projectileCount = 1;
    public float findTargetRange = 5.0f;
    

    // 생성된 투사체들을 저장하는 리스트
    private List<GameObject> m_PooledProjectiles;
    private Collider[] m_FindTargetResults = new Collider[50];
    private GameObject m_CurrentTarget;
    
    public override void WeaponSettingLogic()
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
    
    public override void AttackLogic()
    {
        Debug.Log($"이 컴포넌트의 클래스 이름은 '{this.GetType().Name}' 입니다.");
        m_CurrentTarget = FindTarget();
        if (m_CurrentTarget != null)
        {
            SetTarget(m_CurrentTarget);
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
        newObj.GetComponent<Projectile>().projectilePoolManager = this; // 누락되었던 부분 추가
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
        // 지정된 범위와 레이어 마스크를 사용하여 주변의 모든 콜라이더를 감지합니다.
        int size = Physics.OverlapSphereNonAlloc(transform.position, findTargetRange, m_FindTargetResults, targetLayer);

        // 감지된 타겟이 없으면 null을 반환합니다.
        if (size == 0)
        {
            return null;
        }

        GameObject closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        // 감지된 모든 콜라이더를 순회하며 가장 가까운 타겟을 찾습니다.
        for (int i = 0; i < Mathf.Min(size, m_FindTargetResults.Length); i++)
        {
            Vector3 directionToTarget = m_FindTargetResults[i].transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude; // 제곱 거리를 사용하여 성능 최적화

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestTarget = m_FindTargetResults[i].gameObject;
            }
        }
        
        return closestTarget;
    }
}
