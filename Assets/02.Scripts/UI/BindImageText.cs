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
        
        // [D] UI 바인딩용 데이터 (인터페이스 기반)
        private IBindableUIContent m_CurrentContent;

        private void Awake()
        {
            m_Image = GetComponentInChildren<Image>();
            m_TMPro = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetText(string text)
        {
            if (m_TMPro != null) m_TMPro.text = text;
        }

        public void SetImage(Sprite sprite)
        {
            if (m_Image != null) m_Image.sprite = sprite;
        }

        public void SetColor(Color color)
        {
            if (m_TMPro != null) m_TMPro.color = color;
            if (m_Image != null) m_Image.color = color;
        }

        /// <summary>
        /// 데이터를 바인딩하고 UI를 즉시 갱신합니다.
        /// </summary>
        public void Bind(IBindableUIContent data)
        {
            m_CurrentContent = data;
            
            // 데이터가 없는 경우 초기화
            if (m_CurrentContent == null)
            {
                SetText(string.Empty);
                SetImage(null);
                return;
            }

            // 텍스트 설정 (있을 때만)
            SetText(data.Name);

            // 이미지 설정 (데이터에 아이콘이 있고, UI에 이미지 컴포넌트가 있을 때만)
            if (data.Icon != null)
            {
                SetImage(data.Icon);
                if (m_Image != null) m_Image.enabled = true;
            }
            else
            {
                // 아이콘 데이터가 없으면 이미지 컴포넌트를 끄거나 투명하게 처리
                if (m_Image != null) m_Image.enabled = false;
            }
        }

        /// <summary>
        /// 바인딩된 원본 데이터를 특정 타입으로 가져옵니다. (Augment용)
        /// </summary>
        public T GetData<T>() where T : class, IBindableUIContent
        {
            return m_CurrentContent as T;
        }

        // --- 레거시 호환 및 편의용 ---
        public bool GetPureStatAbility(out PureDataStatAbility data) { data = GetData<PureDataStatAbility>(); return data != null; }
        public bool GetPureWeaponAbility(out PureDataWeaponAbility data) { data = GetData<PureDataWeaponAbility>(); return data != null; }
        
        public void SetPureStatAbility(PureDataStatAbility data) => Bind(data);
        public void SetPureWeaponAbility(PureDataWeaponAbility data) => Bind(data);
    }
}
