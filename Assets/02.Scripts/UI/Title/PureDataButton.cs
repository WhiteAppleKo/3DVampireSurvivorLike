using UnityEngine;

namespace _02.Scripts.UI.Title
{
    /// <summary>
    /// [D] 타이틀 및 일반 UI 버튼 정보를 담는 추상 데이터 클래스.
    /// ScriptableObject를 상속받아 인스펙터에서 다양한 실행 로직을 가질 수 있습니다.
    /// </summary>
    public abstract class PureDataButton : ScriptableObject, IBindableUIContent
    {
        [Header("Common Logic")]
        [SerializeField] private string id;
        public string ID => id;

        [Header("UI Display")]
        [SerializeField] private string buttonName;
        [SerializeField] [TextArea] private string description;
        [SerializeField] private Sprite icon;

        // IBindableUIContent 구현
        public string Name => buttonName;
        public Sprite Icon => icon;
        public string Description => description;

        /// <summary>
        /// 버튼이 클릭되었을 때 실행될 로직입니다. 
        /// 자식 클래스에서 구체적으로 구현합니다. (NewGame, Load, Exit 등)
        /// </summary>
        public abstract void Apply();
    }
}
