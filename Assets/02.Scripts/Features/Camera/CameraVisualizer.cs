using Features.Camera;
using UnityEngine;

namespace _02.Scripts.Features.Camera
{
    public class CameraVisualizer : MonoBehaviour, ICameraVisualizer
    {
        public Vector3 CurrentPosition => transform.position;

        public void MoveTo(Vector3 targetPosition, float speed, float deltaTime)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * deltaTime);
        }
    }
}
