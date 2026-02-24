using _02.Scripts.Managers;
using UnityEngine;

namespace Features.Weapon
{
    public class WeaponVisualizer : MonoBehaviour, IWeaponVisualizer
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        public void PlayAttackAnimation()
        {
            if (_animator != null)
            {
                _animator.SetTrigger("Attack"); // 애니메이터 파라미터는 프로젝트 설정에 맞춰야 함
            }
        }

        public void PlayAttackSound(AudioClip clip)
        {
            if (clip != null && AudioManager.Instance != null)
            {
                // AudioManager의 풀링 시스템을 사용하여 사운드 재생
                AudioManager.Instance.PlaySFX(clip, transform.position);
            }
        }

        public void SpawnAttackEffect(Vector3 position, Quaternion rotation)
        {
            // 이펙트 풀링 시스템이 있다면 여기서 호출
        }
    }
}
