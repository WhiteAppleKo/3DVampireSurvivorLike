using UnityEngine;
using Shapes;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _02.Scripts.UI.Title
{
    /// <summary>
    /// [V] Title 씬 배경을 위한 육각형 그리드 렌더러.
    /// 연산량 최적화 버전 (드로우 콜 및 루프 연산 최소화)
    /// </summary>
    [ExecuteAlways]
    public class TitleHexBackgroundRenderer : ImmediateModeShapeDrawer
    {
        [Header("Grid Settings")]
        public float hexRadius = 1f;
        public float thickness = 0.05f;
        [Range(5, 30)] public int viewDistanceX = 12; // 최적화: 기본 범위 축소
        [Range(5, 30)] public int viewDistanceY = 8;

        [Header("Color Settings")]
        [ColorUsage(true, true)] public Color color = Color.cyan * 0.5f; 
        public bool useOutline = true;
        [ColorUsage(true, true)] public Color outlineColor = Color.black; 
        public float outlineAddThickness = 0.02f;

        [Header("Animation Settings")]
        public Vector2 scrollSpeed = new Vector2(0.2f, 0.1f);
        public bool usePulse = true;
        public float pulseSpeed = 1.5f;
        public float pulseIntensity = 0.3f;

        [Header("Rendering Settings")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        private Vector2 m_VirtualScrollPos;
        private Color m_FinalFaceColor;
        private float m_HexWidth;
        private float m_HexHeight;
        private Quaternion m_HexRot = Quaternion.Euler(0, 0, 30f);

        private void Update()
        {
            if (Application.isPlaying)
            {
                m_VirtualScrollPos += scrollSpeed * Time.deltaTime;
            }

            // 최적화: 펄스 색상 계산을 Update로 이관
            float time = Application.isPlaying ? Time.time : 0f;
            float pulse = 1f;
            if (usePulse)
            {
                pulse += Mathf.Sin(time * pulseSpeed) * pulseIntensity;
            }
            m_FinalFaceColor = color * pulse;

            // 규격 미리 계산
            m_HexWidth = Mathf.Sqrt(3) * hexRadius;
            m_HexHeight = 2f * hexRadius * 0.75f;
        }

        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam, renderPassEvent))
            {
                Draw.Matrix = transform.localToWorldMatrix;
                Draw.ZTest = CompareFunction.Always;

                // 인덱스 계산 최적화
                int centerRow = Mathf.RoundToInt(m_VirtualScrollPos.y / m_HexHeight);
                float xOffsetAtCenter = (centerRow % 2 != 0) ? m_HexWidth * 0.5f : 0f;
                int centerCol = Mathf.RoundToInt((m_VirtualScrollPos.x - xOffsetAtCenter) / m_HexWidth);

                float borderThickness = thickness + outlineAddThickness;

                // 최적화된 이중 루프
                for (int r = -viewDistanceY; r <= viewDistanceY; r++)
                {
                    int currentRow = centerRow + r;
                    // 행 기반 오프셋 미리 계산
                    float currentRowOffset = (currentRow % 2 != 0) ? m_HexWidth * 0.5f : 0f;
                    float basePosY = currentRow * m_HexHeight - m_VirtualScrollPos.y;
                    
                    for (int q = -viewDistanceX; q <= viewDistanceX; q++)
                    {
                        int currentCol = centerCol + q;
                        float posX = currentCol * m_HexWidth + currentRowOffset - m_VirtualScrollPos.x;
                        
                        Vector3 drawPos = new Vector3(posX, basePosY, 0f);

                        // 면 그리기
                        Draw.RegularPolygon(drawPos, m_HexRot, 6, hexRadius, m_FinalFaceColor);

                        // 테두리 그리기
                        if (useOutline)
                        {
                            Draw.RegularPolygonBorder(drawPos, m_HexRot, 6, hexRadius, borderThickness, outlineColor);
                        }
                    }
                }
            }
        }
    }
}
