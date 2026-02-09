namespace _02.Scripts.Managers.Save
{
    /// <summary>
    /// 데이터 저장 및 불러오기를 위한 시스템 인터페이스
    /// </summary>
    public interface ISaveSystem
    {
        void Save<T>(string fileName, T data);
        T Load<T>(string fileName);
        void Delete(string fileName);
        bool Exists(string fileName);
    }
}
