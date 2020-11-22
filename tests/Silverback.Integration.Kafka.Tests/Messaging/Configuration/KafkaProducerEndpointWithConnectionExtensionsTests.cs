// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using FluentAssertions;
using Silverback.Messaging;
using Silverback.Messaging.Configuration;
using Xunit;

namespace Silverback.Tests.Integration.Kafka.Messaging.Configuration
{
    public class KafkaProducerEndpointWithConnectionExtensionsTests
    {
        [Fact]
        public void WithConfiguration_Configuration_ConfigurationSet()
        {
            var endpoint = new KafkaProducerEndpoint("test");

            endpoint.WithConfiguration(
                new KafkaProducerConfig
                {
                    BootstrapServers = "PLAINTEXT://whatever:1111",
                    MaxInFlight = 42
                });

            endpoint.Configuration.Should().NotBeNull();
            endpoint.Configuration.BootstrapServers.Should().Be("PLAINTEXT://whatever:1111");
            endpoint.Configuration.MaxInFlight.Should().Be(42);
        }

        [Fact]
        public void WithConfiguration_ConfigurationAction_ConfigurationSet()
        {
            var endpoint = new KafkaProducerEndpoint("test");

            endpoint.WithConfiguration(
                config =>
                {
                    config.BootstrapServers = "PLAINTEXT://whatever:1111";
                    config.MaxInFlight = 42;
                });

            endpoint.Configuration.Should().NotBeNull();
            endpoint.Configuration.BootstrapServers.Should().Be("PLAINTEXT://whatever:1111");
            endpoint.Configuration.MaxInFlight.Should().Be(42);
        }
    }
}
