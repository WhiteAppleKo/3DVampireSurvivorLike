using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexGridGenerator))]
public class HexGridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터(변수들) 그리기
        base.OnInspectorGUI();

        HexGridGenerator generator = (HexGridGenerator)target;

        EditorGUILayout.Space(10); // 여백 추가

        // 가로로 버튼 배치
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate Grid", GUILayout.Height(30)))
        {
            // 실행 취소(Undo) 기능 지원을 위해 기록
            Undo.RegisterFullObjectHierarchyUndo(generator.gameObject, "Generate Grid");
            generator.GenerateGrid();
        }

        if (GUILayout.Button("Clear Grid", GUILayout.Height(30)))
        {
            Undo.RegisterFullObjectHierarchyUndo(generator.gameObject, "Clear Grid");
            generator.ClearGrid();
        }

        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("Shapes 에셋 사용 시: Hex Size와 Shapes 컴포넌트의 Radius를 일치시키세요.", MessageType.Info);
    }
}
