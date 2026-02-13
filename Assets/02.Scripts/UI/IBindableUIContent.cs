using UnityEngine;

namespace _02.Scripts.UI
{
    /// <summary>
    /// UI 바인딩(이름, 아이콘)을 위한 공통 인터페이스
    /// </summary>
    public interface IBindableUIContent
    {
        string Name { get; }
        Sprite Icon { get; }
        string Description { get; }
    }
}
