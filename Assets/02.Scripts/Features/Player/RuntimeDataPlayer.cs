using System;
using System.Collections.Generic;
using _02.Scripts.Managers.Save;
using Features.Augment;
using Features.Player;
using UnityEngine;

namespace Features.Player
{
    public class RuntimeDataPlayer
    {
        public PureDataPlayer PureData { get; private set; }

        // 가변 상태
        public int CurrentLevel { get; private set; }
        public int CurrentExp { get; private set; }
        public int MaxExp { get; private set; }
        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }
        public float MoveSpeed { get; private set; }
        public LayerMask TargetLayer => PureData.TargetLayer;
        public bool IsDead => CurrentHp <= 0;

        // 증강 누적 데이터
        private int _maxHpAdded = 0;
        private float _moveSpeedMultiplier = 1.0f;
        private List<string> _acquiredAugmentIDs = new List<string>();

        // 외부에서 저장용으로 접근 가능하도록 공개
        public List<string> AcquiredAugmentIDs => _acquiredAugmentIDs;

        // 이벤트
        public event Action<int, int> OnHpChanged; // (current, max)
        public event Action<int, int> OnExpChanged; // (current, max)
        public event Action<int> OnLevelUp;
        public event Action OnDead;

        public RuntimeDataPlayer(PureDataPlayer pureData)
        {
            PureData = pureData;
            Reset();
        }

        public void Reset()
        {
            CurrentLevel = 1;
            CurrentExp = 0;
            MaxExp = PureData.BaseExpToLevelUp;
            
            _maxHpAdded = 0;
            _moveSpeedMultiplier = 1.0f;
            _acquiredAugmentIDs.Clear();
            
            RecalculateStats();
            CurrentHp = MaxHp; // Reset 시 체력 풀 회복

            NotifyAll();
        }

        public void Load(PlayerSaveData saveData)
        {
            if (saveData == null) return;

            CurrentLevel = saveData.playerLevel;
            CurrentExp = saveData.currentExp;
            
            // 레벨에 따른 MaxExp 계산
            MaxExp = PureData.BaseExpToLevelUp + (CurrentLevel - 1) * PureData.ExpIncreasePerLevel;
            
            _maxHpAdded = 0;
            _moveSpeedMultiplier = 1.0f;
            _acquiredAugmentIDs.Clear();

            if (saveData.statAugments != null)
            {
                foreach (string id in saveData.statAugments)
                {
                    var augment = DataHub.Instance.GetStatAbilityData(id);
                    if (augment != null)
                    {
                        // Load 시에는 리스트에만 추가하고 수치는 직접 적용 (Add...Modifier를 쓰면 중복 계산될 수 있음)
                        _acquiredAugmentIDs.Add(id);
                        ApplyStatAugment(augment);
                    }
                }
            }

            RecalculateStats();
            CurrentHp = saveData.currentHp > 0 ? saveData.currentHp : MaxHp;

            NotifyAll();
        }

        private void ApplyStatAugment(PureDataStatAbility augment)
        {
            if (augment.ID.Contains("Hp"))
            {
                _maxHpAdded += (int)augment.ValueAmount;
            }
            else if (augment.ID.Contains("Speed"))
            {
                _moveSpeedMultiplier += augment.ValueAmount;
            }
        }

        public void AddMaxHpModifier(int amount, string augmentID = "")
        {
            _maxHpAdded += amount;
            if (!string.IsNullOrEmpty(augmentID)) _acquiredAugmentIDs.Add(augmentID);
            
            RecalculateStats();
            CurrentHp += amount; 
            OnHpChanged?.Invoke(CurrentHp, MaxHp);
        }

        public void AddMoveSpeedModifier(float multiplierAdd, string augmentID = "")
        {
            _moveSpeedMultiplier += multiplierAdd;
            if (!string.IsNullOrEmpty(augmentID)) _acquiredAugmentIDs.Add(augmentID);
            
            RecalculateStats();
        }

        private void RecalculateStats()
        {
            MaxHp = PureData.BaseMaxHp + _maxHpAdded;
            MoveSpeed = PureData.BaseMoveSpeed * _moveSpeedMultiplier;
        }

        public void AddExp(int amount)
        {
            CurrentExp += amount;
            while (CurrentExp >= MaxExp)
            {
                LevelUp();
            }
            OnExpChanged?.Invoke(CurrentExp, MaxExp);
        }

        private void LevelUp()
        {
            CurrentExp -= MaxExp;
            CurrentLevel++;
            MaxExp += PureData.ExpIncreasePerLevel;
            OnLevelUp?.Invoke(CurrentLevel);
        }

        public void TakeDamage(int damage)
        {
            if (IsDead) return;

            CurrentHp = Math.Max(0, CurrentHp - damage);
            OnHpChanged?.Invoke(CurrentHp, MaxHp);

            if (IsDead)
            {
                OnDead?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            CurrentHp = Math.Min(MaxHp, CurrentHp + amount);
            OnHpChanged?.Invoke(CurrentHp, MaxHp);
        }

        public void UpdateStats(int hpAdd, float speedMult)
        {
            MaxHp = PureData.BaseMaxHp + hpAdd;
            MoveSpeed = PureData.BaseMoveSpeed * speedMult;
            OnHpChanged?.Invoke(CurrentHp, MaxHp);
        }

        private void NotifyAll()
        {
            OnHpChanged?.Invoke(CurrentHp, MaxHp);
            OnExpChanged?.Invoke(CurrentExp, MaxExp);
        }
    }
}
