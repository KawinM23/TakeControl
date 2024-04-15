using AYellowpaper.SerializedCollections;

namespace Assets.Scripts.SaveLoad
{
    [System.Serializable]
    public class GameData
    {
        public string name;
        public string currentScene;
        public SerializedDictionary<string, bool> switches = new SerializedDictionary<string, bool>();
        public int currency;
    }
}