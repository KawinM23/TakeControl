namespace Assets.Scripts.SaveLoad
{
    /// <summary>
    /// Interface for objects that can save and load data 
    /// to persist between scenes transitions and game sessions.
    /// </summary>
    public interface IDataPersist
    {
        /// <summary>
        /// Load the state from the `GameData`.
        /// </summary>
        void LoadData(in GameData data);

        /// <summary>
        /// Save the state to the `GameData` for future `LoadData` calls.
        /// </summary>
        void SaveData(ref GameData data);
    }
}