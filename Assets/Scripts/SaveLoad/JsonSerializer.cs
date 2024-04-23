using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Assets.Scripts.SaveLoad
{
    public class JsonSerializer : ISerializer
    {
        private JsonSerializerSettings settings;

        public JsonSerializer(bool pretty = true)
        {
            settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public void Serialize<T>(Stream writer, T obj)
        {
            var json = JsonConvert.SerializeObject(obj, settings);
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
                return JsonConvert.DeserializeObject<T>(data, settings);
            }
        }

        public static string SerializeString(string data)
        {
            return JsonUtility.ToJson(data);
        }
    }
}