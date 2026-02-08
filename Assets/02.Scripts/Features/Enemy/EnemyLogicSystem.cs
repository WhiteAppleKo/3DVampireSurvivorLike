using _02.Scripts.Cotroller;
using Features.Enemy;
using Shapes;
using UnityEngine;

namespace Features.Enemy
{
    [RequireComponent(typeof(IEnemyVisualizer))]
    public class EnemyLogicSystem : Controller
    {
        [Header("DLV Data")]
        [SerializeField] private PureDataEnemy pureData;
        [SerializeField] private Controller m_Target;

        private RuntimeDataEnemy model;
        private IEnemyVisualizer visuals;

        protected override void Awake()
        {
            // [Internal Binding] Visualizer 연결
            visuals = GetComponent<IEnemyVisualizer>();
            
            if (pureData == null)
            {
                pureData = ScriptableObject.CreateInstance<PureDataEnemy>();
                Debug.LogWarning("[EnemyLogicSystem] PureData가 설정되지 않았습니다.");
            }

            // 모델 초기화
            model = new RuntimeDataEnemy(pureData);
            
            // 기존 Controller 호환을 위한 base 호출
            base.Awake();
            
            ApplyTimeSclae();
            autoAttacker.GameStart();
        }

        protected override void OnEnable()
        {
            ApplyTimeSclae();
            base.OnEnable();
            
            if (model != null)
            {
                model.OnDeath += OnModelDeath;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (model != null)
            {
                model.OnDeath -= OnModelDeath;
            }
        }

        private void Update()
        {
            if (m_Target == null || isMoveDisable) return;

            // 1. 로직: 타겟 방향 및 거리 계산
            Vector3 direction = (m_Target.transform.position - visuals.Position);
            direction.y = 0;

            if (direction.sqrMagnitude > pureData.MinimumDistance)
            {
                Vector3 moveDir = direction.normalized;

                if (isDashing == false)
                {
                    // 2. 명령: 비주얼에게 회전 지시
                    visuals.Rotate(moveDir, model.TurnSpeed, Time.deltaTime);
                }
                
                // 3. 명령: 비주얼에게 이동 지시
                visuals.Move(moveDir, model.MoveSpeed, Time.deltaTime);
            }
        }

        public void SetTarget(Controller target)
        {
            m_Target = target;
        }

        protected void ApplyTimeSclae()
        {
            if (model != null && IMTimer.Instance != null)
            {
                model.ApplyTimeScale(IMTimer.Instance.ElapsedTime);
            }
        }

        protected override void ApplyDamage(int amount)
        {
            // base.ApplyDamage(amount); // 기존 baseStats 대신 model 사용
            model.TakeDamage(amount);
            visuals.PlayDamageVisual();
        }

        private void OnModelDeath()
        {
            Die(model.CurrentHp + 1, model.CurrentHp);
        }

        protected override void Die(int prev, int current)
        {
            // 경험치 지급 (기존 로직 유지)
            if (ExpManager.Instance != null)
            {
                ExpManager.Instance.SetExp(model.ExpAmount, transform.position);
            }
            
            visuals.SetActive(false);
        }

        public override float CurrentMoveSpeed => model != null ? model.MoveSpeed : 0f;
    }
}
