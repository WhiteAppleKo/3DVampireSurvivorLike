using System.IO;
using UnityEngine;

namespace _02.Scripts.Managers.Save
{
    /// <summary>
    /// Json 형식을 사용하는 파일 저장 시스템
    /// </summary>
    public class JsonSaveSystem : ISaveSystem
    {
        private string GetFullPath(string fileName) => Path.Combine(Application.persistentDataPath, fileName);

        public void Save<T>(string fileName, T data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(GetFullPath(fileName), json);
        }

        public T Load<T>(string fileName)
        {
            string path = GetFullPath(fileName);
            if (!File.Exists(path)) return default;

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        public void Delete(string fileName)
        {
            string path = GetFullPath(fileName);
            if (File.Exists(path)) File.Delete(path);
        }

        public bool Exists(string fileName)
        {
            return File.Exists(GetFullPath(fileName));
        }
    }
}
