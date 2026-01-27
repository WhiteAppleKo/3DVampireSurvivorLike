using Shapes;
using UnityEngine;

namespace _02.Scripts.Cotroller
{
    public class EnemyController : Controller
    {
        public int exp;
        [SerializeField]
        private Controller m_Target;

        public float minimumDistance = 0.01f;
        protected override void Awake()
        {
            base.Awake();
            ApplyTimeSclae();
            autoAttacker.GameStart();
        }

        protected override void OnEnable()
        {
            ApplyTimeSclae();
            base.OnEnable();
        }

        private void Update()
        {
            if (m_Target == null) return;

            // 타겟 방향 벡터 계산 (Y축 높이 차이는 무시)
            Vector3 direction = (m_Target.transform.position - transform.position);
            direction.y = 0;

            // 타겟과 일정 거리 이상일 때만 이동
            if (direction.sqrMagnitude > minimumDistance)
            {
                Vector3 moveDir = direction.normalized;

                if (isDashing == false)
                {
                    // 1. 회전 처리 (부드러운 회전)
                    float turnSpeed = FinalStats != null ? FinalStats.turnSpeed : baseStats.turnSpeed;
                    if (moveDir != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                    }
                }
                
                if (isCharging) return;
                
                // 2. 이동 처리 (증강이 적용된 FinalStats 우선 사용)
                float speed = FinalStats != null ? FinalStats.moveSpeed : baseStats.moveSpeed;
                transform.position += moveDir * speed * Time.deltaTime;
            }
        }

        public void SetTarget(Controller target)
        {
            m_Target = target;
        }
        protected void ApplyTimeSclae()
        {
            FinalStats = new BaseStats(baseStats);
            float value = IMTimer.Instance.ElapsedTime / 2;
            var hpvalue = Mathf.FloorToInt(baseStats.maxHp * value);
            FinalStats.hp.IncreaseMaxValue(hpvalue);
            FinalStats.hp.ResetToMax();
            //FinalStats.hp *= value;
        }

        protected override void Die(int prev, int current)
        {
            base.Die(prev, current);
            ExpManager.Instance.SetExp(exp, transform.position);
            gameObject.SetActive(false);
        }
    }
}
