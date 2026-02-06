using UnityEngine;

namespace Features.Camera
{
    public interface ICameraVisualizer
    {
        Vector3 CurrentPosition { get; }
        void MoveTo(Vector3 targetPosition, float speed, float deltaTime);
    }
}
