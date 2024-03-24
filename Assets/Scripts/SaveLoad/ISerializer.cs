using System.IO;

namespace Assets.Scripts.SaveLoad
{
    public interface ISerializer
    {
        void Serialize<T>(Stream writer, T obj);
        T Deserialize<T>(Stream reader);
    }
}