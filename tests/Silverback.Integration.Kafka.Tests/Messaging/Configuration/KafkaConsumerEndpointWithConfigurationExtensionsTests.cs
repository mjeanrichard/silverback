// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using FluentAssertions;
using Silverback.Messaging;
using Silverback.Messaging.Configuration;
using Xunit;

namespace Silverback.Tests.Integration.Kafka.Messaging.Configuration
{
    public class KafkaConsumerEndpointWithConfigurationExtensionsTests
    {
        [Fact]
        public void WithConfiguration_Configuration_ConfigurationSet()
        {
            var endpoint = new KafkaConsumerEndpoint("test");

            endpoint.WithConfiguration(
                new KafkaConsumerConfig
                {
                    BootstrapServers = "PLAINTEXT://whatever:1111",
                    GroupId = "group1"
                });

            endpoint.Configuration.Should().NotBeNull();
            endpoint.Configuration.BootstrapServers.Should().Be("PLAINTEXT://whatever:1111");
            endpoint.Configuration.GroupId.Should().Be("group1");
        }

        [Fact]
        public void WithConfiguration_ConfigurationAction_ConfigurationSet()
        {
            var endpoint = new KafkaConsumerEndpoint("test");

            endpoint.WithConfiguration(
                config =>
                {
                    config.BootstrapServers = "PLAINTEXT://whatever:1111";
                    config.GroupId = "group1";
                });

            endpoint.Configuration.Should().NotBeNull();
            endpoint.Configuration.BootstrapServers.Should().Be("PLAINTEXT://whatever:1111");
            endpoint.Configuration.GroupId.Should().Be("group1");
        }
    }
}
