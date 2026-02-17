using UnityEngine;
using _02.Scripts.Managers.Choice;

namespace _02.Scripts.UI.Title
{
    /// <summary>
    /// [D] 증강 선택지 다시 뽑기(Reroll)를 수행하는 버튼 데이터입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "Button_Reroll", menuName = "PureData/Buttons/Reroll")]
    public class TitleButton_Reroll : PureDataButton
    {
        public override void Apply()
        {
            Debug.Log("[RerollButton] 다시 뽑기 로직 실행");
            
            // ChoiceSystem에게 새로운 선택지 생성을 요청합니다.
            if (ChoiceSystem.Instance != null)
            {
                ChoiceSystem.Instance.GenerateAndShowChoices();
            }
            else
            {
                Debug.LogError("[RerollButton] ChoiceSystem 인스턴스를 찾을 수 없습니다!");
            }
        }
    }
}
