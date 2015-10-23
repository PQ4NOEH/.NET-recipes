
using System;
namespace TheMenu.Core
{
    public interface IBinarySerializer
    {
        byte[] Serialize<T>(T data);
        byte[] Serialize(object data);
        T Deserialize<T>(byte[] data);
        object Deserialize(Type objType, byte[] data);
    }
}
