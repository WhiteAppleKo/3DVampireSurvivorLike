using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BaseStats))]
public class BaseStatsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 현재 그릴 위치를 추적하는 Rect
        Rect currentRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        float space = EditorGUIUtility.standardVerticalSpacing;

        // Foldout (접기/펼치기)
        property.isExpanded = EditorGUI.Foldout(currentRect, property.isExpanded, label);
        currentRect.y += EditorGUIUtility.singleLineHeight + space;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            // 1. usePlayerStats
            DrawProp(ref currentRect, property, "usePlayerStats");

            // 2. HP
            DrawProp(ref currentRect, property, "hp");

            // 3. MaxHP
            DrawProp(ref currentRect, property, "maxHp");

            // 4. Movement Header
            currentRect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(currentRect, "Movement", EditorStyles.boldLabel);
            currentRect.y += currentRect.height + space;

            // 5. Movement Stats
            DrawProp(ref currentRect, property, "moveSpeed");
            DrawProp(ref currentRect, property, "turnSpeed");

            // 6. PlayerStats (체크 해제 시 비활성화)
            SerializedProperty usePlayerStats = property.FindPropertyRelative("usePlayerStats");
            bool enabled = usePlayerStats != null && usePlayerStats.boolValue;

            EditorGUI.BeginDisabledGroup(!enabled);
            DrawProp(ref currentRect, property, "playerStats", true);
            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    private void DrawProp(ref Rect rect, SerializedProperty root, string name, bool includeChildren = false)
    {
        SerializedProperty prop = root.FindPropertyRelative(name);
        if (prop != null)
        {
            rect.height = EditorGUI.GetPropertyHeight(prop, includeChildren);
            EditorGUI.PropertyField(rect, prop, includeChildren);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight; // Foldout 높이
        float space = EditorGUIUtility.standardVerticalSpacing;

        if (property.isExpanded)
        {
            height += space;
            height += GetHeight(property, "usePlayerStats");
            height += GetHeight(property, "hp");
            height += GetHeight(property, "maxHp");
            
            height += EditorGUIUtility.singleLineHeight + space; // Header "Movement"
            
            height += GetHeight(property, "moveSpeed");
            height += GetHeight(property, "turnSpeed");
            
            // PlayerStats는 이제 항상 높이를 차지함
            height += GetHeight(property, "playerStats", true);
        }

        return height;
    }

    private float GetHeight(SerializedProperty root, string name, bool includeChildren = false)
    {
        SerializedProperty prop = root.FindPropertyRelative(name);
        if (prop != null)
        {
            return EditorGUI.GetPropertyHeight(prop, includeChildren) + EditorGUIUtility.standardVerticalSpacing;
        }
        return 0f;
    }
}
