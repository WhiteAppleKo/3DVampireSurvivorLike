using UnityEngine;

namespace Features.Enemy
{
    public interface IEnemyVisualizer
    {
        Vector3 Position { get; }
        void Move(Vector3 direction, float speed, float deltaTime);
        void Rotate(Vector3 direction, float turnSpeed, float deltaTime);
        void PlayDamageVisual();
        void SetActive(bool active);
    }
}
