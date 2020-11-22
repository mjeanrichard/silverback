// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using FluentAssertions;
using Silverback.Messaging.Configuration;
using Silverback.Messaging.Serialization;
using Silverback.Tests.Integration.TestTypes.Domain;
using Silverback.Tests.Types;
using Xunit;

namespace Silverback.Tests.Integration.Messaging.Configuration
{
    public class ProducerEndpointSerializeAsJsonExtensionsTests
    {
        [Fact]
        public void SerializeAsJson_Default_SerializerSet()
        {
            var endpoint = new TestProducerEndpoint("test");

            endpoint.SerializeAsJson();

            endpoint.Serializer.Should().BeOfType<JsonMessageSerializer>();
            endpoint.Serializer.Should().NotBeSameAs(JsonMessageSerializer.Default);
        }

        [Fact]
        public void SerializeAsJson_WithConfig_SerializerAndOptionsSet()
        {
            var endpoint = new TestProducerEndpoint("test");

            endpoint.SerializeAsJson(serializer => { serializer.Options.MaxDepth = 42; });

            endpoint.Serializer.Should().BeOfType<JsonMessageSerializer>();
            endpoint.Serializer.As<JsonMessageSerializer>().Options.MaxDepth.Should().Be(42);
        }

        [Fact]
        public void SerializeAsJson_WithTypeParameter_SerializerSet()
        {
            var endpoint = new TestProducerEndpoint("test");

            endpoint.SerializeAsJson<TestEventOne>();

            endpoint.Serializer.Should().BeOfType<JsonMessageSerializer<TestEventOne>>();
        }

        [Fact]
        public void SerializeAsJson_WithTypeParameterAndConfig_SerializerAndOptionsSet()
        {
            var endpoint = new TestProducerEndpoint("test");

            endpoint.SerializeAsJson<TestEventOne>(serializer => { serializer.Options.MaxDepth = 42; });

            endpoint.Serializer.Should().BeOfType<JsonMessageSerializer<TestEventOne>>();
            endpoint.Serializer.As<JsonMessageSerializer<TestEventOne>>().Options.MaxDepth.Should().Be(42);
        }
    }
}
