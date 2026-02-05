using UnityEngine;

namespace Features.Player
{
    public interface IPlayerVisualizer
    {
        // 이동 및 회전 (물리/위치 처리)
        void Move(Vector3 direction, float speed, float deltaTime);
        void LookAt(Vector3 position);
        
        // Shapes 연출
        void SetMoveVisual(float speedRatio); // 이동 속도에 따른 Shapes 변형 (예: 굵기, 크기 변화)
        void PlayDamageVisual();              // 데미지 시 색상 변화 등
        void PlayLevelUpVisual();             // 레벨업 시 화려한 Shapes 효과
    }
}