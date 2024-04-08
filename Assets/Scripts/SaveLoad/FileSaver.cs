using System.IO;
using UnityEngine;

namespace Assets.Scripts.SaveLoad
{
    public class FileSaver : ISaver
    {
        private readonly ISerializer serializer;
        private readonly string _dirPath;
        private readonly string _fileExt;

        public FileSaver(ISerializer serializer, string path, string fileExt)
        {
            this.serializer = serializer;
            this._dirPath = path;
            this._fileExt = fileExt;
        }

        public void PersistSave(GameData data, string name, bool overwrite = true)
        {
            string path = SavePath(name);
            Directory.CreateDirectory(_dirPath);

            if (!overwrite && File.Exists(path))
            {
                throw new IOException($"File {path} should not already exists when overwrite is disabled");
            }

            using (var stream = new FileStream(path, overwrite ? FileMode.Create : FileMode.CreateNew))
            {
                Debug.Log($"Saving {data} to {path}");
                serializer.Serialize(stream, data);
            }

            Debug.Log($"Game saved to {path}");
        }

        public GameData LoadSave(string name)
        {
            string path = SavePath(name);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Save file at {path} should exist to be loaded");
            }

            Debug.Log($"Game loaded from {path}");

            using (var stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize<GameData>(stream);
            }

        }

        public void DeleteSave(string name)
        {
            string path = SavePath(name);
            File.Delete(path);
            Debug.Log($"Game deleted from {path}");
        }

        public System.Collections.Generic.IEnumerable<string> ListSaves()
        {
            return Directory.EnumerateFiles(_dirPath, $"*{_fileExt}");
        }

        private string SavePath(string name)
        {
            string path = Path.Combine(_dirPath, name);
            path = Path.ChangeExtension(path, _fileExt);
            return path;
        }
    }
}