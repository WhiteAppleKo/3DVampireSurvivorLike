using _02.Scripts.AutoAttack;
using _02.Scripts.Managers.Choice;
using Features.Augment;
using Features.Weapon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02.Scripts.UI
{
    public class BindImageText : MonoBehaviour
    {
        private Image m_Image;
        private TextMeshProUGUI m_TMPro;
        
        // Legacy
        private BaseAbility m_Ability;
        private BaseWeaponData m_WeaponData;
        
        // DLV
        private PureDataStatAbility m_PureStatAbility;
        private PureDataWeaponAbility m_PureWeaponAbility;
        private PureDataWeapon m_PureWeapon;

        private void Awake()
        {
            m_Image = GetComponentInChildren<Image>();
            m_TMPro = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetText(string text) => m_TMPro.text = text;
        public void SetImage(Sprite sprite) => m_Image.sprite = sprite;

        private void ClearAllData()
        {
            m_Ability = null;
            m_WeaponData = null;
            m_PureStatAbility = null;
            m_PureWeaponAbility = null;
            m_PureWeapon = null;
        }

        // --- Setters ---
        public void SetAbility(BaseAbility ability) { ClearAllData(); m_Ability = ability; }
        public void SetWeaponData(BaseWeaponData data) { ClearAllData(); m_WeaponData = data; }
        public void SetPureStatAbility(PureDataStatAbility data) { ClearAllData(); m_PureStatAbility = data; }
        public void SetPureWeaponAbility(PureDataWeaponAbility data) { ClearAllData(); m_PureWeaponAbility = data; }
        public void SetPureWeapon(PureDataWeapon data) { ClearAllData(); m_PureWeapon = data; }

        // --- Getters ---
        public bool GetAbility(out BaseAbility ability) { ability = m_Ability; return ability != null; }
        public bool GetWeaponData(out BaseWeaponData weaponData) { weaponData = m_WeaponData; return weaponData != null; }
        
        public bool GetPureStatAbility(out PureDataStatAbility data) { data = m_PureStatAbility; return data != null; }
        public bool GetPureWeaponAbility(out PureDataWeaponAbility data) { data = m_PureWeaponAbility; return data != null; }
        public bool GetPureWeapon(out PureDataWeapon data) { data = m_PureWeapon; return data != null; }
    }
}
