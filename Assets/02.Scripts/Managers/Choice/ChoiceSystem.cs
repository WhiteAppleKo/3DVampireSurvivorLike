using System.Collections.Generic;
using _02.Scripts.AutoAttack;
using _02.Scripts.UI;
using Features.Augment;
using Features.Player;
using Features.Weapon;
using UnityEngine;

namespace _02.Scripts.Managers.Choice
{
    public class ChoiceSystem : SingletoneBase<ChoiceSystem>
    {
        [Header("DLV Databases")]
        public PureDataBaseStatAbility statAugmentDB;
        public PureDataBaseWeaponAbility weaponAugmentDB;
        public PureDataBaseWeapon weaponDB;

        [Header("References")]
        public PlayerController player;
        private RuntimeDataPlayer m_Model;

        // 선택지 생성 완료 이벤트 (UIView가 이를 구독)
        public event System.Action<List<PureDataAugment>> OnAugmentsGenerated;

        private enum e_ChoiceType { Augment, Weapon }
        private e_ChoiceType m_ChoiceType;
        private HashSet<string> m_CurrentChoiceIDs = new HashSet<string>();

        public void Bind(RuntimeDataPlayer model)
        {
            if (m_Model != null)
            {
                m_Model.OnLevelUp -= OnPlayerLevelUp;
            }

            m_Model = model;
            m_Model.OnLevelUp += OnPlayerLevelUp;
            
            Debug.Log("[ChoiceSystem] Player Model 바인딩 완료");
        }

        private void OnDisable()
        {
            if (m_Model != null)
            {
                m_Model.OnLevelUp -= OnPlayerLevelUp;
            }
        }

        private void OnPlayerLevelUp(int currentLevel)
        {
            SetAugmentChoiceMode();
        }

        public void SetAugmentChoiceMode()
        {
            m_ChoiceType = e_ChoiceType.Augment;
            GenerateAugmentChoices();
        }

        public void SetWeaponChoiceMode()
        {
            m_ChoiceType = e_ChoiceType.Weapon;
            GenerateAugmentChoices();
        }

        public void GenerateAugmentChoices()
        {
            Debug.Log($"[ChoiceSystem] GenerateAugmentChoices 시작. 모드: {m_ChoiceType}");
            
            // 데이터베이스 체크
            if (statAugmentDB == null && weaponAugmentDB == null && weaponDB == null)
            {
                Debug.LogError("[ChoiceSystem] 모든 데이터베이스가 Null입니다! 선택지를 생성할 수 없습니다.");
                return;
            }

            m_CurrentChoiceIDs.Clear();
            List<PureDataAugment> results = new List<PureDataAugment>();

            // [수정] 3개를 모두 채울 때까지 반복 (최대 100회 제한)
            while (results.Count < 3)
            {
                PureDataAugment choice = GetRandomAugment();
                if (choice != null && !m_CurrentChoiceIDs.Contains(choice.ID))
                {
                    results.Add(choice);
                    m_CurrentChoiceIDs.Add(choice.ID);
                    Debug.Log($"[ChoiceSystem] {results.Count}번째 선택지 추가됨: {choice.ID} ({choice.GetType().Name})");
                }
            }

            if (results.Count > 0)
            {
                Debug.Log($"[ChoiceSystem] {results.Count}개의 선택지 생성 완료. 이벤트를 발송합니다.");
                OnAugmentsGenerated?.Invoke(results);
            }
            else
            {
                Debug.LogError("[ChoiceSystem] 생성된 선택지가 0개입니다! 이벤트를 발송하지 않습니다.");
            }
        }

        private PureDataAugment GetRandomAugment()
        {
            // 1. 무기 선택 모드일 때
            if (m_ChoiceType == e_ChoiceType.Weapon)
            {
                return GetRandomWeapon();
            }

            // 2. 증강 모드일 때: 매 슬롯마다 타입 결정 (확률 가중치 부여 가능)
            int rnd = UnityEngine.Random.Range(0, 3);
            switch (rnd)
            {
                case 2: // 약 33% 확률로 무기 출현
                    return GetRandomWeapon();
                default: // 나머지는 순수 증강(Stat/Weapon Ability)
                    return GetRandomPureAugment();
            }
        }

        private PureDataAugment GetRandomPureAugment()
        {
            // 순수 증강 중 내부 타입 결정
            int rnd = UnityEngine.Random.Range(0, 2);
            switch (rnd)
            {
                case 0:
                    if (statAugmentDB != null && statAugmentDB.AbilityList.Count > 0)
                        return statAugmentDB.AbilityList[UnityEngine.Random.Range(0, statAugmentDB.AbilityList.Count)] as PureDataAugment;
                    break;
                case 1:
                    if (weaponAugmentDB != null && weaponAugmentDB.AbilityList.Count > 0)
                        return weaponAugmentDB.AbilityList[UnityEngine.Random.Range(0, weaponAugmentDB.AbilityList.Count)] as PureDataAugment;
                    break;
            }

            return null;
        }

        private PureDataAugment GetRandomWeapon()
        {
            if (weaponDB == null || weaponDB.WeaponList.Count == 0) return null;
            for (int attempt = 0; attempt < 10; attempt++)
            {
                var ch = weaponDB.WeaponList[UnityEngine.Random.Range(0, weaponDB.WeaponList.Count)];
                if (ch != null && !m_CurrentChoiceIDs.Contains(ch.ID)) return ch as PureDataAugment;
            }
            return null;
        }
    }
}

