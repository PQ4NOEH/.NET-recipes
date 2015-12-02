
using Davalor.Base.Contract.Library;
using Davalor.Base.Library.Guards;
using Davalor.Base.Library.Serialization;
using System.ComponentModel.Composition;

namespace Davalor.SynchronizationManager.Host
{
    /// <summary>
    /// Service for binary serialization/deserialization. It is a wrapper of Davalor.Framework.Core.Serialization.BinaryJsonSerializer
    /// </summary>
    [Export(typeof(IBinarySerializer))]
    public class BinarySerializerService : IBinarySerializer
    {
        readonly IBinarySerializer _adaptee;

        [ImportingConstructor]
        public BinarySerializerService()
        {
            _adaptee = new BinaryJsonSerializer();
        }
        public T Deserialize<T>(NotNullable<byte[]> obj)
        {
            return _adaptee.Deserialize<T>(obj);
        }

        public void PopulateObject<T>(NotNullable<byte[]> raw, NotNullable<T> obj)
        {
            _adaptee.PopulateObject<T>(raw, obj);
        }

        public byte[] Serialize<T>(NotNullable<T> obj)
        {
            return _adaptee.Serialize<T>(obj);
        }

        public TResult Deserialize<TResult>(byte[] obj)
        {
            return _adaptee.Deserialize<TResult>(obj);
        }

        public void PopulateObject<TObject>(byte[] raw, TObject obj)
        {
            _adaptee.PopulateObject<TObject>(raw, obj);
        }

        public byte[] Serialize<TObject>(TObject obj)
        {
            return _adaptee.Serialize<TObject>(obj);
        }
    }
}
