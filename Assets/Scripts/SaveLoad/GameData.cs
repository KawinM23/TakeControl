using Assets.Scripts.Combat;
using AYellowpaper.SerializedCollections;
using UnityEngine;

#nullable enable

namespace Assets.Scripts.SaveLoad
{
    [System.Serializable]
    public class GameData
    {

        public string name = "";
        public string currentScene = "";
        public SerializedDictionary<string, bool> visited = new SerializedDictionary<string, bool>(); // Key=MapName
        public SerializedDictionary<string, bool> switches = new SerializedDictionary<string, bool>(); // Key=ID, Value=IsOn
        public SerializedDictionary<string, bool> fightArenas = new SerializedDictionary<string, bool>();
        public SerializedDictionary<string, bool> collectTrials = new SerializedDictionary<string, bool>();
        public SerializedDictionary<string, bool> puzzles = new SerializedDictionary<string, bool>(); // Key=SceneName, Value=IsSolved
        public int currency;
        public ModulesData modules = new ModulesData();


        public void BeforePersist()
        {
            modules = new ModulesData();
        }
    }


}