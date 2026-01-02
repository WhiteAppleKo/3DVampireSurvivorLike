using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using _02.Scripts.Augment.BaseAugment;

public class AbilityImporter
{
    // CSV 파일 경로 (본인의 경로에 맞게 수정하세요)
    private static string m_CsvPath = "Assets/05.Datas/StatAbility.csv";
    private static string m_SoPath = "Assets/05.Datas/AbilityDatabase.asset";

    [MenuItem("Tools/Import Abilities")]
    public static void ImportCSV()
    {
        StatAbilityDatabase asset = AssetDatabase.LoadAssetAtPath<StatAbilityDatabase>(m_SoPath);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<StatAbilityDatabase>();
            AssetDatabase.CreateAsset(asset, m_SoPath);
        }
        Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(m_SoPath);
        foreach (Object subAsset in subAssets)
        {
            if (subAsset != asset)
            {
                AssetDatabase.RemoveObjectFromAsset(subAsset);
                Object.DestroyImmediate(subAsset, true);
            }
        }
        asset.statAbilities.Clear();

        // 한글 깨짐 방지를 위해 UTF8 또는 Default 설정
        string[] lines = File.ReadAllLines(m_CsvPath, Encoding.UTF8);

        // 12번째 줄(인덱스 11)부터 데이터 시작
        for (int i = 11; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] data = lines[i].Split(',');

            // 데이터 개수가 부족한 줄은 스킵
            if (data.Length < 8) continue;

            StatAbility.e_StatType statType;
            switch (data[5].Trim())
            {
                case "Health":
                    statType = StatAbility.e_StatType.Health;
                    break;
                case "MaxHp" :
                    statType = StatAbility.e_StatType.MaxHp;
                    break;
                case "MoveSpeed":
                    statType = StatAbility.e_StatType.MoveSpeed;
                    break;
                default: statType = StatAbility.e_StatType.WrongType;
                    break;
            }
            
            
            
            StatAbility newAbility = ScriptableObject.CreateInstance<StatAbility>();

            newAbility.SetSo(data[0].Trim(),
                data[1].Trim(),
                data[2].Trim(),
                int.Parse(data[3].Trim()),
                data[4].Trim(),
                statType,
                data[6].Trim(),
                data[7].Trim());
            newAbility.name = $"{data[0].Trim()}_{data[1].Trim()}";
            AssetDatabase.AddObjectToAsset(newAbility, asset);
            asset.statAbilities.Add(newAbility);
        }
        

        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"임포트 완료! 총 {asset.statAbilities.Count}개의 능력을 로드했습니다.");
    }
}