namespace _02.Scripts.Managers.Save
{
    public interface ISaveable
    {
        void SaveData();
        void LoadData();

        public void RegistSaveAble()
        {
            SaveManager.Instance.RegistSaveData(this);
        }
    }
}
