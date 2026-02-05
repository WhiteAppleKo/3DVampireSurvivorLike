using UnityEngine;

namespace Features.ExpCristal
{
    public interface IExpCristalVisualizer
    {
        // 현재 위치 (읽기/쓰기)
        Vector3 Position { get; set; }
        
        // 활성화/비활성화
        void SetActive(bool isActive);
    }
}
