using UnityEngine;
using UnityEngine.UI;
using _02.Scripts.UI.Title;

namespace _02.Scripts.UI
{
    /// <summary>
    /// [V] 타이틀 및 인게임 UI에서 공용으로 사용하는 버튼 뷰 컴포넌트입니다.
    /// 하이브리드 애니메이션(Shapes)과 데이터 바인딩(BindImageText)을 통합 관리합니다.
    /// </summary>
    public class ButtonUIView : MonoBehaviour
    {
        [Header("Data (Optional for Title)")]
        [SerializeField] private Title.PureDataButton defaultData;

        [Header("Settings")]
        [SerializeField] private bool applyOnClicked = false; // 클릭 시 즉시 Apply 실행 여부

        [Header("UI References")]
        [SerializeField] private BindImageText contentBinder;
        [SerializeField] private Button targetButton;
        [SerializeField] private Title.ButtonPanel buttonPanel;

        // 클릭 이벤트 (위치 이동용)
        public System.Action OnClicked;

        // 현재 바인딩된 데이터 저장
        private IBindableUIContent m_CurrentContent;
        public IBindableUIContent CurrentContent => m_CurrentContent;

        private void Awake()
        {
            // 컴포넌트 자동 찾기
            if (targetButton == null) targetButton = GetComponent<Button>();
            if (contentBinder == null) contentBinder = GetComponent<BindImageText>();
            if (buttonPanel == null) buttonPanel = GetComponent<Title.ButtonPanel>();

            // 클릭 이벤트 연결
            if (targetButton != null)
            {
                targetButton.onClick.AddListener(() => 
                {
                    // 1. 위치 이동 이벤트 발생
                    OnClicked?.Invoke();

                    // 2. 옵션이 켜져있다면 즉시 로직 실행
                    if (applyOnClicked && m_CurrentContent != null)
                    {
                        m_CurrentContent.Apply();
                    }
                });
            }

            // 인스펙터에 데이터가 미리 할당되어 있다면 즉시 바인딩 (고정 버튼용)
            if (defaultData != null)
            {
                Bind(defaultData);
            }
        }

        public string ButtonID => defaultData != null ? defaultData.ID : string.Empty;

        /// <summary>
        /// 런타임에 데이터를 바인딩합니다. (인게임용)
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
        /// 버튼의 상호작용 상태를 설정합니다.
        /// </summary>
        public void SetInteractable(bool state)
        {
            if (targetButton != null) targetButton.interactable = state;
            if (buttonPanel != null) buttonPanel.SetInteractable(state);
            
            if (contentBinder != null)
            {
                Color targetColor = state ? Color.white : new Color(0.4f, 0.4f, 0.4f, 0.6f);
                contentBinder.SetColor(targetColor);
            }
        }

        /// <summary>
        /// 하이브리드 UX 상태(포커스, 거리, 이동 상태)를 설정합니다.
        /// </summary>
        public void SetUXState(bool focused, float distance, bool isMoving)
        {
            if (buttonPanel != null)
            {
                buttonPanel.SetUXState(focused, distance, isMoving);
            }
        }
    }
}
