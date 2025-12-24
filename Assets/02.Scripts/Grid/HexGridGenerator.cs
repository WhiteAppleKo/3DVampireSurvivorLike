using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 벌집 모양(육각형) 그리드를 자동으로 생성해주는 도구입니다.
/// </summary>
public class HexGridGenerator : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("생성할 육각형 프리팹 (Shapes의 Disc나 RegularPolygon 권장)")]
    public GameObject hexPrefab;

    [Tooltip("가로 타일 개수")]
    public int gridWidth = 10;
    [Tooltip("세로 타일 개수")]
    public int gridHeight = 10;

    [Tooltip("육각형의 반지름 (중심에서 꼭짓점까지 거리)")]
    public float hexSize = 1f;

    [Tooltip("타일 사이의 추가 간격")]
    public float spacing = 0.05f;

    [Header("Orientation")]
    [Tooltip("체크 시 뾰족한 면이 위로(Pointy), 해제 시 평평한 면이 위로(Flat)")]
    public bool pointyTop = true;

    [SerializeField, HideInInspector]
    private List<GameObject> generatedHexes = new List<GameObject>();

    /// <summary>
    /// 에디터 인스펙터 메뉴에서 'Generate Grid'를 누르면 호출됩니다.
    /// </summary>
    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        ClearGrid();

        if (hexPrefab == null)
        {
            Debug.LogError("Hex Prefab이 설정되지 않았습니다. 프리팹을 할당해주세요.");
            return;
        }

        if (pointyTop)
        {
            GeneratePointyTopGrid();
        }
        else
        {
            GenerateFlatTopGrid();
        }
        
        Debug.Log($"그리드 생성 완료: {generatedHexes.Count} 개의 타일");
    }

    private void GeneratePointyTopGrid()
    {
        float width = Mathf.Sqrt(3) * hexSize;
        float height = 2 * hexSize;
        
        float xDist = width + spacing;
        float yDist = (height * 0.75f) + (spacing * Mathf.Sqrt(3)/2f);

        // 중앙 정렬을 위한 오프셋 계산
        // 전체 너비 = (기본 너비) + (줄이 2줄 이상일 때 엇갈리며 튀어나오는 반 칸 너비)
        float totalWidth = (gridWidth - 1) * xDist + (gridHeight > 1 ? xDist * 0.5f : 0);
        float totalHeight = (gridHeight - 1) * yDist;

        float xOffset = totalWidth * 0.5f;
        float zOffset = totalHeight * 0.5f;

        for (int z = 0; z < gridHeight; z++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                float xPos = x * xDist;
                // 홀수 줄은 X축으로 반 칸 이동
                if (z % 2 != 0) xPos += xDist * 0.5f;

                float zPos = z * -yDist;
                
                // 오프셋 적용 (중앙 정렬)
                CreateHex(new Vector3(xPos - xOffset, 0, zPos + zOffset), x, z);
            }
        }
    }

    private void GenerateFlatTopGrid()
    {
        float width = 2 * hexSize;
        float height = Mathf.Sqrt(3) * hexSize;

        float xDist = (width * 0.75f) + (spacing * Mathf.Sqrt(3)/2f);
        float yDist = height + spacing;

        // 중앙 정렬을 위한 오프셋 계산
        float totalWidth = (gridWidth - 1) * xDist;
        // 전체 높이 = (기본 높이) + (열이 2열 이상일 때 엇갈리며 튀어나오는 반 칸 높이)
        float totalHeight = (gridHeight - 1) * yDist + (gridWidth > 1 ? yDist * 0.5f : 0);

        float xOffset = totalWidth * 0.5f;
        float zOffset = totalHeight * 0.5f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                float xPos = x * xDist;
                float zPos = z * -yDist;
                // 홀수 열은 Z축으로 반 칸 이동 (아래로)
                if (x % 2 != 0) zPos -= yDist * 0.5f;

                // 오프셋 적용 (중앙 정렬)
                CreateHex(new Vector3(xPos - xOffset, 0, zPos + zOffset), x, z);
            }
        }
    }

    private void CreateHex(Vector3 position, int x, int z)
    {
        GameObject hex = Instantiate(hexPrefab, transform);
        hex.transform.localPosition = position;
        hex.name = $"Hex_{x}_{z}";
        generatedHexes.Add(hex);
    }

    /// <summary>
    /// 생성된 모든 타일을 제거합니다.
    /// </summary>
    [ContextMenu("Clear Grid")]
    public void ClearGrid()
    {
        foreach (var hex in generatedHexes)
        {
            if (hex != null)
            {
                if (Application.isPlaying) Destroy(hex);
                else DestroyImmediate(hex);
            }
        }
        generatedHexes.Clear();

        // 리스트 외에 자식으로 남아있는 오브젝트들도 청소
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++) children.Add(transform.GetChild(i).gameObject);
        foreach (var child in children)
        {
            if (Application.isPlaying) Destroy(child);
            else DestroyImmediate(child);
        }
    }
}