using System;
using System.Collections.Generic;
using UnityEngine;
using _02.Scripts.AutoAttack;
using Features.Weapon;

namespace _02.Scripts.Managers
{
    /// <summary>
    /// 게임 내 모든 사운드(BGM, SFX)를 중앙 관리하는 매니저입니다.
    /// Audio Pooling 방식을 사용하여 3D 입체 음향을 효율적으로 처리합니다.
    /// </summary>
    public class AudioManager : SingletoneBase<AudioManager>
    {
        [Header("Pool Settings")]
        [SerializeField] private int poolSize = 20;
        private List<AudioSource> _sfxPool = new List<AudioSource>();

        [Header("Audio Sources (2D)")]
        private AudioSource _uiSource;
        private AudioSource _bgmSource;

        [Header("Volume Settings")]
        [Range(0f, 1f)] public float masterVolume = 1.0f;
        [Range(0f, 1f)] public float sfxVolume = 0.8f;
        [Range(0f, 1f)] public float bgmVolume = 0.5f;

        protected override void Awake()
        {
            base.Awake();
            InitializePool();
            Setup2DSources();
        }

        private void InitializePool()
        {
            // SFX 풀 생성 (3D 소리용)
            for (int i = 0; i < poolSize; i++)
            {
                GameObject go = new GameObject($"SFX_Pool_{i}");
                go.transform.SetParent(transform);
                AudioSource source = go.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 1.0f; // 3D 설정
                source.rolloffMode = AudioRolloffMode.Logarithmic;
                source.minDistance = 2f;
                source.maxDistance = 50f;
                _sfxPool.Add(source);
            }
        }

        private void Setup2DSources()
        {
            // UI용 (2D)
            GameObject uiGo = new GameObject("UI_AudioSource");
            uiGo.transform.SetParent(transform);
            _uiSource = uiGo.AddComponent<AudioSource>();
            _uiSource.spatialBlend = 0f;

            // BGM용 (2D)
            GameObject bgmGo = new GameObject("BGM_AudioSource");
            bgmGo.transform.SetParent(transform);
            _bgmSource = bgmGo.AddComponent<AudioSource>();
            _bgmSource.spatialBlend = 0f;
            _bgmSource.loop = true;
        }

        private void OnEnable()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.onDamageEvent += OnDamageProcessed;
            }
        }

        private void OnDisable()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.onDamageEvent -= OnDamageProcessed;
            }
        }

        /// <summary>
        /// 대미지 이벤트 발생 시 피격 위치에서 무기 사운드 재생
        /// </summary>
        private void OnDamageProcessed(BattleManager.DamageEventStruct damageEvent)
        {
            if (damageEvent.senderWeapon != null && damageEvent.senderWeapon.PureData != null)
            {
                AudioClip clip = damageEvent.senderWeapon.PureData.AttackSound;
                if (clip != null)
                {
                    // 피격자 위치에서 3D 사운드 재생
                    Vector3 pos = damageEvent.receiver != null ? damageEvent.receiver.transform.position : damageEvent.senderWeapon.transform.position;
                    PlaySFX(clip, pos);
                }
            }
        }

        /// <summary>
        /// 3D 효과음을 특정 위치에서 재생합니다.
        /// </summary>
        public void PlaySFX(AudioClip clip, Vector3 position, float volumeScale = 1.0f)
        {
            if (clip == null) return;

            AudioSource source = GetAvailableSource();
            if (source != null)
            {
                source.transform.position = position;
                source.clip = clip;
                source.volume = volumeScale * sfxVolume * masterVolume;
                source.Play();
            }
        }

        /// <summary>
        /// 2D 효과음(UI 등)을 재생합니다.
        /// </summary>
        public void Play2D(AudioClip clip, float volumeScale = 1.0f)
        {
            if (clip == null) return;
            _uiSource.PlayOneShot(clip, volumeScale * sfxVolume * masterVolume);
        }

        /// <summary>
        /// 배경음(BGM)을 재생합니다.
        /// </summary>
        public void PlayBGM(AudioClip clip)
        {
            if (clip == null || _bgmSource.clip == clip) return;
            _bgmSource.clip = clip;
            _bgmSource.volume = bgmVolume * masterVolume;
            _bgmSource.Play();
        }

        private AudioSource GetAvailableSource()
        {
            foreach (var s in _sfxPool)
            {
                if (!s.isPlaying) return s;
            }
            // 모든 소스가 사용 중이면 가장 첫 번째 소스 강제 재사용 (또는 확장 가능)
            return _sfxPool[0]; 
        }

        public void SetMasterVolume(float vol)
        {
            masterVolume = Mathf.Clamp01(vol);
            _bgmSource.volume = bgmVolume * masterVolume;
        }
    }
}
