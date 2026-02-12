using UnityEngine;
using Shapes;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _02.Scripts.UI.Title
{
    /// <summary>
    /// [V] Title 씬 배경을 위한 육각형 그리드 렌더러.
    /// 인게임 HexGridRenderer와 동일한 로직을 사용하여 겹침 없는 무한 그리드를 구현합니다.
    /// </summary>
    [ExecuteAlways]
    public class TitleHexBackgroundRenderer : ImmediateModeShapeDrawer
    {
        [Header("Grid Settings")]
        public float hexRadius = 1f;
        public float thickness = 0.05f;
        [Range(5, 50)] public int viewDistanceX = 20;
        [Range(5, 50)] public int viewDistanceY = 15;

        [Header("Color Settings (Sync with In-Game)")]
        [ColorUsage(true, true)] public Color color = Color.cyan * 0.5f; 
        public bool useOutline = true;
        [ColorUsage(true, true)] public Color outlineColor = Color.black; 
        public float outlineAddThickness = 0.02f;

        [Header("Animation Settings")]
        public Vector2 scrollSpeed = new Vector2(0.2f, 0.1f);
        public bool usePulse = true; // Pulsing 효과 사용 여부
        public float pulseSpeed = 1.5f;
        public float pulseIntensity = 0.3f;

        [Header("Rendering Settings")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        private Vector2 m_VirtualScrollPos;

        private void Update()
        {
            if (Application.isPlaying)
            {
                m_VirtualScrollPos += scrollSpeed * Time.deltaTime;
            }
        }

        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam, renderPassEvent))
            {
                // 오브젝트의 Transform 기준 렌더링
                Draw.Matrix = transform.localToWorldMatrix;
                Draw.ZTest = CompareFunction.Always;
                
                // 인게임과 동일한 규격 계산
                float width = Mathf.Sqrt(3) * hexRadius;
                float height = 2f * hexRadius * 0.75f;

                // 애니메이션
                float time = Application.isPlaying ? Time.time : 0f;
                float pulse = 1f;
                if (usePulse)
                {
                    pulse += Mathf.Sin(time * pulseSpeed) * pulseIntensity;
                }
                Color finalFaceColor = color * pulse;

                // 인게임 방식의 중심점 기반 인덱스 계산
                // 가상의 스크롤 위치를 타겟 위치로 간주합니다.
                int centerRow = Mathf.RoundToInt(m_VirtualScrollPos.y / height);
                float xOffsetAtCenter = (centerRow % 2 != 0) ? width * 0.5f : 0f;
                int centerCol = Mathf.RoundToInt((m_VirtualScrollPos.x - xOffsetAtCenter) / width);

                Quaternion hexRot = Quaternion.Euler(0, 0, 30f);

                // 중복 없이 화면 범위만큼만 순회하여 오버드로우 해결
                for (int r = -viewDistanceY; r <= viewDistanceY; r++)
                {
                    int currentRow = centerRow + r;
                    float currentRowOffset = (currentRow % 2 != 0) ? width * 0.5f : 0f;
                    
                    for (int q = -viewDistanceX; q <= viewDistanceX; q++)
                    {
                        int currentCol = centerCol + q;

                        // 최종 로컬 좌표 계산
                        float posX = currentCol * width + currentRowOffset - m_VirtualScrollPos.x;
                        float posY = currentRow * height - m_VirtualScrollPos.y;

                        Vector3 drawPos = new Vector3(posX, posY, 0f);

                        // 1. 육각형 면
                        Draw.RegularPolygon(drawPos, hexRot, 6, hexRadius, finalFaceColor);

                        // 2. 육각형 테두리
                        if (useOutline)
                        {
                            Draw.RegularPolygonBorder(
                                drawPos, 
                                hexRot, 
                                6, 
                                hexRadius, 
                                thickness + outlineAddThickness, 
                                outlineColor
                            );
                        }
                    }
                }
            }
        }
    }
}
