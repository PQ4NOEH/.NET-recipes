using System;
using System.IO;
using Thrift.Protocol;
using Thrift.Transport;
using Xunit;

namespace Serialization.UnitTests
{
    public class ThriftSpec
    {
        public class ThriftSerializer
        {
            public byte[] Serialize<T>(T obj) where T : TBase
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using(TTransport transport = new TStreamTransport(stream, stream))
                    {
                        TProtocol protocol = new TBinaryProtocol(transport);
                        obj.Write(protocol);
                    }
                    return stream.GetBuffer();
                }
            }
            public T Deserialize<T>(byte[] obj) where T : TBase, new()
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (TTransport transport = new TStreamTransport(stream, stream))
                    {
                        TProtocol protocol = new TBinaryProtocol(transport);
                        T t = new T();
                        t.Read(protocol);
                        return t;
                    }
                }
            }

        }
        public class BinaryJsonSpec
        {
            [Fact]
            public void Serializates_deserialiates()
            {
                var data = GenerateRandom();
                var serializedData = new ThriftSerializer().Serialize<ThriftDataStructure>(data);
                var deserialized = new ThriftSerializer().Deserialize<ThriftDataStructure>(serializedData);
                Assert.Equal(data.OtherProperty, deserialized.OtherProperty);
                Assert.Equal(data.MessageId, deserialized.MessageId);
                Assert.Equal(data.Topic, deserialized.Topic);
                Assert.Equal(data.MessageType, deserialized.MessageType);
                Assert.Equal(data.Aggregate, deserialized.Aggregate);
                Assert.Equal(data.MessageOriginator, deserialized.MessageOriginator);
                Assert.Equal(data.CreatedTime, deserialized.CreatedTime);
            }

            [Fact]
            public void Duck_typing()
            {
                var data = GenerateRandom();
                var serializedData = new ThriftSerializer().Serialize<ThriftDataStructure>(data);
                var deserialized = new ThriftSerializer().Deserialize<ThriftDataStructure>(serializedData);
                Assert.Equal(data.MessageId, deserialized.MessageId);
                Assert.Equal(data.Topic, deserialized.Topic);
                Assert.Equal(data.MessageType, deserialized.MessageType);
                Assert.Equal(data.Aggregate, deserialized.Aggregate);
                Assert.Equal(data.CreatedTime, deserialized.CreatedTime);
                Assert.Equal(data.MessageOriginator, deserialized.MessageOriginator);
            }


            ThriftDataStructure GenerateRandom()
            {
                return new ThriftDataStructure
                {
                    MessageId = Guid.NewGuid(),
                    Topic = StringExtensions.RandomString(),
                    MessageOriginator = StringExtensions.RandomString(),
                    Aggregate = System.Text.ASCIIEncoding.UTF8.GetBytes(StringExtensions.RandomString(40)),
                    MessageType = StringExtensions.RandomString(),
                    CreatedTime = DateTimeOffset.Now,
                    OtherProperty = StringExtensions.RandomString()
                };
            }
        }

        public class ThriftBaseDataStructure
        {

            public Guid MessageId { get; set; }

            public string Topic { get; set; }

            public string MessageType { get; set; }

            public string MessageOriginator { get; set; }

            public byte[] Aggregate { get; set; }

            public DateTimeOffset CreatedTime { get; set; }
        }

        public class ThriftDataStructure : ThriftBaseDataStructure
        {
            public string OtherProperty { get; set; }
        }
    }
}
