using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using _02.Scripts.Augment.BaseAugment;
using Features.Augment;

public class StatAbilityImporter
{
    private static string m_CsvPath = "Assets/05.Datas/StatAbility/StatAbility.csv";
    private static string m_PureDataPath = "Assets/05.Datas/StatAbility/PureData";
    private static string m_DatabasePath = "Assets/05.Datas/StatAbility/PureDataBaseStatAbility.asset";

    [MenuItem("Tools/Import Stat Abilities (DLV)")]
    public static void ImportCSV()
    {
        if (!Directory.Exists(m_PureDataPath))
        {
            Directory.CreateDirectory(m_PureDataPath);
        }

        PureDataBaseStatAbility database = AssetDatabase.LoadAssetAtPath<PureDataBaseStatAbility>(m_DatabasePath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<PureDataBaseStatAbility>();
            AssetDatabase.CreateAsset(database, m_DatabasePath);
        }
        database.AbilityList.Clear();

        string[] lines = File.ReadAllLines(m_CsvPath, Encoding.UTF8);

        for (int i = 11; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] data = lines[i].Split(',');
            // 최소 8개 데이터(ID ~ Value)가 있어야 함
            if (data.Length < 8) continue;

            string id = data[0].Trim();
            if (string.IsNullOrEmpty(id)) continue;

            string name = data[1].Trim();
            string type = data[2].Trim();
            int iconNum = 0;
            int.TryParse(data[3].Trim(), out iconNum);
            string desc = data[4].Trim();

            StatAbility.e_StatType statType;
            switch (data[5].Trim())
            {
                case "Health": statType = StatAbility.e_StatType.Health; break;
                case "MaxHp": statType = StatAbility.e_StatType.MaxHp; break;
                case "MoveSpeed": statType = StatAbility.e_StatType.MoveSpeed; break;
                default: statType = StatAbility.e_StatType.WrongType; break;
            }

            string valueType = data[6].Trim();
            float valueAmount = 0;
            float.TryParse(data[7].Trim(), out valueAmount);

            // 9번째 데이터(IsTemporary)가 있으면 읽고, 없으면 false
            bool isTemp = false;
            if (data.Length >= 9)
            {
                isTemp = data[8].Trim().ToUpper() == "TRUE";
            }

            string assetPath = $"{m_PureDataPath}/PureDataStatAbility_{id}_{name}.asset";
            PureDataStatAbility pureData = AssetDatabase.LoadAssetAtPath<PureDataStatAbility>(assetPath);
            if (pureData == null)
            {
                pureData = ScriptableObject.CreateInstance<PureDataStatAbility>();
                AssetDatabase.CreateAsset(pureData, assetPath);
            }

            pureData.ID = id;
            pureData.Name = name;
            pureData.Type = type;
            pureData.IconNumber = iconNum;
            pureData.Description = desc;
            pureData.TargetStatType = statType;
            pureData.ValueType = valueType;
            pureData.ValueAmount = valueAmount;
            pureData.IsTemporary = isTemp;

            EditorUtility.SetDirty(pureData);
            database.AbilityList.Add(pureData);
        }

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[DLV Stat Ability Import] 완료! 총 {database.AbilityList.Count}개의 능력을 로드했습니다.");
    }
}