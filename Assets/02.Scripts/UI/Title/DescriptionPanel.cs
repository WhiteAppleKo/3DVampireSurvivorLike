using UnityEngine;
using Shapes;

namespace _02.Scripts.UI.Title
{
    /// <summary>
    /// [V] 설명이나 정보 출력을 위한 단순 패널입니다.
    /// ButtonPanel의 비주얼 스타일은 유지하되, 상호작용(Hover, Press 등) 기능을 제외합니다.
    /// </summary>
    [ExecuteAlways]
    public class DescriptionPanel : ImmediateModePanel
    {
        public enum SizeMode { Fill, Fixed, AspectRatio }

        [Header("외형 설정")]
        public float cornerRadius = 12f;
        public float thickness = 4f;
        public float padding = 4f;

        [Header("크기 및 비율 설정")]
        public SizeMode sizeMode = SizeMode.Fill;
        public Vector2 fixedSize = new Vector2(400, 200);
        public float aspectRatio = 1.0f; // AspectRatio 모드에서 사용 (가로/세로)

        [Header("색상 설정")]
        [ColorUsage(true, true)] public Color borderColor = Color.white;
        [ColorUsage(true, true)] public Color backgroundColor = new Color(0, 0, 0, 0.5f);

        public override void DrawPanelShapes(Rect rect, ImCanvasContext ctx)
        {
            // 1. 크기 및 위치 계산
            Rect drawRect = Inset(rect, padding);
            Vector2 finalSize = drawRect.size;

            if (sizeMode == SizeMode.Fixed)
            {
                finalSize = fixedSize;
            }
            else if (sizeMode == SizeMode.AspectRatio)
            {
                finalSize.y = finalSize.x / aspectRatio;
            }

            // 중앙 기준 Rect 생성
            Vector2 center = drawRect.center;
            Rect scaledRect = new Rect(center.x - finalSize.x * 0.5f, center.y - finalSize.y * 0.5f, finalSize.x, finalSize.y);
            
            // 2. 그리기 (배경 및 테두리)
            Draw.Rectangle(scaledRect, cornerRadius, backgroundColor);
            Draw.RectangleBorder(scaledRect, thickness, cornerRadius, borderColor);
        }

        private Rect Inset(Rect r, float amount)
        {
            return new Rect(r.x + amount, r.y + amount, r.width - amount * 2, r.height - amount * 2);
        }
        
        /// <summary>
        /// 외부에서 색상을 동적으로 변경할 때 사용합니다.
        /// </summary>
        public void SetColor(Color color)
        {
            borderColor = color;
        }
    }
}
