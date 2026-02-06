using Features.Camera;
using UnityEngine;

namespace _02.Scripts.Features.Camera
{
    [RequireComponent(typeof(ICameraVisualizer))]
    public class CameraLogicSystem : MonoBehaviour
    {
        [Header("DLV Data")]
        [SerializeField] private PureDataCamera pureData;
        [SerializeField] private Transform player;

        private ICameraVisualizer visuals;
        private Vector3 m_TargetPosition;

        private void Awake()
        {
            // [Internal Binding]
            visuals = GetComponent<ICameraVisualizer>();
            
            if (pureData == null)
            {
                Debug.LogWarning("[CameraLogicSystem] PureData가 설정되지 않았습니다.");
            }
        }

        private void LateUpdate()
        {
            if (player == null || pureData == null) return;

            // 로직: 타겟 위치 계산
            m_TargetPosition.x = player.position.x;
            m_TargetPosition.y = player.position.y + pureData.HeightOffset;
            m_TargetPosition.z = player.position.z;

            // 명령: 비주얼에게 이동 지시
            visuals.MoveTo(m_TargetPosition, pureData.FollowSpeed, Time.deltaTime);
        }

        public void SetTarget(Transform target)
        {
            player = target;
        }
    }
}
