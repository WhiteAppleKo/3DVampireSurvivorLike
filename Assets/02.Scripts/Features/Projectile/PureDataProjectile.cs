using UnityEngine;

namespace Features.Projectile
{
    [CreateAssetMenu(fileName = "PureDataProjectile", menuName = "PureData/Combat/PureDataProjectile")]
    public class PureDataProjectile : ScriptableObject
    {
        [Tooltip("기본 이동 속도")]
        public float Speed = 10f;
        
        [Tooltip("화면 밖으로 나간 후 풀로 돌아가기까지의 대기 시간")]
        public float ReturnToPoolDelay = 2f;

        [Tooltip("관통 가능 횟수 (0이면 무제한 혹은 1회)")]
        public int PiercingCount = 1;
    }
}
