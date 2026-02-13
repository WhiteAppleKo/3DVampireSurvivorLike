using UnityEngine;
using UnityEngine.UI;
using Features.Augment;

namespace _02.Scripts.UI.Title
{
    /// <summary>
    /// [V] 개별 버튼의 비주얼 및 이벤트를 담당합니다.
    /// 증강 선택지 UI의 외형(BindImageText)을 재활용합니다.
    /// </summary>
    public class TitleButtonUIView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PureDataTitleButton buttonData;

        [Header("UI References")]
        [SerializeField] private BindImageText contentBinder;
        [SerializeField] private Button targetButton;
        [SerializeField] private TitleButtonPanel buttonPanel;

        public System.Action<string> OnClicked;

        private void Awake()
        {
            // 1. 자기 자신에게서 필요한 컴포넌트를 자동으로 찾습니다.
            if (targetButton == null) targetButton = GetComponent<Button>();
            if (contentBinder == null) contentBinder = GetComponent<BindImageText>();
            if (buttonPanel == null) buttonPanel = GetComponent<TitleButtonPanel>();

            // 2. 버튼 클릭 이벤트 연결 (null 체크 포함)
            if (targetButton != null)
            {
                targetButton.onClick.AddListener(() => {
                    if (buttonData != null) OnClicked?.Invoke(buttonData.ID);
                });
            }
            else
            {
                Debug.LogError($"[TitleButtonUI] {gameObject.name}에 Button 컴포넌트가 없습니다!");
            }
            
            // 3. 데이터 바인딩 (내용물 출력)
            if (buttonData != null && contentBinder != null)
            {
                contentBinder.Bind(buttonData);
            }
            else if (contentBinder == null)
            {
                Debug.LogWarning($"[TitleButtonUI] {gameObject.name}에 BindImageText 컴포넌트가 없습니다!");
            }
        }

        public string ButtonID => buttonData != null ? buttonData.ID : string.Empty;
        
        /// <summary>
        /// 버튼의 상호작용 상태를 설정하고 시각적(Shapes 패널, 텍스트 색상) 피드백을 동기화합니다.
        /// </summary>
        public void SetInteractable(bool state)
        {
            // 1. 실제 버튼 클릭 기능 제어
            if (targetButton != null) targetButton.interactable = state;

            // 2. Shapes 패널 상태 제어 (Hover, Pressed 효과 무시 및 색상 고정)
            if (buttonPanel != null) buttonPanel.SetInteractable(state);

            // 3. 텍스트 및 아이콘 색상 제어 (비활성화 시 어둡게)
            if (contentBinder != null)
            {
                Color targetColor = state ? Color.white : new Color(0.4f, 0.4f, 0.4f, 0.6f);
                contentBinder.SetColor(targetColor);
            }
        }

        /// <summary>
        /// 포커스 상태를 설정하여 크기 확대 연출을 활성화합니다.
        /// </summary>
        public void SetFocus(bool focused)
        {
            if (buttonPanel != null) buttonPanel.SetFocus(focused);
        }

        /// <summary>
        /// 상세 UX 상태(포커스, 거리, 타입)를 설정합니다.
        /// </summary>
        public void SetUXState(bool focused, float distance, ChoiceUXType uxType)
        {
            if (buttonPanel != null) buttonPanel.SetUXState(focused, distance, uxType);
        }
    }
}
