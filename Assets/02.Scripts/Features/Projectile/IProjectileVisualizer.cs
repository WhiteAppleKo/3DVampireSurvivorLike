using System;
using UnityEngine;

namespace Features.Projectile
{
    public interface IProjectileVisualizer
    {
        // 위치/회전 제어
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 Forward { get; }
        
        // 활성/비활성 제어
        void SetActive(bool isActive);
        void LookAt(Transform target);
        
        // 이벤트 (물리 충돌 -> 로직 전달)
        event Action<Collider> OnTriggerEnterEvent;
    }
}
