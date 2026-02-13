using UnityEngine;

namespace _02.Scripts.UI.Title
{
    /// <summary>
    /// [D] 타이틀 씬의 버튼 정보를 담는 데이터 클래스.
    /// 임포터 호환성 없이 UI 전용으로 설계되었습니다.
    /// </summary>
    [CreateAssetMenu(fileName = "PureDataTitleButton", menuName = "PureData/UI/TitleButton")]
    public class PureDataTitleButton : ScriptableObject, IBindableUIContent
    {
        [Header("Button Logic")]
        [SerializeField] private string id;
        public string ID => id;

        [Header("UI Display")]
        [SerializeField] private string buttonName;
        [SerializeField] [TextArea] private string description;
        [SerializeField] private Sprite icon;

        // IBindableUIContent 구현
        public string Name => buttonName;
        public string Description => description;
        public Sprite Icon => icon;
    }
}
