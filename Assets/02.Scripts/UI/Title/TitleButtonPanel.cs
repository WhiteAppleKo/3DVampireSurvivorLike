using UnityEngine;
using Shapes;
using UnityEngine.EventSystems;

namespace _02.Scripts.UI.Title
{
    /// <summary>
    /// [V] 개별 타이틀 버튼의 Shapes 외형을 담당하는 패널.
    /// 마우스 상호작용에 따른 색상 변화와 부드러운 테두리 연출을 제공합니다.
    /// </summary>
    [ExecuteAlways]
    public class TitleButtonPanel : ImmediateModePanel, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("외형 설정")]
        public float cornerRadius = 12f;
        public float thickness = 4f;
        public float padding = 4f;

        [Header("색상 설정")]
        [ColorUsage(true, true)] public Color normalColor = Color.white;
        [ColorUsage(true, true)] public Color hoverColor = Color.cyan;
        [ColorUsage(true, true)] public Color pressedColor = Color.gray;
        [ColorUsage(true, true)] public Color backgroundColor = new Color(0, 0, 0, 0.5f);

        private Color m_TargetColor;
        private Color m_CurrentColor;
        private bool m_IsHovered;
        private bool m_IsPressed;

        public override void OnEnable()
        {
            base.OnEnable();
            m_CurrentColor = normalColor;
            m_TargetColor = normalColor;
        }

        public override void DrawPanelShapes(Rect rect, ImCanvasContext ctx)
        {
            // 1. 상태에 따른 목표 색상 결정
            if (m_IsPressed) m_TargetColor = pressedColor;
            else if (m_IsHovered) m_TargetColor = hoverColor;
            else m_TargetColor = normalColor;

            // 2. 부드러운 색상 전환 (애니메이션)
            m_CurrentColor = Color.Lerp(m_CurrentColor, m_TargetColor, Time.unscaledDeltaTime * 15f);

            // 3. 그리기 영역 계산
            Rect drawRect = Inset(rect, padding);

            // 4. 배경 사각형 그리기
            Draw.Rectangle(drawRect, cornerRadius, backgroundColor);

            // 5. 테두리 그리기
            Draw.RectangleBorder(drawRect, thickness, cornerRadius, m_CurrentColor);
        }

        // --- 마우스 이벤트 핸들러 ---
        public void OnPointerEnter(PointerEventData eventData) => m_IsHovered = true;
        public void OnPointerExit(PointerEventData eventData) => m_IsHovered = false;
        public void OnPointerDown(PointerEventData eventData) => m_IsPressed = true;
        public void OnPointerUp(PointerEventData eventData) => m_IsPressed = false;

        private Rect Inset(Rect r, float amount)
        {
            return new Rect(r.x + amount, r.y + amount, r.width - amount * 2, r.height - amount * 2);
        }
    }
}
