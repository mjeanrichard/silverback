// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using FluentAssertions;
using Silverback.Messaging;
using Silverback.Messaging.Configuration;
using Xunit;

namespace Silverback.Tests.Integration.Kafka.Messaging.Configuration
{
    public class KafkaConsumerEndpointParallelismExtensionsTests
    {
        [Fact]
        public void ProcessPartitionsIndependently_ProcessPartitionsIndependentlySet()
        {
            var endpoint = new KafkaConsumerEndpoint("test");

            endpoint.ProcessPartitionsIndependently();

            endpoint.ProcessPartitionsIndependently.Should().BeTrue();
        }

        [Fact]
        public void ProcessAllPartitionsTogether_ProcessPartitionsIndependentlySet()
        {
            var endpoint = new KafkaConsumerEndpoint("test");

            endpoint.ProcessAllPartitionsTogether();

            endpoint.ProcessPartitionsIndependently.Should().BeTrue();
        }

        [Fact]
        public void LimitParallelism_MaxDegreeOfParallelismSet()
        {
            var endpoint = new KafkaConsumerEndpoint("test");

            endpoint.LimitParallelism(42);

            endpoint.MaxDegreeOfParallelism.Should().Be(42);
        }

        [Fact]
        public void LimitBackpressure_BackpressureLimitSet()
        {
            var endpoint = new KafkaConsumerEndpoint("test");

            endpoint.LimitBackpressure(42);

            endpoint.BackpressureLimit.Should().Be(42);
        }
    }
}
