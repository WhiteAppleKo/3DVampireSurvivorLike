using UnityEngine;

namespace _02.Scripts.UI
{
    /// <summary>
    /// UI 바인딩(이름, 아이콘) 및 실행 로직을 위한 공통 인터페이스
    /// </summary>
    public interface IBindableUIContent
    {
        string Name { get; }
        Sprite Icon { get; }
        string Description { get; }

        /// <summary>
        /// 데이터가 선택되었을 때 실행될 로직
        /// </summary>
        void Apply();
    }
}
