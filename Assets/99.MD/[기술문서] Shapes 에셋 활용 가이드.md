# [기술문서] Shapes 에셋 활용 가이드 (Immediate Mode)

본 문서는 2025-12-24 육각형 그리드 시스템(HexGridRenderer)을 개발하며 습득한 Unity **Shapes** 에셋의 **Immediate Mode** 활용법과 주요 트러블슈팅 내용을 정리한 문서입니다.

---

## 1. 기본 구조 (Immediate Mode)
Shapes의 Immediate Mode는 `Update` 문에서 매 프레임 그리는 방식이 아닌, 카메라 렌더링 파이프라인에 명령을 주입하는 방식입니다.

### 기본 셋업
```csharp
using Shapes;
using UnityEngine;

// 1. ImmediateModeShapeDrawer 상속
public class MyShapeRenderer : ImmediateModeShapeDrawer 
{
    public override void DrawShapes(Camera cam)
    {
        // 2. Draw.Command를 사용하여 그리기 컨텍스트 생성
        using (Draw.Command(cam))
        {
            // 3. 스타일 설정
            Draw.LineGeometry = LineGeometry.Flat2D;
            
            // 4. 그리기 명령
            Draw.Line(Vector3.zero, Vector3.up, Color.red);
        }
    }
}
```

---

## 2. 렌더링 순서 및 가림 문제 해결 (핵심)
Shapes는 기본적으로 **Transparent(투명)** 큐 이후에 그려질 수 있어, 캐릭터(Opaque)보다 앞에 그려지거나 깊이(Depth) 검사가 제대로 이루어지지 않는 문제가 발생할 수 있습니다.

### 문제 상황
*   바닥에 그린 그리드가 캐릭터 발등 위로 덮어씌워짐.
*   캐릭터 뒤에 있어도 가려지지 않음.

### 해결 방법
**URP RenderPassEvent**와 **ZTest**를 명시적으로 설정해야 합니다.

```csharp
using UnityEngine.Rendering; // CompareFunction
using UnityEngine.Rendering.Universal; // RenderPassEvent

public override void DrawShapes(Camera cam)
{
    // 1. 시점 지정: 불투명 물체(캐릭터)를 다 그린 후, 투명 물체 그리기 전
    using (Draw.Command(cam, RenderPassEvent.BeforeRenderingTransparents))
    {
        // 2. 깊이 테스트 활성화:
        // LessEqual: 이미 그려진 물체(캐릭터)보다 깊이가 같거나 앞에 있을 때만 그림.
        // 즉, 캐릭터 뒤에 있으면 가려짐.
        Draw.ZTest = CompareFunction.LessEqual; 

        // 도형 그리기...
    }
}
```

---

## 3. 도형 그리기 및 외곽선(Outline) 처리

### 면(Fill) vs 선(Border)
`Draw.RegularPolygon` 메서드는 **thickness(두께)** 인자의 유무에 따라 채우기 모드와 선 모드로 나뉩니다.

*   `thickness` 인자 **없음** (혹은 0): **면(Fill)**으로 그려짐.
*   `thickness` 인자 **있음** (> 0): **선(Border)**으로 그려짐.

### 아웃라인(Outline) 구현 팁
깔끔한 외곽선을 구현하려면 **"두 번 그리기"** 기법을 사용합니다. 이때 **Z-Fighting(겹침 깜빡임)**을 방지하기 위해 미세한 오프셋을 줍니다.

```csharp
Vector3 pos = ...;
Quaternion rot = ...;

// 1. 테두리 (검은색, 더 두껍게)
// Y축을 아주 살짝 낮춰서(-0.001f) 바닥에 먼저 깔리게 함
Draw.RegularPolygon(
    pos - new Vector3(0, 0.001f, 0), 
    rot, 
    sides: 6, 
    radius: 1f, 
    thickness: 0.07f, // 메인보다 두껍게
    Color.black
);

// 2. 메인 (형광색, 원래 두께)
Draw.RegularPolygon(
    pos, 
    rot, 
    sides: 6, 
    radius: 1f, 
    thickness: 0.05f, 
    Color.cyan
);
```

---

## 4. 특수 메서드 주의사항: `RegularPolygonBorder`
사용자 정의 혹은 특정 버전의 `Draw.RegularPolygonBorder` 정적 메서드를 사용할 때 매개변수 순서에 주의해야 합니다.

### 흔한 실수
`Draw.RegularPolygonBorder(pos, rot, radius, thickness, angle, color)`

*   **주의**: 이 메서드에는 `sides`(변의 개수) 인자가 없는 경우가 많습니다.
*   **해결**: 전역 설정인 `Draw.RegularPolygonSideCount`를 사용해야 합니다.
*   **실수 사례**: `angle` 자리에 `6`(변의 개수)을 넣으면 육각형이 6라디안(약 343도) 회전되어 그려져 어긋나 보입니다.

```csharp
// 올바른 사용법
Draw.RegularPolygonSideCount = 6; // 전역 설정

Draw.RegularPolygonBorder(
    pos, 
    rot, 
    radius, 
    thickness, 
    0f, // angle (회전각, 라디안)
    color
);
```

---

## 5. HDR Glow 효과 적용
네온 사인처럼 빛나는 효과를 내려면 **HDR Color**를 사용해야 합니다.

1.  변수 선언 시 `[ColorUsage(true, true)]` 속성 사용.
2.  인스펙터에서 Intensity 값을 2~3 이상으로 설정.
3.  URP Volume의 **Bloom** 효과가 켜져 있어야 함.

```csharp
[ColorUsage(true, true)]
public Color glowColor = Color.cyan * 3.0f; // 코드에서 강도 곱하기 가능
```

---

## 6. 육각형 그리드 수학 (Pointy Top 기준)
*   **반지름(Radius)**: 중심에서 꼭짓점까지의 거리.
*   **가로 간격(Width)**: `Mathf.Sqrt(3) * radius`
*   **세로 간격(Height)**: `radius * 1.5f`
*   **오프셋**: 홀수 줄(Row)마다 가로 간격의 절반(`width * 0.5f`)만큼 이동.

---
*작성일: 2025-12-24*
