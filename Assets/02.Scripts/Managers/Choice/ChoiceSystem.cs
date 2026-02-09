using System.Collections.Generic;
using _02.Scripts.AutoAttack;
using _02.Scripts.UI;
using Features.Augment;
using Features.Weapon;
using UnityEngine;

namespace _02.Scripts.Managers.Choice
{
    public class ChoiceSystem : MonoBehaviour
    {
        [Header("DLV Databases")]
        public PureDataBaseStatAbility statAugmentDB;
        public PureDataBaseWeaponAbility weaponAugmentDB;
        public PureDataBaseWeapon weaponDB;

        [Header("References")]
        public PlayerController player;

        // 선택지 생성 완료 이벤트 (UIView가 이를 구독)
        public event System.Action<List<PureDataAugment>> OnAugmentsGenerated;

        private enum e_ChoiceType { Augment, Weapon }
        private e_ChoiceType m_ChoiceType;
        private HashSet<string> m_CurrentChoiceIDs = new HashSet<string>();

        private void OnEnable()
        {
            if (player != null && player.Model != null)
            {
                player.Model.OnLevelUp += OnPlayerLevelUp;
            }
        }

        private void OnDisable()
        {
            if (player != null && player.Model != null)
            {
                player.Model.OnLevelUp -= OnPlayerLevelUp;
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
            Debug.Log("[ChoiceSystem] GenerateAugmentChoices 시작");
            m_CurrentChoiceIDs.Clear();
            List<PureDataAugment> results = new List<PureDataAugment>();

            for (int i = 0; i < 3; i++)
            {
                PureDataAugment choice = GetRandomAugment();
                if (choice != null)
                {
                    results.Add(choice);
                    m_CurrentChoiceIDs.Add(choice.ID);
                }
            }

            Debug.Log($"[ChoiceSystem] {results.Count}개의 선택지 생성 완료. 이벤트 발송.");
            OnAugmentsGenerated?.Invoke(results);
        }

        private PureDataAugment GetRandomAugment()
        {
            // [Logic] 선택지 추출 로직 (e_ChoiceType에 따른 분기)
            if (m_ChoiceType == e_ChoiceType.Weapon)
            {
                return GetRandomWeapon();
            }
            
            // Augment 모드일 때 (랜덤하게 무기 또는 증강)
            int rnd = UnityEngine.Random.Range(0, 3);
            if (rnd == 2) return GetRandomWeapon();
            return GetRandomPureAugment();
        }

        private PureDataAugment GetRandomPureAugment()
        {
            for (int attempt = 0; attempt < 10; attempt++)
            {
                PureDataAugment ch = null;
                int rnd = UnityEngine.Random.Range(0, 2);
                if (rnd == 0 && statAugmentDB != null)
                    ch = statAugmentDB.AbilityList[UnityEngine.Random.Range(0, statAugmentDB.AbilityList.Count)] as PureDataAugment;
                else if (weaponAugmentDB != null)
                    ch = weaponAugmentDB.AbilityList[UnityEngine.Random.Range(0, weaponAugmentDB.AbilityList.Count)] as PureDataAugment;

                if (ch != null && !m_CurrentChoiceIDs.Contains(ch.ID)) return ch;
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

