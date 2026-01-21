using System.Collections;
using System.Collections.Generic;
using _02.Scripts.AutoAttack.Projectile;
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
    
    [Tooltip("미리 생성해 둘 투사체 개수")]
    public int poolSize = 20;
    
    // 생성된 투사체들을 저장하는 리스트
    private List<GameObject> m_PooledProjectiles;
    private Collider[] m_FindTargetResults = new Collider[50];
    private GameObject m_CurrentTarget;
    private Controller m_Controller;
    
    public override void WeaponSettingLogic()
    {
        m_Controller = GetComponentInParent<Controller>();
        m_PooledProjectiles = new List<GameObject>();
        baseStats.projectileWeaponStats = new ProjectileWeaponStats();
        for (int i = 0; i < poolSize; i++)
        {
            // 투사체를 생성하고 비활성화 상태로 둔 뒤 리스트에 추가
            GameObject obj = Instantiate(projectilePrefab);
            // 데미지 값을 미리 설정하지 않고, Controller와 Weapon 참조만 전달합니다.
            obj.GetComponent<Projectile>().ProjectileSetting(m_Controller, this, targetLayer);
            obj.SetActive(false);
            m_PooledProjectiles.Add(obj);
        }
    }
    
    public override void AttackLogic()
    {
        // 베이스 클래스의 AttackLogic을 호출하여 증강의 OnAttack 효과를 발동시킵니다.
        base.AttackLogic();
        
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
        newObj.GetComponent<Projectile>().ProjectileSetting(m_Controller, this, targetLayer);
        m_PooledProjectiles.Add(newObj);
        return newObj;
    }

    public void SetTarget(GameObject target)
    {
        m_CurrentTarget = target;
        // 증강으로 변경된 최종 투사체 수(finalStats.projectileCount)를 사용합니다.
        for (int i = 0; i < FinalStats.projectileWeaponStats.projectileCount; i++)
        {
            GameObject obj = GetProjectile();
            obj.SetActive(true);
            obj.transform.position = transform.position;
            obj.GetComponent<Projectile>().SetTarget(target);
        }
    }

    private GameObject FindTarget()
    {
        // 증강으로 변경된 최종 타겟 탐지 범위(finalStats.findTargetRange)를 사용합니다.
        int size = Physics.OverlapSphereNonAlloc(transform.position, FinalStats.projectileWeaponStats.findTargetRange, m_FindTargetResults, targetLayer);

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

