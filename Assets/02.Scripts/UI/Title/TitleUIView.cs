using UnityEngine;
using System.Collections.Generic;

namespace _02.Scripts.UI.Title
{
    /// <summary>
    /// [V] Title 씬의 전체 버튼들을 관리하고 로직과 연결합니다.
    /// </summary>
    public class TitleUIView : MonoBehaviour
    {
        [Header("Logic Reference")]
        [SerializeField] private TitleLogicSystem logicSystem;

        [Header("Title Buttons")]
        [SerializeField] private List<TitleButtonUIView> titleButtons;

        private void Start()
        {
            if (logicSystem == null)
            {
                Debug.LogError("[TitleUI] TitleLogicSystem이 할당되지 않았습니다.");
                return;
            }

            foreach (var buttonView in titleButtons)
            {
                if (buttonView == null) continue;

                // 1. 버튼 클릭 이벤트 연결
                buttonView.OnClicked += logicSystem.HandleButtonAction;

                // 2. 'Load' 버튼 활성화 상태 체크 (Data 기반 시각화)
                if (buttonView.ButtonID == "Load")
                {
                    bool canLoad = DataHub.Instance.HasSaveFile;
                    buttonView.SetInteractable(canLoad);
                    Debug.Log($"[TitleUI] 불러오기 버튼 상태 설정: {canLoad}");
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var buttonView in titleButtons)
            {
                if (buttonView != null)
                    buttonView.OnClicked -= logicSystem.HandleButtonAction;
            }
        }
    }
}
