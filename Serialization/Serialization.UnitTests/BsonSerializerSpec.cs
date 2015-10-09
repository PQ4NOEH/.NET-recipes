using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Serialization.UnitTests
{
    public class BsonSerializerSpec
    {
        [Fact]
        public void Serializa_deserializa()
        {
            var expected = GenerateOtherRandom();
            var serialized = expected.ToBson();
            var actual = BsonSerializer.Deserialize<BsonOtherClass>(serialized);
            Assert.Equal(expected.CreatedTime, actual.CreatedTime);
            Assert.Equal(expected.MessageId, actual.MessageId);
            Assert.Equal(expected.Topic, actual.Topic);
            Assert.Equal(expected.MessageType, actual.MessageType);
            Assert.Equal(expected.Aggregate, actual.Aggregate);
            Assert.Equal(expected.MessageOriginator, actual.MessageOriginator);
        }
        [Fact]
        public void Serializa_deserializa_al_tipo_base()
        {
            var expected = GenerateOtherRandom();
            var serialized = expected.ToBson();
            var actual = BsonSerializer.Deserialize<BsonBaseDataStructure>(serialized);
            Assert.Equal(expected.CreatedTime, actual.CreatedTime);
            Assert.Equal(expected.MessageId, actual.MessageId);
            Assert.Equal(expected.Topic, actual.Topic);
            Assert.Equal(expected.MessageType, actual.MessageType);
            Assert.Equal(expected.MessageOriginator, actual.MessageOriginator);
        }

        BsonOtherClass GenerateOtherRandom()
        {
            return new BsonOtherClass
            {
                MessageId = Guid.NewGuid(),
                Topic = StringExtensions.RandomString(),
                MessageOriginator = StringExtensions.RandomString(),
                Aggregate = System.Text.ASCIIEncoding.UTF8.GetBytes(StringExtensions.RandomString(40)),
                MessageType = StringExtensions.RandomString(),
                CreatedTime = DateTimeOffset.Now
            };
        }
        public class BsonBaseDataStructure
        {
            
            public Guid MessageId { get; set; }
            
            public string Topic { get; set; }
            
            public string MessageType { get; set; }
            
            public string MessageOriginator { get; set; }

            
            public DateTimeOffset CreatedTime { get; set; }
        }
        public class BsonOtherClass : BsonBaseDataStructure
        {
            public byte[] Aggregate { get; set; }
        }
    }
}
