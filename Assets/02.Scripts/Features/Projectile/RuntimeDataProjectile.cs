using UnityEngine;

namespace Features.Projectile
{
    public class RuntimeDataProjectile
    {
        public PureDataProjectile PureData { get; private set; }
        
        // 상태 변수
        public bool IsInScreen { get; set; }
        public float TimeSinceOutOfScreen { get; set; }
        public GameObject CurrentTarget { get; private set; }

        public RuntimeDataProjectile(PureDataProjectile data)
        {
            PureData = data;
            Reset();
        }

        public void Reset()
        {
            IsInScreen = true;
            TimeSinceOutOfScreen = 0f;
            CurrentTarget = null;
        }

        public void SetTarget(GameObject target)
        {
            CurrentTarget = target;
        }
    }
}
