// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using FluentAssertions;
using Silverback.Messaging.Configuration;
using Silverback.Tests.Types;
using Xunit;

namespace Silverback.Tests.Integration.Messaging.Configuration
{
    public class ConsumerEndpointThrowIfUnhandledExtensionsTests
    {
        [Fact]
        public void ThrowIfUnhandled_ThrowIfUnhandledSet()
        {
            var endpoint = new TestConsumerEndpoint("test");

            endpoint.ThrowIfUnhandled();

            endpoint.ThrowIfUnhandled.Should().Be(true);
        }

        [Fact]
        public void IgnoreUnhandledMessages_ThrowIfUnhandledSet()
        {
            var endpoint = new TestConsumerEndpoint("test");

            endpoint.IgnoreUnhandledMessages();

            endpoint.ThrowIfUnhandled.Should().Be(false);
        }
    }
}
