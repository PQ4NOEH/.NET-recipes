using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Serialization.UnitTests
{
    public class BinaryJsonSerializer
    {
        public byte[] Serialize(object obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }
        public T Deserialize<T>(byte[] obj)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(obj));
        }

    }
    public class BinaryJsonSpec
    {
        [Fact]
        public void Serializates_deserialiates()
        {
            var data = GenerateRandom();
            var serializedData = new BinaryJsonSerializer().Serialize(data);
            var deserialized = new BinaryJsonSerializer().Deserialize<JsonDataStructure>(serializedData);
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
            var serializedData = new BinaryJsonSerializer().Serialize(data);
            var deserialized = new BinaryJsonSerializer().Deserialize<JsonBaseDataStructure>(serializedData);
            Assert.Equal(data.MessageId, deserialized.MessageId);
            Assert.Equal(data.Topic, deserialized.Topic);
            Assert.Equal(data.MessageType, deserialized.MessageType);
            Assert.Equal(data.Aggregate, deserialized.Aggregate);
            Assert.Equal(data.CreatedTime, deserialized.CreatedTime);
            Assert.Equal(data.MessageOriginator, deserialized.MessageOriginator);
        }

        
        JsonDataStructure GenerateRandom()
        {
            return new JsonDataStructure
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

    public class JsonBaseDataStructure
    {
        
        public Guid MessageId { get; set; }
        
        public string Topic { get; set; }
        
        public string MessageType { get; set; }
        
        public string MessageOriginator { get; set; }
        
        public byte[] Aggregate { get; set; }
        
        public DateTimeOffset CreatedTime { get; set; }
    }

    public class JsonDataStructure :JsonBaseDataStructure
    {
        public string OtherProperty { get; set; }
    }
}
