using Newtonsoft.Json;
using System.Text;

namespace Serialization
{
    public class BinaryJsonSerializer : IBinarySerializer
    {
        public byte[] Serialize<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public T Deserialize<T>(byte[] obj)
        {
            var txt = Encoding.UTF8.GetString(obj);
            return JsonConvert.DeserializeObject<T>(txt);
        }
    }
}
