using _02.Scripts.AutoAttack;
using _02.Scripts.UI;
using UnityEngine;

namespace _02.Scripts.Managers.Choice
{
    public class ChoiceSystem : MonoBehaviour
    {
        public StatAbilityDatabase statAbilityDatabase;
        public WeaponAbilityDatabase weaponAbilityDatabase;
        public WeaponDatabase weaponDatabase;
        public GameObject choicePanel;
        public BindImageText[] bindImageText;
    
        private BaseAbility[] m_Abilities;
        private int m_StatCount;
        private int m_WeponCont;

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
                        SettingAbility(index);
                        break;
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
            m_ChoiceType = e_ChoiceType.Augment;
            SetChoices(index);
        }

        public void SetWeaponChoice(int index)
        {
            m_ChoiceType = e_ChoiceType.Weapon;
            SettingWeapon(index);
        }

        private void SettingAbility(int index)
        {
            var ch = ChoiceAbilities();
            bindImageText[index].SetImage(ch.icon);
            bindImageText[index].SetText(ch.abilityName);
            bindImageText[index].SetAbility(ch);
        }

        private void SettingWeapon(int index)
        {
            var ch = ChoiceWeapon();
            bindImageText[index].SetImage(ch.icon);
            bindImageText[index].SetText(ch.weaponName);
            bindImageText[index].SetWeaponData(ch);
        }

        private int m_Rnd;
        private int m_ChoiceIndex;
        private BaseAbility ChoiceAbilities()
        {
            //0이면 스탯 1이면 장비
            m_Rnd = UnityEngine.Random.Range(0, 2);
            switch (m_Rnd)
            {
                case 0:
                    m_ChoiceIndex = UnityEngine.Random.Range(0, statAbilityDatabase.statAbilities.Count);
                    return statAbilityDatabase.statAbilities[m_ChoiceIndex];
                case 1:
                    m_ChoiceIndex = UnityEngine.Random.Range(0, weaponAbilityDatabase.weaponAbilities.Count);
                    return weaponAbilityDatabase.weaponAbilities[m_ChoiceIndex];
            }
            return null;
        }

        private BaseWeaponData ChoiceWeapon()
        {
            m_ChoiceIndex = UnityEngine.Random.Range(0, weaponDatabase.weaponDatas.Count);
            return weaponDatabase.weaponDatas[m_ChoiceIndex];
        }

        public void PopUpChoiceUI()
        {
            TimeScaleManager.Instance.SetTimeScale(0);
            choicePanel.SetActive(true);
        }
    }
}

