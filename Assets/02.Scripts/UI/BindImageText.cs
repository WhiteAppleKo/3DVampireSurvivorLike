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
        
        // [D] DLV 통합 데이터
        private PureDataAugment m_CurrentData;

        private void Awake()
        {
            m_Image = GetComponentInChildren<Image>();
            m_TMPro = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetText(string text) => m_TMPro.text = text;
        public void SetImage(Sprite sprite) => m_Image.sprite = sprite;

        /// <summary>
        /// 데이터를 바인딩하고 UI를 즉시 갱신합니다.
        /// </summary>
        public void Bind(PureDataAugment data)
        {
            m_CurrentData = data;
            if (m_CurrentData == null)
            {
                SetText(string.Empty);
                if (m_Image != null) m_Image.sprite = null;
                return;
            }

            SetText(data.Name);
            if (data.Icon != null) SetImage(data.Icon);
        }

        public T GetData<T>() where T : PureDataAugment
        {
            return m_CurrentData as T;
        }

        // --- 레거시 호환용 (필요 시 점진적 제거) ---
        public bool GetPureStatAbility(out PureDataStatAbility data) { data = GetData<PureDataStatAbility>(); return data != null; }
        public bool GetPureWeaponAbility(out PureDataWeaponAbility data) { data = GetData<PureDataWeaponAbility>(); return data != null; }
        public bool GetPureWeapon(out PureDataWeapon data) { data = GetData<PureDataWeapon>(); return data != null; }
        
        public void SetPureStatAbility(PureDataStatAbility data) => Bind(data);
        public void SetPureWeaponAbility(PureDataWeaponAbility data) => Bind(data);
        public void SetPureWeapon(PureDataWeapon data) => Bind(data);
    }
}
