namespace Assets.Scripts.SaveLoad
{
    public interface IDataPersist
    {
        void LoadData(in GameData data);
        void SaveData(ref GameData data);
    }
}