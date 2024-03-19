using System.Collections.Generic;

namespace Assets.Scripts.SaveLoad
{
    public interface ISaver
    {
        void PersistSave(GameData data, string name, bool overwrite = true);
        GameData LoadSave(string name);
        void DeleteSave(string name);
        IEnumerable<string> ListSaves();
    }
}