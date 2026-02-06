using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using _02.Scripts.Augment.BaseAugment;
using Features.Augment;

public class WeaponAbilityImporter
{
    private static string m_CsvPath = "Assets/05.Datas/WeaponAbility/WeaponAbility.csv";
    private static string m_PureDataPath = "Assets/05.Datas/WeaponAbility/PureData";
    private static string m_DatabasePath = "Assets/05.Datas/WeaponAbility/PureDataBaseWeaponAbility.asset";

    [MenuItem("Tools/Import Weapon Abilities (DLV)")]
    public static void ImportCSV()
    {
        if (!Directory.Exists(m_PureDataPath))
        {
            Directory.CreateDirectory(m_PureDataPath);
        }

        PureDataBaseWeaponAbility database = AssetDatabase.LoadAssetAtPath<PureDataBaseWeaponAbility>(m_DatabasePath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<PureDataBaseWeaponAbility>();
            AssetDatabase.CreateAsset(database, m_DatabasePath);
        }
        database.AbilityList.Clear();

        string[] lines = File.ReadAllLines(m_CsvPath, Encoding.UTF8);

        for (int i = 11; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] data = lines[i].Split(',');
            if (data.Length < 8) continue;

            string id = data[0].Trim();
            if (string.IsNullOrEmpty(id)) continue;

            string name = data[1].Trim();
            string type = data[2].Trim();
            int iconNum = int.Parse(data[3].Trim());
            string desc = data[4].Trim();

            WeaponAbility.e_WeaponStatType statType;
            switch (data[5].Trim())
            {
                case "AttackDelay": statType = WeaponAbility.e_WeaponStatType.AttackDelay; break;
                case "Damage": statType = WeaponAbility.e_WeaponStatType.Damage; break;
                case "AoE": statType = WeaponAbility.e_WeaponStatType.AoE; break;
                default: statType = WeaponAbility.e_WeaponStatType.WrongType; break;
            }

            string valueType = data[6].Trim();
            float valueAmount = float.Parse(data[7].Trim());

            string assetPath = $"{m_PureDataPath}/PureDataWeaponAbility_{id}_{name}.asset";
            PureDataWeaponAbility pureData = AssetDatabase.LoadAssetAtPath<PureDataWeaponAbility>(assetPath);
            if (pureData == null)
            {
                pureData = ScriptableObject.CreateInstance<PureDataWeaponAbility>();
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

            EditorUtility.SetDirty(pureData);
            database.AbilityList.Add(pureData);
        }

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[DLV Weapon Ability Import] 완료! 총 {database.AbilityList.Count}개의 능력을 로드했습니다.");
    }
}