using UnityEngine;
using Shapes;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]public class HexGridRenderer : ImmediateModeShapeDrawer
{
    [Header("Target")]
    public Transform player;

    [Header("Grid Settings")]
    [Range(1, 20)] public int viewDistance = 10;
    public float radius = 1f;
    public float thickness = 0.05f; // 메인 선 두께
    
    [ColorUsage(true, true)]
    public Color glowColor = Color.cyan * 3f;
    
    public float yOffset = 0.01f;

    [Header("Outline Settings")]
    public bool useOutline = true;
    public float outlineAddThickness = 0.02f; // 메인 선보다 얼마나 더 두껍게 할지
    [ColorUsage(true, true)]
    public Color outlineColor = Color.black;  // 테두리 색상
    public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
    public override void DrawShapes(Camera cam)
    {
        if (player == null) return;

        using (Draw.Command(cam, renderPassEvent))
        {
            Draw.ZTest = CompareFunction.LessEqual;
            Draw.LineGeometry = LineGeometry.Flat2D;
            Draw.PolylineGeometry = PolylineGeometry.Flat2D;

            Color finalColor = glowColor;

            float width = Mathf.Sqrt(3) * radius;
            float height = 2f * radius * 0.75f;

            int centerZ = Mathf.RoundToInt(player.position.z / height);
            float xOffset = (centerZ % 2 != 0) ? width * 0.5f : 0f;
            int centerX = Mathf.RoundToInt((player.position.x - xOffset) / width);

            Quaternion hexRot = Quaternion.LookRotation(Vector3.up) * Quaternion.Euler(0, 0, 30f);

            for (int r = -viewDistance; r <= viewDistance; r++)
            {
                int currentZ = centerZ + r;
                float currentRowOffset = (currentZ % 2 != 0) ? width * 0.5f : 0f;

                for (int q = -viewDistance; q <= viewDistance; q++)
                {
                    int currentX = centerX + q;
                    float posX = currentX * width + currentRowOffset;
                    float posZ = currentZ * height;

                    Vector3 drawPos = new Vector3(posX, yOffset, posZ); 
                    
                    // 1. 테두리 먼저 그리기 (thickness가 0보다 크면 자동으로 Border 모드임)
                    

                    // 2. 메인(Glow) 그리기
                    Draw.RegularPolygon(
                        drawPos,
                        hexRot,
                        6,                                // sides
                        radius,                           // radius
                        finalColor                        // color
                    );
                    
                    if (useOutline)
                    {
                        Draw.RegularPolygonSideCount = 6; // Border 메서드는 이 값을 참조함
                        Draw.RegularPolygonBorder(
                            drawPos - new Vector3(0, 0.001f, 0), // Z-Fighting 방지
                            hexRot,
                            radius, 
                            thickness + outlineAddThickness, // 두께
                            0f,                              // angle: hexRot에 회전이 포함되어 있으므로 0f
                            outlineColor
                        );
                    }
                }
            }
        }
    }
}