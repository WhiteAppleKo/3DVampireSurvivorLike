using UnityEngine;

namespace Features.ExpCristal
{
    [CreateAssetMenu(fileName = "PureDataExpCristal", menuName = "PureData/Item/ExpCristal")]
    public class PureDataExpCristal : ScriptableObject
    {
        [Header("Movement Settings")]
        [Tooltip("기본 이동 속도")]
        public float BaseMoveSpeed = 5f;
        [Tooltip("초당 가속도")]
        public float Acceleration = 10f;
        [Tooltip("획득 판정 거리")]
        public float AcquisitionDistance = 0.1f;

        [Header("Value Settings")]
        [Tooltip("기본 경험치 획득량")]
        public int BaseExpAmount = 10;
    }
}
