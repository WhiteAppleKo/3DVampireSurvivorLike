using System;
using System.Collections.Generic;
using UnityEngine;
using _02.Scripts.AutoAttack;
using Features.Weapon;

namespace _02.Scripts.Managers
{
    /// <summary>
    /// 전투 중 발생하는 통계 데이터(킬수, 피해량)를 수집하는 매니저입니다.
    /// </summary>
    public class BattleStatisticsManager : SingletoneBase<BattleStatisticsManager>
    {
        public class WeaponStat
        {
            public PureDataWeapon WeaponData;
            public int KillCount;
            public long TotalDamage;
        }

        private Dictionary<PureDataWeapon, WeaponStat> _weaponStats = new Dictionary<PureDataWeapon, WeaponStat>();
        private int _totalPoints = 0;

        public Dictionary<PureDataWeapon, WeaponStat> WeaponStats => _weaponStats;
        public int TotalPoints => _totalPoints;

        protected override void Awake()
        {
            base.Awake();
            // 씬 전환 시 파괴되지 않도록 설정 (필요 시)
            // DontDestroyOnLoad(gameObject);
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

        public void ResetStats()
        {
            _weaponStats.Clear();
            _totalPoints = 0;
        }

        private void OnDamageProcessed(BattleManager.DamageEventStruct damageEvent)
        {
            // 플레이어가 가한 대미지만 기록
            if (damageEvent.sender == null || damageEvent.sender != BattleManager.Instance.player) return;

            // 무기가 가한 대미지만 기록 (플레이어 본체 몸박 등도 포함 가능)
            if (damageEvent.senderWeapon == null) return;

            PureDataWeapon weaponData = damageEvent.senderWeapon.PureData;
            if (weaponData == null) return;

            if (!_weaponStats.ContainsKey(weaponData))
            {
                _weaponStats[weaponData] = new WeaponStat { WeaponData = weaponData };
            }

            WeaponStat stat = _weaponStats[weaponData];
            stat.TotalDamage += damageEvent.damageAmount;

            // 사망 판정: 피격자가 대미지 적용 후 비활성화되었는지 확인
            // EnemyLogicSystem.Die() 에서 visuals.SetActive(false)를 호출함
            if (damageEvent.receiver != null && !damageEvent.receiver.gameObject.activeInHierarchy)
            {
                stat.KillCount++;
            }
        }
    }
}
