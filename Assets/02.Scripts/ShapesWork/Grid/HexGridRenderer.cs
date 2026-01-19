using UnityEngine;
using Shapes;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 플레이어 주변에 육각형 그리드를 렌더링하고, AoE(범위 공격) 판정 및 차징 효과를 관리하는 스크립트입니다.
/// Shapes 라이브러리의 Immediate Mode를 사용하여 매 프레임 그리드를 직접 그립니다.
/// </summary>
[ExecuteAlways]
public class HexGridRenderer : ImmediateModeShapeDrawer
{
    // 그리드 표시 모드: 배경 전체 출력 또는 특정 범위(AoE)만 출력
    public enum GridMode
    {
        Background, 
        AoE
    }

    [Header("모드 설정")]
    public GridMode gridMode = GridMode.Background;

    [Header("추적 대상")]
    [FormerlySerializedAs("player")]
    public Transform target; // 그리드의 중심이 될 대상 (보통 플레이어)

    [Header("그리드 기본 설정")]
    [Range(1, 20)] public int viewDistance = 10; // 배경 모드에서 보여줄 타일 거리
    
    [FormerlySerializedAs("radius")]
    public float hexRadius = 1f; // 육각형 하나의 반지름 (중심에서 꼭짓점까지 거리)
    
    public float thickness = 0.05f; // 육각형 선 두께
    
    [ColorUsage(true, true)]
    [FormerlySerializedAs("glowColor")]
    public Color color = Color.cyan * 3f; // 그리드 기본 색상 (HDR 지원)
    
    public float yOffset = 0.01f; // 바닥과의 Z-Fighting을 방지하기 위한 높이 오프셋

    [Header("AoE(범위) 설정")]
    [Tooltip("0 = 중심만, 1 = 중심 + 주변 1칸(총 7칸)")]
    [FormerlySerializedAs("aoeRadius")]
    public int aoeRange = 1; // AoE 모드에서 표시할 육각형 칸 수

    [Header("외곽선 설정")]
    public bool useOutline = true; // 육각형 테두리 사용 여부
    public float outlineAddThickness = 0.02f; // 기본 선보다 얼마나 더 두껍게 할지
    [ColorUsage(true, true)]
    public Color outlineColor = Color.black; // 테두리 색상
    [ColorUsage(true, true)]
    public Color chargeColor = Color.white * 3f; // 차징 효과 진행 시의 색상

    [Header("렌더링 설정")]
    public CompareFunction zTest = CompareFunction.LessEqual; // 깊이 테스트 설정
    public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingTransparents; // 렌더링 타이밍

    // --- 차징(기 모으기) 로직 관련 데이터 구조 ---
    private class ChargeInstance
    {
        public Vector3 center; // 차징 중심점
        public int range; // 차징 범위(칸 수)
        public float duration; // 총 차징 시간
        public float timer; // 현재 경과 시간
        public LayerMask layer; // 완료 후 감지할 레이어
        public System.Action<List<Collider>> onComplete; // 완료 시 실행할 콜백 함수
    }

    // 현재 진행 중인 모든 차징 효과 리스트
    private List<ChargeInstance> activeCharges = new List<ChargeInstance>();

    /// <summary>
    /// 특정 범위에 차징 효과를 시작하고, 시간이 다 되면 적을 감지하여 콜백을 호출합니다.
    /// </summary>
    public void StartCharge(Vector3 center, int range, float duration, LayerMask layer, System.Action<List<Collider>> onComplete)
    {
        activeCharges.Add(new ChargeInstance 
        { 
            center = center, 
            range = range, 
            duration = duration, 
            timer = 0f, 
            layer = layer,
            onComplete = onComplete 
        });
    }

