using UnityEngine;

namespace Features.Weapon
{
    public interface IWeaponVisualizer
    {
        // 공격 애니메이션 재생
        void PlayAttackAnimation();
        
        // 공격 사운드 재생
        void PlayAttackSound(AudioClip clip);
        
        // 이펙트 생성 (필요 시)
        void SpawnAttackEffect(Vector3 position, Quaternion rotation);
    }
}
