using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using Features.Enemy;
using Features.Stage;

public class StageImporter
{
    private static string m_CsvPath = "Assets/05.Datas/StageData/StageData.csv";
    private static string m_PureDataPath = "Assets/05.Datas/StageData/PureData";
    private static string m_DatabasePath = "Assets/05.Datas/StageData/PureDataBaseStage.asset";
    private static string m_MonsterDatabasePath = "Assets/05.Datas/MonsterData/PureDataBaseEnemy.asset";

    [MenuItem("Tools/Import StageDatas (DLV)")]
    public static void ImportCSV()
    {
        if (!Directory.Exists(m_PureDataPath))
        {
            Directory.CreateDirectory(m_PureDataPath);
        }

        PureDataBaseStage database = AssetDatabase.LoadAssetAtPath<PureDataBaseStage>(m_DatabasePath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<PureDataBaseStage>();
            AssetDatabase.CreateAsset(database, m_DatabasePath);
        }
        database.StageList.Clear();

        PureDataBaseEnemy monsterDB = AssetDatabase.LoadAssetAtPath<PureDataBaseEnemy>(m_MonsterDatabasePath);
        if (monsterDB == null)
        {
            Debug.LogError("[StageImporter] PureDataBaseEnemy를 찾을 수 없습니다. 몬스터 임포트를 먼저 진행하세요.");
            return;
        }

        string[] lines = File.ReadAllLines(m_CsvPath, Encoding.UTF8);

        for (int i = 11; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] data = lines[i].Split(',');
            if (data.Length < 4) continue;

            string id = data[0].Trim();
            if (string.IsNullOrEmpty(id)) continue;

            // Monster List Parsing (ID1;ID2;ID3...)
            var monsterIds = data[1].Replace("\"", "").Split(';');
            List<PureDataEnemy> monsterList = new List<PureDataEnemy>();
            foreach (var mId in monsterIds)
            {
                var trimmedId = mId.Trim();
                var found = monsterDB.GetData(trimmedId);
                if (found != null) monsterList.Add(found);
                else Debug.LogWarning($"[StageImporter] 몬스터 ID {trimmedId}를 찾을 수 없습니다.");
            }

            // Boss Monster Parsing
            string bossId = data[2].Replace("\"", "").Trim();
            PureDataEnemy bossMonster = monsterDB.GetData(bossId);

            bool isBossStage = data[3].Trim().ToUpper() == "TRUE";

            string assetPath = $"{m_PureDataPath}/PureDataStage_{id}.asset";
            PureDataStage pureData = AssetDatabase.LoadAssetAtPath<PureDataStage>(assetPath);
            if (pureData == null)
            {
                pureData = ScriptableObject.CreateInstance<PureDataStage>();
                AssetDatabase.CreateAsset(pureData, assetPath);
            }

            pureData.ID = id;
            
            // [DLV Refactoring] 기존 MonsterList를 기본 웨이브로 변환
            var defaultWave = new WaveData
            {
                startTime = 0,
                endTime = 9999,
                monsters = monsterList,
                spawnInterval = 1.0f, // 기본값
                maxCount = 50 // 기본값
            };
            
            pureData.Waves = new List<WaveData> { defaultWave };
            pureData.BossMonster = bossMonster;
            pureData.IsBossStage = isBossStage;

            EditorUtility.SetDirty(pureData);
            database.StageList.Add(pureData);
        }

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[DLV Stage Import] 완료! 총 {database.StageList.Count}개의 스테이지를 로드했습니다.");
    }
}