    private void Update()
    {
        // 에디터 모드가 아닌 실제 게임 실행 중에만 타이머 작동
        //if (!Application.isPlaying) return;

        for (int i = activeCharges.Count - 1; i >= 0; i--)
        {
            var charge = activeCharges[i];
            charge.timer += Time.deltaTime;

            // 차징 시간이 완료되었을 때
            if (charge.timer >= charge.duration)
            {
                // 설정된 범위 내의 타겟들을 스캔
                var targets = ScanTargets(charge.center, charge.range, charge.layer);
                // 등록된 함수 실행 (예: 데미지 입히기)
                charge.onComplete?.Invoke(targets);
                // 리스트에서 제거
                activeCharges.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 특정 위치를 중심으로 육각형 범위 내에 있는 충돌체(Collider)들을 감지합니다.
    /// </summary>
    public List<Collider> ScanTargets(Vector3 centerPos, int range, LayerMask targetLayer)
    {
        List<Collider> validTargets = new List<Collider>();

        // 1단계: 성능을 위해 먼저 구체(Sphere) 범위로 넓게 물리 검색
        float hexWidth = Mathf.Sqrt(3) * hexRadius;
        float searchRadius = (hexWidth * range) + hexWidth; 

        Collider[] hits = Physics.OverlapSphere(centerPos, searchRadius, targetLayer);
        Vector3Int centerCube = WorldToCube(centerPos);

        // 2단계: 감지된 대상들이 "진짜 육각형 칸 거리" 안에 있는지 정밀 검사
        foreach (var hit in hits)
        {
            Vector3Int hitCube = WorldToCube(hit.transform.position);
            if (GetHexDistance(centerCube, hitCube) <= range)
            {
                validTargets.Add(hit);
            }
        }

        return validTargets;
    }

    // Offset 좌표계(지그재그형 바둑판)를 Cube 좌표계(3축 대각선)로 변환
    // 육각형 거리 계산을 위해 필수적인 변환입니다.
    private Vector3Int OffsetToCube(int col, int row)
    {
        var q = col - (row - (row & 1)) / 2;
        var r = row;
        return new Vector3Int(q, -q - r, r);
    }

    // Cube 좌표계를 다시 Offset 좌표계로 변환 (그리기 위치 계산용)
    private Vector2Int CubeToOffset(Vector3Int cube)
    {
        int col = cube.x + (cube.z - (cube.z & 1)) / 2;
        int row = cube.z;
        return new Vector2Int(col, row);
    }

    /// <summary>
    /// 월드 좌표(Vector3)를 육각형 Cube 좌표(Vector3Int)로 변환합니다.
    /// </summary>
    public Vector3Int WorldToCube(Vector3 worldPos)
    {
        float width = Mathf.Sqrt(3) * hexRadius;
        float height = 2f * hexRadius * 0.75f;

        int row = Mathf.RoundToInt(worldPos.z / height);
        float xOffset = (row % 2 != 0) ? width * 0.5f : 0f;
        int col = Mathf.RoundToInt((worldPos.x - xOffset) / width);

        return OffsetToCube(col, row);
    }

    /// <summary>
    /// 두 육각형 좌표 사이의 거리(몇 칸 떨어져 있는지)를 반환합니다.
    /// </summary>
    public int GetHexDistance(Vector3Int a, Vector3Int b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
    }

    /// <summary>
    /// 특정 위치가 현재 타겟(플레이어)의 AoE 범위 안에 있는지 확인합니다.
    /// </summary>
    public bool IsPositionInAoE(Vector3 targetPos)
    {
        if (target == null) return false;

        Vector3Int centerCube = WorldToCube(target.position);
        Vector3Int targetCube = WorldToCube(targetPos);

        return GetHexDistance(centerCube, targetCube) <= aoeRange;
    }

    // Shapes 라이브러리가 매 프레임 도형을 그리기 위해 호출하는 함수
    public override void DrawShapes(Camera cam)
    {
        if (target == null) return;

        // 지정된 렌더링 시점에 그리기 명령 시작
        using (Draw.Command(cam, renderPassEvent))
        {
            Draw.ZTest = zTest;
            Draw.LineGeometry = LineGeometry.Flat2D;
            Draw.PolylineGeometry = PolylineGeometry.Flat2D;

            // 육각형 가로/세로 간격 계산 (Pointy-topped 기준)
            float width = Mathf.Sqrt(3) * hexRadius;
            float height = 2f * hexRadius * 0.75f;

            // 중심 타겟의 인덱스 좌표 계산
            int centerZ = Mathf.RoundToInt(target.position.z / height);
            float xOffset = (centerZ % 2 != 0) ? width * 0.5f : 0f;
            int centerX = Mathf.RoundToInt((target.position.x - xOffset) / width);
            Vector3Int centerCube = OffsetToCube(centerX, centerZ);

            // 육각형이 30도 회전되어야 뾰족한 부분이 위를 향함
            Quaternion hexRot = Quaternion.LookRotation(Vector3.up) * Quaternion.Euler(0, 0, 30f);

            // 모드에 따라 그릴 범위 결정
            int effectiveViewDistance = (gridMode == GridMode.AoE) ? aoeRange : viewDistance;
            int xLoopRange = (gridMode == GridMode.AoE) ? aoeRange + 1 : viewDistance;

            // --- 1. 기본 배경 그리드 또는 AoE 범위 그리기 ---
            for (int r = -effectiveViewDistance; r <= effectiveViewDistance; r++)
            {
                int currentZ = centerZ + r;
                float currentRowOffset = (currentZ % 2 != 0) ? width * 0.5f : 0f;

                for (int q = -xLoopRange; q <= xLoopRange; q++)
                {
                    int currentX = centerX + q;
                    
                    // AoE 모드일 경우 거리 체크하여 범위 밖은 그리지 않음
                    if (gridMode == GridMode.AoE)
                    {
                        Vector3Int currentCube = OffsetToCube(currentX, currentZ);
                        if (GetHexDistance(centerCube, currentCube) > aoeRange) continue;
                    }

                    float posX = currentX * width + currentRowOffset;
                    float posZ = currentZ * height;
                    Vector3 drawPos = new Vector3(posX, yOffset, posZ); 

                    // 꽉 찬 육각형 그리기
                    Draw.RegularPolygon(drawPos, hexRot, 6, hexRadius, color);
                    
                    // 테두리(외곽선) 그리기
                    if (useOutline)
                    {
                        Draw.RegularPolygonSideCount = 6; 
                        Draw.RegularPolygonBorder(
                            drawPos - new Vector3(0, 0.001f, 0), // Z-Fighting 방지
                            hexRot,
                            hexRadius, 
                            thickness + outlineAddThickness,
                            0f,                              
                            outlineColor
                        );
                    }
                }
            }

            // --- 2. 진행 중인 차징 효과 그리기 (테두리에서 중앙으로 채워짐) ---
            if (activeCharges.Count > 0)
            {
                // 차징 효과는 다른 모든 그리드보다 위에 그려지도록 설정
                Draw.ZTest = CompareFunction.Always;

                foreach (var charge in activeCharges)
                {
                    float progress = Mathf.Clamp01(charge.timer / charge.duration);
                    
                    // 진행도에 따라 색상이 진해짐
                    Color cColor = chargeColor; 
                    cColor.a = 0.5f + (progress * 0.5f);

                    // 테두리 두께가 0에서 반지름까지 커지며 중앙을 메움
                    float currentThickness = hexRadius * progress;
                    // 외곽선을 유지하면서 안쪽으로만 두꺼워지도록 반지름 보정
                    float adjustRadius = hexRadius - (currentThickness * 0.5f);

                    Vector3Int cCube = WorldToCube(charge.center);
                    int N = charge.range;

                    // Cube 좌표계 스파이럴 루프: 범위 내의 모든 육각형을 정확히 순회
                    for (int q = -N; q <= N; q++)
                    {
                        int r1 = Mathf.Max(-N, -q - N);
                        int r2 = Mathf.Min(N, -q + N);
                        for (int r = r1; r <= r2; r++)
                        {
                            Vector3Int currentCube = cCube + new Vector3Int(q, -q - r, r);
                            
                            // 다시 그리기용 인덱스로 변환
                            Vector2Int offset = CubeToOffset(currentCube);
                            int col = offset.x;
                            int row = offset.y;
                            
                            float rowOffset = (row % 2 != 0) ? width * 0.5f : 0f;
                            float pX = col * width + rowOffset;
                            float pZ = row * height;
                            
                            // 배경 그리드보다 높은 위치에 출력 (0.02f)
                            Vector3 pPos = new Vector3(pX, yOffset + 0.02f, pZ);

                            // 차징 진행 효과 (점점 두꺼워지는 테두리)
                            Draw.RegularPolygonBorder(pPos, hexRot, 6, adjustRadius, currentThickness, cColor);
                        }
                    }
                }
            }
        }
    }
}