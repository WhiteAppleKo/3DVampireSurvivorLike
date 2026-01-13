namespace _02.Scripts.Managers.Save
{
    public interface ISaveable
    {
        public void RegistSavedata()
        {
            SaveManager.Instance.RegistSaveData(this);
        }
    }
}
