using Microsoft.Hadoop.Avro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Serialization.UnitTests
{
    [DataContract(Name = "BaseMessage")]
    public class BaseMessage
    {
        [DataMember(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember(Name = "Topic")]
        public string Topic { get; set; }
    }
    [DataContract(Name = "MyMessage")]
    public class MyMessage : BaseMessage
    {
        [DataMember(Name = "Aggregate", IsRequired=false)]
        public byte[] Aggregate { get; set; }
    }

    public class AvroSPec
    {
        public void Serializates_deseriazates()
        {
            BaseMessage actual = null;
            var expected = new MyMessage
            {
                Id = Guid.NewGuid(),
                Topic = StringExtensions.RandomString(),
                Aggregate = Encoding.UTF8.GetBytes(StringExtensions.RandomString())//random
            };
            byte[] SerilizedStream = null;
            using (MemoryStream stream = new MemoryStream())
            {
                AvroSerializer.Create<MyMessage>().Serialize(stream, expected);
                SerilizedStream = stream.GetBuffer();
            }
            using (MemoryStream stream = new MemoryStream(SerilizedStream))
            {
                actual = AvroSerializer.Create<MyMessage>().Deserialize(stream);
            }
            Assert.Equal(actual.Id, expected.Id);
            Assert.Equal(actual.Topic, expected.Topic);
        }
        [Fact]
        public void I_class_can_be_serializated_an_then_deserializated_to_its_base_type()
        {
            BaseMessage actual = null;
            var expected = new MyMessage
            {
                Id = Guid.NewGuid(),
                Topic = StringExtensions.RandomString(),
                Aggregate = Encoding.UTF8.GetBytes(StringExtensions.RandomString())//random
            };
            byte[] SerilizedStream = null;
            using (MemoryStream stream = new MemoryStream())
            {
                AvroSerializer.Create<MyMessage>().Serialize(stream, expected);
                SerilizedStream = stream.GetBuffer();
            }
            using (MemoryStream stream = new MemoryStream(SerilizedStream))
            {
                actual = AvroSerializer.Create<BaseMessage>().Deserialize(stream);
            }
            Assert.Equal(actual.Id, expected.Id);
            Assert.Equal(actual.Topic, expected.Topic);
        }
    }
}
