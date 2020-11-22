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
    public class ConsumerEndpointDeserializeJsonExtensionsTests
    {
        [Fact]
        public void DeserializeJson_Default_SerializerSet()
        {
            var endpoint = new TestConsumerEndpoint("test");

            endpoint.DeserializeJson();

            endpoint.Serializer.Should().BeOfType<JsonMessageSerializer>();
            endpoint.Serializer.Should().NotBeSameAs(JsonMessageSerializer.Default);
        }

        [Fact]
        public void DeserializeJson_WithConfig_SerializerAndOptionsSet()
        {
            var endpoint = new TestConsumerEndpoint("test");

            endpoint.DeserializeJson(serializer => { serializer.Options.MaxDepth = 42; });

            endpoint.Serializer.Should().BeOfType<JsonMessageSerializer>();
            endpoint.Serializer.As<JsonMessageSerializer>().Options.MaxDepth.Should().Be(42);
        }

        [Fact]
        public void DeserializeJson_WithTypeParameter_SerializerSet()
        {
            var endpoint = new TestConsumerEndpoint("test");

            endpoint.DeserializeJson<TestEventOne>();

            endpoint.Serializer.Should().BeOfType<JsonMessageSerializer<TestEventOne>>();
        }

        [Fact]
        public void DeserializeJson_WithTypeParameterAndConfig_SerializerAndOptionsSet()
        {
            var endpoint = new TestConsumerEndpoint("test");

            endpoint.DeserializeJson<TestEventOne>(serializer => { serializer.Options.MaxDepth = 42; });

            endpoint.Serializer.Should().BeOfType<JsonMessageSerializer<TestEventOne>>();
            endpoint.Serializer.As<JsonMessageSerializer<TestEventOne>>().Options.MaxDepth.Should().Be(42);
        }
    }
}
