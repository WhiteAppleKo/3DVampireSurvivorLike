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
        public PureDataBaseStatAbility statAbilityDB;
        public PureDataBaseWeaponAbility weaponAbilityDB;
        public PureDataBaseWeapon weaponDB;

        public GameObject choicePanel;
        public BindImageText[] bindImageText;
    
        private BaseAbility[] m_Abilities;
        private HashSet<string> m_CurrentChoiceIDs = new HashSet<string>();

        private enum e_ChoiceType
        {
            Augment,
            Weapon
        }
        private  e_ChoiceType m_ChoiceType;

        private void Awake()
        {
            m_Abilities = new BaseAbility[3];
        }

        public void RerollChoices()
        {
            m_CurrentChoiceIDs.Clear();
            for (int i = 0; i < 3; i++)
            {
                SetChoices(i);
            }
        }
        private void SetChoices(int index)
        {
            if (m_ChoiceType == e_ChoiceType.Augment)
            {
                int rnd = UnityEngine.Random.Range(0, 3);
                switch (rnd)
                {
                    case 0:
                    case 1:
                        SettingAbility(index);
                        break;
                    case 2:
                        SettingWeapon(index);
                        break;
                }
            }

            if (m_ChoiceType == e_ChoiceType.Weapon)
            {
                SettingWeapon(index);
            }
        }

        public void SetAugmentChoice(int index)
        {
            if (index == 0) m_CurrentChoiceIDs.Clear(); // 첫 번째 슬롯일 때 초기화
            m_ChoiceType = e_ChoiceType.Augment;
            SetChoices(index);
        }

        public void SetWeaponChoice(int index)
        {
            if (index == 0) m_CurrentChoiceIDs.Clear();
            m_ChoiceType = e_ChoiceType.Weapon;
            SettingWeapon(index);
        }

        private void SettingAbility(int index)
        {
            // 중복 방지를 위한 시도 횟수 제한 (무한 루프 방지)
            for (int attempt = 0; attempt < 10; attempt++)
            {
                int rnd = UnityEngine.Random.Range(0, 2);
                if (rnd == 0 && statAbilityDB != null && statAbilityDB.AbilityList.Count > 0)
                {
                    var ch = statAbilityDB.AbilityList[UnityEngine.Random.Range(0, statAbilityDB.AbilityList.Count)];
                    if (m_CurrentChoiceIDs.Contains(ch.ID)) continue;

                    m_CurrentChoiceIDs.Add(ch.ID);
                    bindImageText[index].SetText(ch.Name);
                    bindImageText[index].SetPureStatAbility(ch);
                    return;
                }
                else if (weaponAbilityDB != null && weaponAbilityDB.AbilityList.Count > 0)
                {
                    var ch = weaponAbilityDB.AbilityList[UnityEngine.Random.Range(0, weaponAbilityDB.AbilityList.Count)];
                    if (m_CurrentChoiceIDs.Contains(ch.ID)) continue;

                    m_CurrentChoiceIDs.Add(ch.ID);
                    bindImageText[index].SetText(ch.Name);
                    bindImageText[index].SetPureWeaponAbility(ch);
                    return;
                }
            }
        }

        private void SettingWeapon(int index)
        {
            for (int attempt = 0; attempt < 10; attempt++)
            {
                var ch = ChoiceWeapon();
                if (ch == null) return;
                if (m_CurrentChoiceIDs.Contains(ch.ID)) continue;

                m_CurrentChoiceIDs.Add(ch.ID);
                bindImageText[index].SetImage(ch.Icon);
                bindImageText[index].SetText(ch.Name);
                bindImageText[index].SetPureWeapon(ch);
                return;
            }
        }

        private PureDataWeapon ChoiceWeapon()
        {
            if (weaponDB == null || weaponDB.WeaponList.Count == 0) return null;
            int idx = UnityEngine.Random.Range(0, weaponDB.WeaponList.Count);
            return weaponDB.WeaponList[idx];
        }

        public void PopUpChoiceUI()
        {
            TimeScaleManager.Instance.SetTimeScale(0);
            choicePanel.SetActive(true);
        }
    }
}

