using Microsoft.Hadoop.Avro.Container;
using System;
using System.Linq;
using System.IO;
using Microsoft.Hadoop.Avro;

namespace Serialization
{
    public class AvroSerialization : IBinarySerializer
    {
        public byte[] Serialize<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                AvroSerializer.Create<T>().Serialize(stream, obj);
                return stream.GetBuffer();
            }
        }
        public T Deserialize<T>(byte[] obj)
        {
            using (MemoryStream stream = new MemoryStream(obj))
            {
                return  AvroSerializer.Create<T>().Deserialize(stream);
            }
        }
    }
}
