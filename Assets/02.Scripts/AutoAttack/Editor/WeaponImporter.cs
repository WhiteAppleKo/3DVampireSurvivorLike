using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Features.Weapon;

namespace _02.Scripts.AutoAttack.Editor
{
    public class WeaponImporter
    {
        private static string m_CsvPath = "Assets/05.Datas/WeaponData/WeaponDatas.csv";
        private static string m_PureDataPath = "Assets/05.Datas/WeaponData/PureData";
        private static string m_DatabasePath = "Assets/05.Datas/WeaponData/PureDataBaseWeapon.asset";
        private static string m_WeaponPrefabPath = "Assets/00.Resources/Weapons/";

        [MenuItem("Tools/Import WeaponDatas (DLV)")]
        public static void ImportCSV()
        {
            if (!Directory.Exists(m_PureDataPath))
            {
                Directory.CreateDirectory(m_PureDataPath);
            }

            PureDataBaseWeapon database = AssetDatabase.LoadAssetAtPath<PureDataBaseWeapon>(m_DatabasePath);
            if (database == null)
            {
                database = ScriptableObject.CreateInstance<PureDataBaseWeapon>();
                AssetDatabase.CreateAsset(database, m_DatabasePath);
            }
            database.WeaponList.Clear();

            string[] lines = File.ReadAllLines(m_CsvPath, Encoding.UTF8);

            for (int i = 11; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] data = lines[i].Split(',');
                if (data.Length < 9) continue;

                string id = data[0].Trim();
                if (string.IsNullOrEmpty(id)) continue;

                string weaponName = data[1].Trim();
                string weaponType = data[2].Trim();
                float delay = float.Parse(data[3].Trim());
                int damage = int.Parse(data[4].Trim());
                float range = float.Parse(data[5].Trim());
                int projCount = int.Parse(data[6].Trim());
                int iconNum = int.Parse(data[7].Trim());
                string desc = data[8].Trim();

                string assetPath = $"{m_PureDataPath}/PureDataWeapon_{id}_{weaponName}.asset";
                PureDataWeapon pureData = AssetDatabase.LoadAssetAtPath<PureDataWeapon>(assetPath);
                if (pureData == null)
                {
                    pureData = ScriptableObject.CreateInstance<PureDataWeapon>();
                    AssetDatabase.CreateAsset(pureData, assetPath);
                }

                pureData.ID = id;
                pureData.Name = weaponName;
                pureData.Type = weaponType;
                pureData.AttackDelay = delay;
                pureData.Damage = damage;
                pureData.EffectRange = range;
                pureData.ProjectileCount = projCount;
                pureData.IconNumber = iconNum;
                pureData.Description = desc;

                string prefabPath = m_WeaponPrefabPath + $"Weapon_{id}.prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab != null)
                {
                    pureData.Prefab = prefab;
                    PatchPrefab(prefab, pureData);
                }

                EditorUtility.SetDirty(pureData);
                database.WeaponList.Add(pureData);
            }

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[DLV Weapon Import] 완료! 총 {database.WeaponList.Count}개의 무기를 로드했습니다.");
        }

        private static void PatchPrefab(GameObject prefab, PureDataWeapon pureData)
        {
            string prefabAssetPath = AssetDatabase.GetAssetPath(prefab);
            using (var editScope = new PrefabUtility.EditPrefabContentsScope(prefabAssetPath))
            {
                GameObject root = editScope.prefabContentsRoot;

                // 1. Missing Scripts cleanup
                var allTransforms = root.GetComponentsInChildren<Transform>(true);
                foreach (var t in allTransforms)
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
                }

                // 2. Add WeaponVisualizer if missing
                var visualizer = root.GetComponent<WeaponVisualizer>();
                if (visualizer == null)
                {
                    visualizer = root.AddComponent<WeaponVisualizer>();
                }

                // 3. Link Logic System if exists
                var weaponLogic = root.GetComponent<Weapon>();
                if (weaponLogic != null)
                {
                    SerializedObject so = new SerializedObject(weaponLogic);
                    SerializedProperty prop = so.FindProperty("pureData");
                    if (prop != null)
                    {
                        // Note: Weapon.cs might need updating to use the new PureDataWeapon type
                    }
                }
            }
        }
    }
}