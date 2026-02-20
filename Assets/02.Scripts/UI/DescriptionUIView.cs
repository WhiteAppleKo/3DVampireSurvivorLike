using UnityEngine;
using _02.Scripts.UI.Title;

namespace _02.Scripts.UI
{
    /// <summary>
    /// [V] 단순 정보(설명 등)를 표시하기 위한 뷰 컴포넌트입니다.
    /// 버튼 기능이 없으며, 시각적인 데이터 바인딩만 수행합니다.
    /// </summary>
    public class DescriptionUIView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private BindImageText contentBinder;
        [SerializeField] private DescriptionPanel panel;

        // 현재 바인딩된 데이터 저장
        private IBindableUIContent m_CurrentContent;
        public IBindableUIContent CurrentContent => m_CurrentContent;

        private void Awake()
        {
            // 컴포넌트 자동 찾기
            if (contentBinder == null) contentBinder = GetComponent<BindImageText>();
            if (panel == null) panel = GetComponent<DescriptionPanel>();
        }

        /// <summary>
        /// 데이터를 바인딩합니다.
        /// </summary>
        public void Bind(IBindableUIContent data)
        {
            m_CurrentContent = data;
            if (contentBinder != null)
            {
                contentBinder.Bind(data);
            }
        }

        /// <summary>
        /// 패널의 테두리 색상을 설정합니다.
        /// </summary>
        public void SetColor(Color color)
        {
            if (panel != null) panel.SetColor(color);
            if (contentBinder != null) contentBinder.SetColor(color);
        }
    }
}
