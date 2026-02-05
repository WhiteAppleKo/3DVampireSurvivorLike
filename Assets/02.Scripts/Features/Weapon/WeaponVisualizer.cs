using UnityEngine;

namespace Features.Weapon
{
    [RequireComponent(typeof(AudioSource))]
    public class WeaponVisualizer : MonoBehaviour, IWeaponVisualizer
    {
        private AudioSource _audioSource;
        private Animator _animator;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
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
            if (clip != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(clip);
            }
        }

        public void SpawnAttackEffect(Vector3 position, Quaternion rotation)
        {
            // 이펙트 풀링 시스템이 있다면 여기서 호출
        }
    }
}
