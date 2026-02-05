using System;
using UnityEngine;

namespace Features.Projectile
{
    public class ProjectileVisualizer : MonoBehaviour, IProjectileVisualizer
    {
        public event Action<Collider> OnTriggerEnterEvent;

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        public Vector3 Forward => transform.forward;

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void LookAt(Transform target)
        {
            if (target != null)
            {
                transform.LookAt(target);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterEvent?.Invoke(other);
        }
    }
}
