using Shapes;
using UnityEngine;

namespace Features.Enemy
{
    public class EnemyVisualizer : MonoBehaviour, IEnemyVisualizer
    {
        public Vector3 Position => transform.position;

        public void Move(Vector3 direction, float speed, float deltaTime)
        {
            transform.position += direction * speed * deltaTime;
        }

        public void Rotate(Vector3 direction, float turnSpeed, float deltaTime)
        {
            if (direction == Vector3.zero) return;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * deltaTime);
        }

        public void PlayDamageVisual()
        {
            // TODO: Shapes 에셋을 활용한 데미지 연출 (예: 색상 변경 등)
            Debug.Log($"[EnemyVisualizer] {gameObject.name} 연출: 데미지 입음");
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
