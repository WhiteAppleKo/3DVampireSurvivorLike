using Shapes;
using UnityEngine;

namespace Features.Player
{
    // [V] World Visualizer using Shapes Asset
    public class PlayerVisualizer : MonoBehaviour, IPlayerVisualizer
    {
        [Header("Shapes Components")]
        [SerializeField] private Disc bodyDisc; // 예시: 플레이어 본체를 나타내는 Disc
        
        [Header("Visual Settings")]
        public float baseRadius = 0.5f;
        public float moveSquashAmount = 0.2f;

        public void Move(Vector3 direction, float speed, float deltaTime)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                // 실질적인 위치 이동 수행
                transform.Translate(direction.normalized * (speed * deltaTime), Space.World);
                
                // 회전 (Shapes도 앞뒤 구분이 있다면 회전 필요)
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * deltaTime);
            }
        }

        public void LookAt(Vector3 position)
        {
            Vector3 direction = (position - transform.position).normalized;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        public void SetMoveVisual(float speedRatio)
        {
            // Shapes 연출: 이동 시 본체 Disc의 크기나 굵기를 살짝 변화시켜 역동성 부여
            if (bodyDisc != null)
            {
                // 이동 속도에 따라 Radius를 미세하게 조절 (Squash & Stretch 효과 예시)
                bodyDisc.Radius = baseRadius + (speedRatio * moveSquashAmount);
            }
        }

        public void PlayDamageVisual()
        {
            // 데미지 시 Shapes 색상을 빨간색으로 깜빡이는 등의 연출
        }

        public void PlayLevelUpVisual()
        {
            // 레벨업 시 Shapes의 굵기를 일시적으로 키우거나 반짝이는 효과
        }
    }
}