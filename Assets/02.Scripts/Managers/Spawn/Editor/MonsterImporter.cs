using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using _02.Scripts.Augment.BaseAugment;
using _02.Scripts.Managers.Spawn;

public class MonsterImporter
{
    // CSV 파일 경로 (본인의 경로에 맞게 수정하세요)
    private static string m_CsvPath = "Assets/05.Datas/MonsterData/MonsterDatas.csv";
    private static string m_SoPath = "Assets/05.Datas/MonsterData/MonsterDatabase.asset";
    private static string m_MonsterPrefabPath = "Assets/00.Resources/Monsters/";

    [MenuItem("Tools/Import MonsterDatas")]
    public static void ImportCSV()
    {
        MonsterDatabase asset = AssetDatabase.LoadAssetAtPath<MonsterDatabase>(m_SoPath);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<MonsterDatabase>();
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
        asset.monsterDatas.Clear();

        // 한글 깨짐 방지를 위해 UTF8 또는 Default 설정
        string[] lines = File.ReadAllLines(m_CsvPath, Encoding.UTF8);

        // 12번째 줄(인덱스 11)부터 데이터 시작
        for (int i = 11; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] data = lines[i].Split(',');
            
            string prefabPath = m_MonsterPrefabPath + $"Monster_{data[0].Trim()}.prefab";
            Debug.Log(prefabPath);
            GameObject monster = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            // 데이터 개수가 부족한 줄은 스킵
            if (data.Length < 2) continue;
            MonsterData newMonsterData = ScriptableObject.CreateInstance<MonsterData>();
            newMonsterData.SetSo(
                data[0].Trim(), 
                data[1].Trim(),
                data[2].Trim(),
                data[3].Trim(),
                data[4].Trim(),
                data[5].Trim(),
                data[6].Trim(),
                data[7].Trim(),
                monster);
            
            newMonsterData.name = $"Monster_{data[0].Trim()}";
            AssetDatabase.AddObjectToAsset(newMonsterData, asset);
            asset.monsterDatas.Add(newMonsterData);
        }
        

        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"임포트 완료! 총 {asset.monsterDatas.Count}개의 몬스터를 로드했습니다.");
    }
}