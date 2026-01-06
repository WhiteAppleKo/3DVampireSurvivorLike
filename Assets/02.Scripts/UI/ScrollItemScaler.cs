using UnityEngine;
using UnityEngine.UI;

public class ScrollItemScaler : MonoBehaviour
{
    public RectTransform scrollContent; // 스크롤 뷰의 Content
    public RectTransform viewPort;
    public float minScale = 0.5f;       // 가장 멀 때 크기
    public float maxScale = 1.2f;       // 중앙에 왔을 때 크기
    public float viewRange = 500f;      // 영향을 받는 거리 범위
    
    public RectTransform SelectedItem { get; private set;}  // 현재 선택된(가장 중앙에 있는) 아이템

    // GC 방지를 위한 캐싱 변수 제거 (TransformPoint 방식 사용 시 불필요)

    void Update()
    {
        if (scrollContent == null) return;
        
        if (viewPort == null) return;

        // Viewport의 기하학적 중심 Y (Local Space)
        float viewportCenterY = viewPort.rect.center.y;

        int childCount = scrollContent.childCount;
        
        float closestDistance = float.MaxValue;
        RectTransform closestChild = null;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = scrollContent.GetChild(i) as RectTransform;
            if (child == null) continue;

            // 1. 자식의 기하학적 중심을 월드 좌표로 변환
            Vector3 childWorldCenter = child.TransformPoint(child.rect.center);
            
            // 2. 월드 중심을 다시 Viewport의 로컬 좌표로 변환
            Vector3 childLocalPos = viewPort.InverseTransformPoint(childWorldCenter);

            // 3. Viewport 중심 Y와 자식 중심 Y의 거리 계산 (세로 스크롤)
            float distance = Mathf.Abs(viewportCenterY - childLocalPos.y);
            
            // [추가] 가장 가까운 아이템 추적
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestChild = child;
            }

            // 4. 스케일 계산
            float scaleAmount = Mathf.Clamp01(1 - (distance / viewRange));
            float finalScale = Mathf.Lerp(minScale, maxScale, scaleAmount);
            child.localScale = new Vector3(finalScale, finalScale, 1f);
        }

        // [추가] 선택된 아이템 갱신
        if (closestChild != null && closestChild != SelectedItem)
        {
            SelectedItem = closestChild;
            // 필요하다면 여기서 선택 변경 이벤트 호출 가능
            // Debug.Log($"Selected Item Changed: {selectedItem.name}");
        }
    }

    // 에디터에서 범위를 눈으로 확인하기 위한 기즈모 (세로 기준)
    void OnDrawGizmosSelected()
    {
        if (scrollContent == null) return;
        if (viewPort == null) return;

        Gizmos.color = Color.yellow;
        // Viewport 중심 시각화
        Vector3 center = viewPort.TransformPoint(viewPort.rect.center);
        Gizmos.DrawWireSphere(center, 30f);

        // 영향 범위 시각화 (상하 범위)
        Gizmos.color = Color.red;
        Vector3 topLimit = viewPort.TransformPoint(viewPort.rect.center + new Vector2(0, viewRange));
        Vector3 bottomLimit = viewPort.TransformPoint(viewPort.rect.center + new Vector2(0, -viewRange));
        
        // 가로선으로 범위 표시
        Gizmos.DrawLine(topLimit + Vector3.left * 100, topLimit + Vector3.right * 100);
        Gizmos.DrawLine(bottomLimit + Vector3.left * 100, bottomLimit + Vector3.right * 100);
    }
}