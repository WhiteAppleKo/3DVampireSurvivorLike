using System.Collections.Generic;
using _02.Scripts.AutoAttack;
using _02.Scripts.Cotroller;
using UnityEngine;

/// <summary>
/// 투사체를 관리하는 오브젝트 풀링 클래스입니다. (DLV Refactored)
/// </summary>
public class ProjectileWeapon : Weapon
{
    [Tooltip("풀링할 투사체 프리팹")]
    public GameObject projectilePrefab;
    
    [Tooltip("미리 생성해 둘 투사체 개수")]
    public int poolSize = 20;
    
    // 생성된 투사체들을 저장하는 리스트
    private List<GameObject> m_PooledProjectiles;
    
    // NonAlloc을 위한 캐시
    private Collider[] m_FindTargetResults = new Collider[50];
    private Controller m_Controller;
    private GameObject m_CurrentTarget;

    public override void WeaponSettingLogic()
    {
        m_Controller = GetComponentInParent<Controller>();
        m_PooledProjectiles = new List<GameObject>();
        
        // 초기 풀 생성
        for (int i = 0; i < poolSize; i++)
        {
            CreateProjectile();
        }
    }

    private GameObject CreateProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"[ProjectileWeapon] 프리팹이 설정되지 않았습니다: {name}");
            return null;
        }

        GameObject obj = Instantiate(projectilePrefab);
        // Projectile 컴포넌트 설정 (DLV Logic System)
        var projectileLogic = obj.GetComponent<Projectile>();
        if (projectileLogic != null)
        {
            projectileLogic.ProjectileSetting(m_Controller, this, FinalStats.targetLayer);
        }
        else
        {
            Debug.LogError($"[ProjectileWeapon] 프리팹에 Projectile 컴포넌트가 없습니다.");
        }

        obj.SetActive(false);
        m_PooledProjectiles.Add(obj);
        return obj;
    }

    public override void AttackLogic()
    {
        base.AttackLogic(); // Visual & Sound & CD

        m_CurrentTarget = FindTarget();
        if (m_CurrentTarget != null)
        {
            SpawnProjectiles(m_CurrentTarget);
        }
    }

    /// <summary>
    /// 풀에서 비활성화된 투사체를 찾아 반환하거나 새로 생성합니다.
    /// </summary>
    public GameObject GetProjectile()
    {
        foreach (var projectile in m_PooledProjectiles)
        {
            if (!projectile.activeInHierarchy)
            {
                return projectile;
            }
        }
        return CreateProjectile();
    }

    private void SpawnProjectiles(GameObject target)
    {
        // 증강 등으로 변경된 최종 투사체 수 사용 (RuntimeData 활용 권장하지만 호환성 위해 FinalStats 사용)
        int count = FinalStats.projectileWeaponStats.projectileCount;
        
        for (int i = 0; i < count; i++)
        {
            GameObject obj = GetProjectile();
            if (obj == null) continue;

            obj.transform.position = transform.position;
            obj.SetActive(true);
            
            var logic = obj.GetComponent<Projectile>();
            if (logic != null)
            {
                logic.SetTarget(target);
            }
        }
    }

    private GameObject FindTarget()
    {
        // 탐지 범위
        float range = FinalStats.projectileWeaponStats.findTargetRange;
        int size = Physics.OverlapSphereNonAlloc(transform.position, range, m_FindTargetResults, FinalStats.targetLayer);

        if (size == 0) return null;

        GameObject closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        for (int i = 0; i < size; i++)
        {
            if (m_FindTargetResults[i] == null) continue;

            Vector3 directionToTarget = m_FindTargetResults[i].transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestTarget = m_FindTargetResults[i].gameObject;
            }
        }
        
        return closestTarget;
    }
}

