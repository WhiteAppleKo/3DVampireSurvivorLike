using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Shapes;
using UnityEditor.Searcher;
using UnityEngine;

namespace _02.Scripts.AutoAttack.Charge
{
    public class ChargeDashWeapon : Weapon
    {
        public Triangle triangle;
        // 타겟을 지나쳐서 더 이동할 거리
        public float overShootDistance = 3.0f;
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
        public override void WeaponSettingLogic()
        {
            // 자기 자신을 참조하기 때문에 테스트 해볼 필요 있음
            m_Controller = GetComponentInParent<Controller>();
            m_TargetColliders = new Collider[20];
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
            if (m_CurrentTarget != null)
            {
                SetTarget(m_CurrentTarget);
            }else SetTarget(null);

            if (m_CurrentTarget != null)
            {
                Charge(m_CurrentTarget).Forget();
            }
        }

        private async UniTaskVoid Charge(GameObject currenttTarget)
        {
            // 1. Safe Token : 오브젝트가 파괴되면 작업 취소
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            m_IsDashing = true;
            m_Controller.isCharging = m_IsDashing;
            m_HitEnemies.Clear();

            try
            {
                // 2. 돌진 목표 지점 계산
                // 타겟 방향 벡터 계산
                Vector3 startPos = m_Controller.transform.position;
                Vector3 targetPos = currenttTarget.transform.position;

                // Y축 높이는 0으로 동일 하기 때문에 건드리지 않음
                Vector3 direction = (targetPos - startPos).normalized;
                float distanceToTarget = Vector3.Distance(startPos, targetPos);

                Vector3 endPos = startPos + direction * (distanceToTarget + overShootDistance);

                // 3. 공격 딜레이 AttackDelay만큼 대기하면서 게이지를 채움 (TimeSpan 사용)
                //FinalStats.attackDelay

                // 연출 파트를 여기에 넣으면 될듯
                //await UniTask.Delay(TimeSpan.FromSeconds(FinalStats.attackDelay), cancellationToken: token);
                float chargeDuration = FinalStats.attackDelay;
                float currentChargeTime = 0f;
                
                // Delay대신 while문으로 직접 시간을 셈
                while (currentChargeTime < chargeDuration)
                {
                    //토큰 체크
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    currentChargeTime += Time.deltaTime;
                    float progress = Mathf.Clamp01(currentChargeTime / chargeDuration) / 2;
                    // 연출 호출
                    UpdateChargeVisuals(progress);
                    
                    await UniTask.NextFrame(PlayerLoopTiming.Update, token);
                }
                
                float elapsedTime = 0f;
                while (elapsedTime < dashDuration)
                {
                    //토큰 체크
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / dashDuration;
                    m_Controller.transform.position = Vector3.Lerp(startPos, endPos, t);

                    //대기 후 실행할 함수 호출
                    ChargeAttack(currenttTarget);

                    await UniTask.NextFrame(PlayerLoopTiming.Update, token);
                }

                m_Controller.transform.position = endPos;
                ChargeAttack(currenttTarget);
            }
            catch (OperationCanceledException)
            {
                //대기 중 오브젝트 파괴 취소 처리 (나중에 필요해 지면)
            }
            finally
            {
                m_IsDashing = false;
                m_Controller.isCharging = m_IsDashing;
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

        private void ChargeAttack(GameObject target)
        {
            // 현재 오브젝트 위치 기준 범위 내 적 감지
            int hitCount = Physics.OverlapSphereNonAlloc(target.transform.position, hitRadius, m_TargetColliders, FinalStats.targetLayer);
            Debug.Log($"{gameObject.name} {hitCount}");
            for (int i = 0; i < hitCount; i++)
            {
                GameObject enemyObj = m_TargetColliders[i].gameObject;
                int enemyId = enemyObj.GetInstanceID();
                
                //이번 돌진에서 아직 맞지 않은 적만 타격
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
