using System;
using System.Collections.Generic;
using System.Threading;
using _02.Scripts.Cotroller;
using Cysharp.Threading.Tasks;
using Features.Enemy;
using Shapes;
using UnityEditor.Searcher;
using UnityEngine;

namespace _02.Scripts.AutoAttack.Charge
{
    public class ChargeDashWeapon : Weapon
    {
        public Triangle triangle;
        // 타겟을 지나쳐서 더 이동할 거리
        // 전체 돌진에 걸리는 시간
        public float dashDuration; 
        // 돌진 중 공격 반경
        public float hitRadius = 1f;
        
        // 돌진 중 중복 피격 방지를 위한 해시셋
        private HashSet<int> m_HitEnemies = new HashSet<int>();

        private Collider[] m_TargetColliders;
        private GameObject m_CurrentTarget;
        private Controller m_Controller;
        private bool m_IsDashing;
        private BattleManager.DamageEventStruct m_DamageEvent;
        private EnemyLogicSystem m_EnemyController;
        
        // 플레이어 돌진 판정은 플레이어가 누르는 방향으로 돌진해야함
        private bool m_IsPlayer;
        public override void WeaponSettingLogic()
        {
            // 자기 자신을 참조하기 때문에 테스트 해볼 필요 있음
            m_Controller = GetComponentInParent<Controller>();
            m_IsPlayer = m_Controller is PlayerController;
            m_TargetColliders = new Collider[20];

            if (m_IsPlayer == false)
            {
                m_EnemyController = m_Controller as EnemyLogicSystem;
                // Note: EnemyLogicSystem uses pureData for minimumDistance, 
                // but since we need to set it per weapon/instance, we might need a way to override it.
                // For now, keeping the logic similar to original.
            }
        }

        public override void AttackLogic()
        {
            // 이미 공격 준비 중이면 로직을 건너 뜀
            if (m_IsDashing)
            {
                return;
            }
            // 베이스 클래스의 AttackLogic을 호출하여 증강의 OnAttack 효과를 발동시킵니다.
            base.AttackLogic();

            m_CurrentTarget = FindTarget();
            Vector3 dashTargetPos;

            if (m_CurrentTarget != null)
            {
                dashTargetPos = m_CurrentTarget.transform.position;
                SetTarget(m_CurrentTarget);
                Charge(dashTargetPos).Forget();
            }
            else if (m_IsPlayer)
            {
                // 플레이어는 타겟이 없어도 보는 방향으로 돌진
                dashTargetPos = m_Controller.transform.position + m_Controller.transform.forward * FinalStats.chargeWeaponStat.findTargetRange;
                SetTarget(null);
                Charge(dashTargetPos).Forget();
            }
        }

        protected override void RecalculateStats()
        {
            base.RecalculateStats();
            dashDuration = m_Controller.FinalStats.moveSpeed * 0.2f;
        }

        private async UniTaskVoid Charge(Vector3 targetPos)
        {
            // 1. Safe Token : 오브젝트가 파괴되면 작업 취소
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            m_IsDashing = true;
            m_Controller.isMoveDisable = m_IsDashing;
            m_HitEnemies.Clear();

            try
            {
                // 2. 돌진 목표 지점 계산
                Vector3 startPos = m_Controller.transform.position;
                Vector3 direction = (targetPos - startPos).normalized;
                if (direction == Vector3.zero) direction = m_Controller.transform.forward;

                float distanceToTarget = Vector3.Distance(startPos, targetPos);
                Vector3 endPos = startPos + direction * (distanceToTarget + FinalStats.chargeWeaponStat.findTargetRange);

                float chargeDuration = FinalStats.attackDelay;
                float currentChargeTime = 0f;
                
                while (currentChargeTime < chargeDuration)
                {
                    if (token.IsCancellationRequested) return;
                    currentChargeTime += Time.deltaTime;
                    float progress = Mathf.Clamp01(currentChargeTime / chargeDuration) / 2;
                    UpdateChargeVisuals(progress);
                    await UniTask.NextFrame(PlayerLoopTiming.Update, token);
                }
                
                float elapsedTime = 0f;
                m_Controller.isDashing = m_IsDashing;
                while (elapsedTime < dashDuration)
                {
                    if (token.IsCancellationRequested) return;

                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / dashDuration;
                    m_Controller.transform.position = Vector3.Lerp(startPos, endPos, t);

                    ChargeAttack();
                    await UniTask.NextFrame(PlayerLoopTiming.Update, token);
                }

                m_Controller.isDashing = false;
                m_Controller.transform.position = endPos;
                ChargeAttack();
            }
            catch (OperationCanceledException) { }
            finally
            {
                m_IsDashing = false;
                m_Controller.isMoveDisable = m_IsDashing;
            }
        }

        private Vector3 m_TriangleProgress;
        private void UpdateChargeVisuals(float progress)
        {
            if (triangle != null)
            {
                float updateValue = 0.5f - progress;
                m_TriangleProgress = new Vector3(updateValue, updateValue, 0);
                triangle.A = m_TriangleProgress;
            }
        }

        private void ChargeAttack()
        {
            // dasher의 현재 위치 기준 범위 내 적 감지 (버그 수정: target 대신 m_Controller 위치 사용)
            int hitCount = Physics.OverlapSphereNonAlloc(m_Controller.transform.position, hitRadius, m_TargetColliders, FinalStats.targetLayer);
            
            for (int i = 0; i < hitCount; i++)
            {
                GameObject enemyObj = m_TargetColliders[i].gameObject;
                if (enemyObj == m_Controller.gameObject) continue;

                int enemyId = enemyObj.GetInstanceID();
                if (!m_HitEnemies.Contains(enemyId))
                {
                    m_HitEnemies.Add(enemyId);
                    ApplyDamage(enemyObj);
                }
            }
        }
        
        private void ApplyDamage(GameObject enemy)
        {
            Controller controller = enemy.GetComponent<Controller>();
            m_DamageEvent = new BattleManager.DamageEventStruct(FinalStats.damage, this, m_Controller, controller);
            BattleManager.Instance.BroadcastDamageEvent(m_DamageEvent);
        }

        private GameObject FindTarget()
        {
            // 증강으로 변경된 최종 타겟 탐지 범위(finalStats.findTargetRange)를 사용합니다.
            int size = Physics.OverlapSphereNonAlloc(transform.position, FinalStats.chargeWeaponStat.findTargetRange, m_TargetColliders, FinalStats.targetLayer);

            // 감지된 타겟이 없으면 null을 반환합니다.
            if (size == 0)
            {
                return null;
            }

            GameObject closestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            // 감지된 모든 콜라이더를 순회하며 가장 가까운 타겟을 찾습니다.
            for (int i = 0; i < Mathf.Min(size, m_TargetColliders.Length); i++)
            {
                Vector3 directionToTarget = m_TargetColliders[i].transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude; // 제곱 거리를 사용하여 성능 최적화

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestTarget = m_TargetColliders[i].gameObject;
                }
            }
        
            return closestTarget;
        }

        private void SetTarget(GameObject target)
        {
            m_CurrentTarget = target;
        }
    }
}
