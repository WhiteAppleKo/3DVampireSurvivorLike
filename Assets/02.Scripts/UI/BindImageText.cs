using _02.Scripts.AutoAttack;
using _02.Scripts.Managers.Choice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02.Scripts.UI
{
    public class BindImageText : MonoBehaviour
    {
        private Image m_Image;
        private TextMeshProUGUI m_TMPro;
        private BaseAbility m_Ability;
        private BaseWeaponData m_WeaponData;
        private int m_ChoiceType;

        private void Awake()
        {
            m_Image = GetComponentInChildren<Image>();
            m_TMPro = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetText(string text)
        {
            m_TMPro.text = text;
        }

        public void SetImage(Sprite sprite)
        {
            m_Image.sprite = sprite;
        }

        public void SetAbility(BaseAbility ability)
        {
            m_Ability = ability;
            m_WeaponData = null;
            m_ChoiceType = 0;
        }

        public bool GetAbility(out BaseAbility ability)
        {
            ability = m_Ability;
            return ability != null;
        }

        public void SetWeaponData(BaseWeaponData data)
        {
            m_WeaponData = data;
            m_Ability = null;
            m_ChoiceType = 1;
        }

        public bool GetWeaponData(out BaseWeaponData weaponData)
        {
            weaponData = m_WeaponData;
            return weaponData != null;
        }
    }
}
