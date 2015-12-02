
using Davalor.Base.Contract.Library;
using Davalor.Base.Library.Guards;
using Davalor.Base.Library.Serialization;
using System.ComponentModel.Composition;

namespace Davalor.SynchronizationManager.Host
{
    /// <summary>
    /// Service for string serialization/deserialization. It is a wrapper of Davalor.Framework.Core.Serialization.JsonSerializer
    /// </summary>
    [Export(typeof(IStringSerializer))]
    public class StringSerializerService : IStringSerializer
    {
        readonly IStringSerializer _adaptee;

        [ImportingConstructor]
        public StringSerializerService()
        {
            _adaptee = new JsonSerializer();
        }
        public T Deserialize<T>(NotNullOrWhiteSpaceString obj)
        {
            return _adaptee.Deserialize<T>(obj);
        }

        public void PopulateObject<T>(NotNullOrWhiteSpaceString raw, T obj)
        {
            _adaptee.PopulateObject<T>(raw, obj);
        }

        public string Serialize<T>(NotNullable<T> obj)
        {
            return _adaptee.Serialize<T>(obj);
        }

        public TResult Deserialize<TResult>(string obj)
        {
            return _adaptee.Deserialize<TResult>(obj);
        }

        public void PopulateObject<TObject>(string raw, TObject obj)
        {
            _adaptee.PopulateObject<TObject>(raw, obj);
        }

        public string Serialize<TObject>(TObject obj)
        {
            return _adaptee.Serialize<TObject>(obj);
        }
    }
}
