using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using Features.Enemy;
using System.Collections.Generic;

public class MonsterImporter
{
    // CSV 파일 경로
    private static string m_CsvPath = "Assets/05.Datas/MonsterData/MonsterDatas.csv";
    
    // DLV 데이터 저장 경로 (새로운 경로)
    private static string m_PureDataPath = "Assets/05.Datas/MonsterData/PureData"; 
    private static string m_DatabasePath = "Assets/05.Datas/MonsterData/PureDataBaseEnemy.asset";
    
    // 프리팹 경로
    private static string m_MonsterPrefabPath = "Assets/00.Resources/Monsters/";

    [MenuItem("Tools/Import MonsterDatas (DLV)")]
    public static void ImportCSV()
    {
        // 0. 폴더 확인 및 생성
        if (!Directory.Exists(m_PureDataPath))
        {
            Directory.CreateDirectory(m_PureDataPath);
        }

        // 1. PureDataBase 로드 또는 생성
        PureDataBaseEnemy database = AssetDatabase.LoadAssetAtPath<PureDataBaseEnemy>(m_DatabasePath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<PureDataBaseEnemy>();
            AssetDatabase.CreateAsset(database, m_DatabasePath);
        }
        
        // 기존 리스트 초기화 (새로 채우기 위함)
        database.MonsterList.Clear();

        // 2. CSV 읽기
        string[] lines = File.ReadAllLines(m_CsvPath, Encoding.UTF8);

        // 12번째 줄(인덱스 11)부터 데이터 시작
        for (int i = 11; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] data = lines[i].Split(',');
            // 최소 데이터 길이 확인 (ID, Name, HP, Speed, Exp 등등...)
            if (data.Length < 8) continue;

            // 데이터 파싱 (ID는 문자열로 처리하여 앞자리 0 보존)
            string id = data[0].Trim();
            if (string.IsNullOrEmpty(id)) 
            {
                continue;
            }

            string monsterName = data[1].Trim(); // 이름
            // data[2] : desc (사용 안 함)
            
            if (!int.TryParse(data[3].Trim(), out int hp)) hp = 10;
            if (!float.TryParse(data[4].Trim(), out float speed)) speed = 3.0f;
            // data[5] : atk (사용 안 함 - 접촉 데미지?)
            // data[6] : atkDelay (사용 안 함)
            if (!int.TryParse(data[7].Trim(), out int exp)) exp = 5;
            
            // 3. PureDataEnemy 에셋 생성 또는 로드
            string assetPath = $"{m_PureDataPath}/PureDataEnemy_{id}_{monsterName}.asset";
            PureDataEnemy pureData = AssetDatabase.LoadAssetAtPath<PureDataEnemy>(assetPath);
            
            if (pureData == null)
            {
                pureData = ScriptableObject.CreateInstance<PureDataEnemy>();
                AssetDatabase.CreateAsset(pureData, assetPath);
            }

            // 4. 데이터 주입
            pureData.ID = id;
            pureData.MonsterName = monsterName;
            
            pureData.BaseMaxHp = hp;
            pureData.BaseMoveSpeed = speed;
            pureData.BaseExpAmount = exp;
            
            // 기타 기본값 설정 (CSV에 없는 값)
            pureData.BaseTurnSpeed = 5.0f; 
            pureData.MinimumDistance = 0.01f;

            // [추가] SerializedObject를 사용하여 TargetLayer를 확실하게 저장
            SerializedObject pureDataSo = new SerializedObject(pureData);
            SerializedProperty targetLayerProp = pureDataSo.FindProperty("TargetLayer");
            if (targetLayerProp != null)
            {
                targetLayerProp.intValue = LayerMask.GetMask("Player");
                pureDataSo.ApplyModifiedProperties();
            }

            // 6. 프리팹 자동 연결 및 할당
            string prefabPath = m_MonsterPrefabPath + $"Monster_{id}.prefab"; 
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                pureData.Prefab = prefab; // 필드 할당
                
                // 프리팹 내부 수정
                string prefabAssetPath = AssetDatabase.GetAssetPath(prefab);
                using (var editScope = new PrefabUtility.EditPrefabContentsScope(prefabAssetPath))
                {
                    GameObject root = editScope.prefabContentsRoot;
                    
                    // 1. 프리팹 전체 계층에서 누락된 스크립트 제거 (자식 오브젝트 포함)
                    var allTransforms = root.GetComponentsInChildren<Transform>(true);
                    foreach (var t in allTransforms)
                    {
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);

                        // [무결성 복구] 무기 스크립트들이 요구하는 컴포넌트가 없어서 저장이 안 되는 문제 해결
                        // BodyAttack -> BoxCollider 필요
                        if (t.GetComponent<_02.Scripts.AutoAttack.BodyAttack.BodyAttack>() != null && t.GetComponent<BoxCollider>() == null)
                        {
                            t.gameObject.AddComponent<BoxCollider>().isTrigger = true;
                        }
                        // AoEWeapon -> SphereCollider 필요
                        if (t.GetComponent<_02.Scripts.AutoAttack.AoE.AoEWeapon>() != null && t.GetComponent<SphereCollider>() == null)
                        {
                            t.gameObject.AddComponent<SphereCollider>().isTrigger = true;
                        }

                        // [무결성 복구] 모든 Weapon은 IWeaponVisualizer(구체 클래스: WeaponVisualizer)를 요구함
                        if (t.GetComponent<_02.Scripts.AutoAttack.Weapon>() != null && t.GetComponent<Features.Weapon.WeaponVisualizer>() == null)
                        {
                            t.gameObject.AddComponent<Features.Weapon.WeaponVisualizer>();
                        }
                    }

                    // 2. 비주얼 및 로직 컴포넌트 자동 추가
                    var visuals = root.GetComponent<EnemyVisualizer>();
                    if (visuals == null)
                    {
                        visuals = root.AddComponent<EnemyVisualizer>();
                    }

                    var logic = root.GetComponent<EnemyLogicSystem>();
                    if (logic == null)
                    {
                        logic = root.AddComponent<EnemyLogicSystem>();
                    }

                    // 3. 몬스터 로직 PureData 할당
                    if (logic != null)
                    {
                        SerializedObject so = new SerializedObject(logic);
                        SerializedProperty dataProp = so.FindProperty("pureData");
                        if (dataProp != null)
                        {
                            dataProp.objectReferenceValue = pureData;
                            so.ApplyModifiedProperties();
                        }
                    }

                    // 4. [중요] 하위 무기들 PureData 할당 및 동기화 (DLV)
                    // 몬스터가 자식으로 무기를 들고 있는 경우, 각 무기에도 PureData를 꽂아줘야 함.
                    var childWeapons = root.GetComponentsInChildren<_02.Scripts.AutoAttack.Weapon>(true);
                    foreach (var weapon in childWeapons)
                    {
                        // 무기 ID 추출 시도 (프리팹 이름이나 기존 ID 참조)
                        // 보통 몬스터 무기 ID는 05xxxxxx 형태
                        // 여기서는 무기 프리팹 이름에서 ID를 유추하거나 이미 설정된 ID를 기반으로 DB 조회
                        string weaponID = weapon.name.Replace("Weapon_", ""); 
                        
                        // DataHub의 무기 데이터베이스 로드 (에디터 전용 로드)
                        string weaponDbPath = "Assets/05.Datas/WeaponData/PureDataBaseWeapon.asset";
                        var weaponDb = AssetDatabase.LoadAssetAtPath<Features.Weapon.PureDataBaseWeapon>(weaponDbPath);
                        
                        if (weaponDb != null)
                        {
                            var weaponPureData = weaponDb.GetData(weaponID);
                            if (weaponPureData != null)
                            {
                                SerializedObject wSo = new SerializedObject(weapon);
                                SerializedProperty wDataProp = wSo.FindProperty("pureData");
                                if (wDataProp != null)
                                {
                                    wDataProp.objectReferenceValue = weaponPureData;
                                    wSo.ApplyModifiedProperties();
                                    Debug.Log($"[MonsterImporter] 몬스터 '{root.name}'의 하위 무기 '{weapon.name}'에 PureData 할당 완료.");
                                }
                            }
                            else
                            {
                                Debug.LogWarning($"[MonsterImporter] 무기 ID '{weaponID}'를 DB에서 찾을 수 없습니다. (몬스터: {root.name})");
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"[Importer] 프리팹을 찾을 수 없습니다: {prefabPath}");
            }

            EditorUtility.SetDirty(pureData);
            
            // 5. 데이터베이스에 등록
            if (!database.MonsterList.Contains(pureData))
            {
                database.MonsterList.Add(pureData);
            }
        }

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[DLV Import] 완료! 총 {database.MonsterList.Count}개의 몬스터 데이터 생성 및 갱신.");
    }
}