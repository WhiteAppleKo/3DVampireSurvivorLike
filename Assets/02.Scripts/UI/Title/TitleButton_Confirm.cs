using UnityEngine;

namespace _02.Scripts.UI.Title
{
    /// <summary>
    /// [D] 선택을 확정짓는 'Confirm' 버튼을 위한 데이터 클래스입니다.
    /// 현재 활성화된 하이브리드 UI를 찾아 확정 로직을 트리거합니다.
    /// </summary>
    [CreateAssetMenu(fileName = "Button_Confirm", menuName = "PureData/Buttons/Confirm")]
    public class TitleButton_Confirm : PureDataButton
    {
        public override void Apply()
        {
            Debug.Log("[ConfirmButton] 확정 버튼 데이터 Apply 호출");

            // 1. 현재 씬에서 활성화된 하이브리드 UI 찾기
            // (보통 선택창은 하나만 열리므로 FindFirstObjectByType 사용)
            var activeUI = Object.FindFirstObjectByType<BaseHybridScrollUIView>();

            if (activeUI != null)
            {
                // 2. 해당 UI의 확정 로직 실행
                activeUI.ConfirmSelection();
            }
            else
            {
                Debug.LogWarning("[ConfirmButton] 실행할 활성 하이브리드 UI를 찾을 수 없습니다.");
            }
        }
    }
}
