using UnityEngine;

namespace Features.Camera
{
    [CreateAssetMenu(fileName = "PureDataCamera", menuName = "PureData/System/Camera")]
    public class PureDataCamera : ScriptableObject
    {
        [SerializeField] private float heightOffset = 10f;
        [SerializeField] private float followSpeed = 5f;

        public float HeightOffset => heightOffset;
        public float FollowSpeed => followSpeed;
    }
}
