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
        [Header("UI Components")]
        [SerializeField] private Image m_Image;
        [SerializeField] private TextMeshProUGUI m_TitleText;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;
        
        // [D] UI 바인딩용 데이터 (인터페이스 기반)
        private IBindableUIContent m_CurrentContent;

        public void ButtonAwake()
        {
            // 수동 할당이 안 된 경우에만 자동 찾기 시도 (기존 호환성 유지)
            if (m_Image == null) m_Image = GetComponentInChildren<Image>();
            if (m_TitleText == null) m_TitleText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetText(string text)
        {
            if (m_TitleText != null) m_TitleText.text = text;
        }

        public void SetDescription(string text)
        {
            if (m_DescriptionText != null) m_DescriptionText.text = text;
        }

        public void SetImage(Sprite sprite)
        {
            if (m_Image != null) m_Image.sprite = sprite;
        }

        public void SetColor(Color color)
        {
            if (m_TitleText != null) m_TitleText.color = color;
            if (m_DescriptionText != null) m_DescriptionText.color = color;
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
                SetDescription(string.Empty);
                SetImage(null);
                return;
            }

            // 텍스트 설정 (제목 및 설명)
            SetText(data.Name);
            SetDescription(data.Description);

            // 이미지 설정
            if (data.Icon != null)
            {
                SetImage(data.Icon);
                if (m_Image != null) m_Image.enabled = true;
            }
            else
            {
                if (m_Image != null) m_Image.enabled = false;
            }
        }

        // ... (기존 GetData 등 메서드는 유지)
        
        /// <summary>
        /// 바인딩된 원본 데이터를 특정 타입으로 가져옵니다. (Augment용)
        /// </summary>
        public T GetData<T>() where T : class, IBindableUIContent
        {
            return m_CurrentContent as T;
        }

        public bool GetPureStatAbility(out PureDataStatAbility data) { data = GetData<PureDataStatAbility>(); return data != null; }
        public bool GetPureWeaponAbility(out PureDataWeaponAbility data) { data = GetData<PureDataWeaponAbility>(); return data != null; }
        public void SetPureStatAbility(PureDataStatAbility data) => Bind(data);
        public void SetPureWeaponAbility(PureDataWeaponAbility data) => Bind(data);
    }
}
