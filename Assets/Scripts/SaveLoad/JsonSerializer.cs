using System.IO;
using UnityEngine;

namespace Assets.Scripts.SaveLoad
{
    public class JsonSerializer : ISerializer
    {
        private bool pretty;

        public JsonSerializer(bool pretty = true)
        {
            this.pretty = pretty;
        }

        public void Serialize<T>(Stream writer, T obj)
        {
            var json = JsonUtility.ToJson(obj, pretty);
            using (var w = new StreamWriter(writer))
            {
                w.Write(json);
                w.Flush();
            }
        }

        public T Deserialize<T>(Stream reader)
        {
            using (var r = new StreamReader(reader))
            {
                var data = r.ReadToEnd();
                return JsonUtility.FromJson<T>(data);
            }
        }

        public static string SerializeString(string data)
        {
            return JsonUtility.ToJson(data);
        }
    }
}