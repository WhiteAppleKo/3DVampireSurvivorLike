using System.Collections.Generic;
using _02.Scripts.AutoAttack;
using _02.Scripts.UI;
using Features.Augment;
using Features.Player;
using Features.Weapon;
using UnityEngine;

namespace _02.Scripts.Managers.Choice
{
    /// <summary>
    /// [L] 증강 선택 로직을 총괄합니다. 
    /// UI를 직접 제어하여 선택 페이즈를 관리합니다.
    /// </summary>
    public class ChoiceSystem : SingletoneBase<ChoiceSystem>
    {
        [Header("DLV Databases")]
        public PureDataBaseStatAbility statAugmentDB;
        public PureDataBaseWeaponAbility weaponAugmentDB;
        public PureDataBaseWeapon weaponDB;

        [Header("References")]
        public PlayerController player;
        [SerializeField] private ChoiceUIView choiceUIView; // UI 직접 참조

        private RuntimeDataPlayer m_Model;
        private enum e_ChoiceType { Augment, Weapon }
        private e_ChoiceType m_ChoiceType;
        private HashSet<string> m_CurrentChoiceIDs = new HashSet<string>();

        public void Bind(RuntimeDataPlayer model)
        {
            if (m_Model != null) m_Model.OnLevelUp -= OnPlayerLevelUp;
            m_Model = model;
            m_Model.OnLevelUp += OnPlayerLevelUp;
            Debug.Log("[ChoiceSystem] Player Model 바인딩 완료");
        }

        private void OnDisable()
        {
            if (m_Model != null) m_Model.OnLevelUp -= OnPlayerLevelUp;
        }

        private void OnPlayerLevelUp(int currentLevel)
        {
            SetAugmentChoiceMode();
        }

        public void SetAugmentChoiceMode()
        {
            m_ChoiceType = e_ChoiceType.Augment;
            GenerateAndShowChoices();
        }

        public void SetWeaponChoiceMode()
        {
            m_ChoiceType = e_ChoiceType.Weapon;
            GenerateAndShowChoices();
        }

        /// <summary>
        /// 선택지를 생성하고 UI를 즉시 활성화합니다.
        /// </summary>
        public void GenerateAndShowChoices()
        {
            if (choiceUIView == null)
            {
                Debug.LogError("[ChoiceSystem] ChoiceUIView가 할당되지 않았습니다!");
                return;
            }

            Debug.Log($"[ChoiceSystem] 선택지 생성 및 UI 활성화 시작. 모드: {m_ChoiceType}");
            
            m_CurrentChoiceIDs.Clear();
            List<PureDataAugment> results = new List<PureDataAugment>();

            while (results.Count < 3)
            {
                PureDataAugment choice = GetRandomAugment();
                if (choice != null && !m_CurrentChoiceIDs.Contains(choice.ID))
                {
                    results.Add(choice);
                    m_CurrentChoiceIDs.Add(choice.ID);
                }
            }

            // [핵심] UI에게 직접 명령
            choiceUIView.ShowChoices(results);
        }

        private PureDataAugment GetRandomAugment()
        {
            if (m_ChoiceType == e_ChoiceType.Weapon) return GetRandomWeapon();
            int rnd = UnityEngine.Random.Range(0, 3);
            return (rnd == 2) ? GetRandomWeapon() : GetRandomPureAugment();
        }

        private PureDataAugment GetRandomPureAugment()
        {
            int rnd = UnityEngine.Random.Range(0, 2);
            if (rnd == 0 && statAugmentDB?.AbilityList.Count > 0)
                return statAugmentDB.AbilityList[UnityEngine.Random.Range(0, statAugmentDB.AbilityList.Count)] as PureDataAugment;
            if (weaponAugmentDB?.AbilityList.Count > 0)
                return weaponAugmentDB.AbilityList[UnityEngine.Random.Range(0, weaponAugmentDB.AbilityList.Count)] as PureDataAugment;
            return null;
        }

        private PureDataAugment GetRandomWeapon()
        {
            if (weaponDB == null || weaponDB.WeaponList.Count == 0) return null;
            return weaponDB.WeaponList[UnityEngine.Random.Range(0, weaponDB.WeaponList.Count)] as PureDataAugment;
        }
    }
}
