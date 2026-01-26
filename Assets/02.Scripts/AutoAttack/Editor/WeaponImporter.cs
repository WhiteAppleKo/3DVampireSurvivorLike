using System.IO;
using System.Text;
using _02.Scripts.Managers.Spawn;
using UnityEditor;
using UnityEngine;

namespace _02.Scripts.AutoAttack.Editor
{
    public class WeaponImporter
    {
        // CSV 파일 경로 (본인의 경로에 맞게 수정하세요)
        private static string m_CsvPath = "Assets/05.Datas/WeaponData/WeaponDatas.csv";
        private static string m_SoPath = "Assets/05.Datas/WeaponData/WeaponDatabase.asset";
        private static string m_WeaponPrefabPath = "Assets/00.Resources/Weapons/";

        [MenuItem("Tools/Import WeaponDatas")]
        public static void ImportCSV()
        {
            WeaponDatabase asset = AssetDatabase.LoadAssetAtPath<WeaponDatabase>(m_SoPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<WeaponDatabase>();
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
            asset.weaponDatas.Clear();

            // 한글 깨짐 방지를 위해 UTF8 또는 Default 설정
            string[] lines = File.ReadAllLines(m_CsvPath, Encoding.UTF8);

            // 12번째 줄(인덱스 11)부터 데이터 시작
            for (int i = 11; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] data = lines[i].Split(',');
            
                string prefabPath = m_WeaponPrefabPath + $"Weapon_{data[0].Trim()}.prefab";
                GameObject weapon = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                // 데이터 개수가 부족한 줄은 스킵
                if (data.Length < 7) continue;
                WeaponData newWeaponData = ScriptableObject.CreateInstance<WeaponData>();
                newWeaponData.SetSo(
                    data[0].Trim(), 
                    data[1].Trim(),
                    data[2].Trim(),
                    data[3].Trim(),
                    data[4].Trim(),
                    data[5].Trim(),
                    data[6].Trim(),
                    weapon,
                    int.Parse(data[7].Trim()),
                    data[8].Trim()
                    );
            
                newWeaponData.name = $"Weapon_{data[0].Trim()}";
                AssetDatabase.AddObjectToAsset(newWeaponData, asset);
                asset.weaponDatas.Add(newWeaponData);
            }
        

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"임포트 완료! 총 {asset.weaponDatas.Count}개의 무기를 로드했습니다.");
        }
    }
}