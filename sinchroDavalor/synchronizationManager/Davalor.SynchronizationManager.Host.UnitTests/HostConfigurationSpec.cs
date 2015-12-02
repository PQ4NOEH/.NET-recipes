using Davalor.SynchronizationManager.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Davalor.SynchronizationManager.Host.UnitTests
{
    public class HostConfigurationSpec
    {
        [Fact]
        public void It_can_load_the_kafkaConfiguration()
        {
            var config = new HostConfiguration();
            Assert.NotNull(config.kafkaConfiguration);
            Assert.Equal(config.kafkaConfiguration.Address[0], "http://webmail.contigotic.com/");
            Assert.Equal(config.kafkaConfiguration.Address[1], "https://www.google.es/");
            Assert.Equal(config.kafkaConfiguration.Uri[0], new Uri(config.kafkaConfiguration.Address[0]));
            Assert.Equal(config.kafkaConfiguration.Uri[1], new Uri(config.kafkaConfiguration.Address[1]));
            Assert.Equal(config.kafkaConfiguration.ConsumerGroup, "aConsumer");
            Assert.Equal(config.kafkaConfiguration.TopicsToListen[0], "one");
            Assert.Equal(config.kafkaConfiguration.TopicsToListen[1], "two");
            Assert.Equal(config.kafkaConfiguration.TopicsToListen[2], "all");
        }

        [Fact]
        public void It_can_load_the_connectionStrings()
        {
            var config = new HostConfiguration();
            Assert.NotNull(config.kafkaConfiguration);
            Assert.Equal(config.PortalPacienteConnectionString, "PortalPacienteConnectionString_string");
            Assert.Equal(config.VisionLocalConnectionString, "VisionLocalConnectionString_string");
        }

        [Fact]
        public void It_can_load_the_MessagesHandlers()
        {
            var config = new HostConfiguration();
            Assert.Equal(config.MessagesHandlers.Count, 3);

            Assert.Equal(config.MessagesHandlers.First().MessageHandlerName, "firstMessageHandler");
            Assert.Equal(config.MessagesHandlers.First().SystemToSynchronize.Count, 2);
            Assert.True(config.MessagesHandlers.First().SystemToSynchronize.Any(s => s == ESynchroSystem.VisionLocal));
            Assert.True(config.MessagesHandlers.First().SystemToSynchronize.Any(s => s == ESynchroSystem.PortalPaciente));

            Assert.Equal(config.MessagesHandlers.Skip(1).First().MessageHandlerName, "secondMessageHandler");
            Assert.Equal(config.MessagesHandlers.Skip(1).First().SystemToSynchronize.Count, 1);
            Assert.Equal(config.MessagesHandlers.Skip(1).First().SystemToSynchronize.First(), ESynchroSystem.VisionLocal);

            Assert.Equal(config.MessagesHandlers.Last().MessageHandlerName, "theLast");
            Assert.Equal(config.MessagesHandlers.Last().SystemToSynchronize.Count, 0);
        }
    }
}
