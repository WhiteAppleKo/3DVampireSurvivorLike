using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using _02.Scripts.Augment.BaseAugment;
using _02.Scripts.Managers.Spawn;
using _02.Scripts.Managers.Stage;

public class StageImporter
{
    // CSV 파일 경로 (본인의 경로에 맞게 수정하세요)
    private static string m_CsvPath = "Assets/05.Datas/StageData/StageData.csv";
    private static string m_SoPath = "Assets/05.Datas/StageData/StageDataBase.asset";
    private static string m_MonsterDataBasePath = "Assets/05.Datas/MonsterData/MonsterDatabase.asset";

    [MenuItem("Tools/Import StageDatas")]
    public static void ImportCSV()
    {
        StageDataBase asset = AssetDatabase.LoadAssetAtPath<StageDataBase>(m_SoPath);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<StageDataBase>();
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
        asset.stageDatas.Clear();
        MonsterDatabase monsterDB =
            AssetDatabase.LoadAssetAtPath<MonsterDatabase>(m_MonsterDataBasePath); 

        // 한글 깨짐 방지를 위해 UTF8 또는 Default 설정
        string[] lines = File.ReadAllLines(m_CsvPath, Encoding.UTF8);

        // 12번째 줄(인덱스 11)부터 데이터 시작
        for (int i = 11; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] data = lines[i].Split(',');
            
            var monsterDatas = data[1].Split(';');
            List<MonsterData> monsterDataList = new List<MonsterData>();
            
            
            MonsterData foundMonster = null;
            for (int j = 0; j < monsterDatas.Length; j++)
            {
                string targetID = monsterDatas[j].Replace("\"", "").Trim();

                
                foreach (var monster in monsterDB.monsterDatas)
                {
                    if (monster.monsterID == targetID)
                    {
                        foundMonster = monster;
                    }
                }

                if (foundMonster != null)
                {
                    monsterDataList.Add(foundMonster);
                    foundMonster = null;
                }
                else
                {
                    Debug.LogWarning($"[StageImporter] ID가 {targetID}인 몬스터를 찾을 수 없습니다.");
                }
            }
            
            string bossID = data[2].Replace("\"", "").Trim();

            foreach (var monster in monsterDB.monsterDatas)
            {
                
                if (monster.monsterID == bossID)
                {
                    foundMonster = monster;
                }
            }
            
            // 데이터 개수가 부족한 줄은 스킵
            if (data.Length < 4) continue;
            StageData newStageData = ScriptableObject.CreateInstance<StageData>();
            newStageData.SetSo(
                data[0].Trim(), 
                monsterDataList,
                foundMonster,
                data[3].Trim()
                );
            
            newStageData.name = $"Stage_{data[0].Trim()}";
            AssetDatabase.AddObjectToAsset(newStageData, asset);
            asset.stageDatas.Add(newStageData);
        }
        

        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"임포트 완료! 총 {asset.stageDatas.Count}개의 스테이지를 로드했습니다.");
    }
}