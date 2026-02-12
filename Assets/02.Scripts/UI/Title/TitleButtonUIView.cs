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

        public System.Action<string> OnClicked;

        private void Awake()
        {
            if (targetButton == null) targetButton = GetComponent<Button>();
            
            targetButton.onClick.AddListener(() => {
                if (buttonData != null) OnClicked?.Invoke(buttonData.ID);
            });
            
            // 데이터 바인딩 (증강 UI와 동일한 비주얼 출력)
            if (buttonData != null && contentBinder != null)
            {
                contentBinder.Bind(buttonData);
            }
        }

        public string ButtonID => buttonData != null ? buttonData.ID : string.Empty;
        
        public void SetInteractable(bool state)
        {
            if (targetButton != null) targetButton.interactable = state;
        }
    }
}
