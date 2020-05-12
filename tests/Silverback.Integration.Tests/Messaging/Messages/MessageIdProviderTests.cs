// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System.Linq;
using FluentAssertions;
using Silverback.Messaging.Messages;
using Xunit;

namespace Silverback.Tests.Integration.Messaging.Messages
{
    public class MessageIdProviderTests
    {
        [Fact]
        public void EnsureMessageIdIsInitialized_NoHeaderSet_HeaderInitialized()
        {
            var message = new object();
            var headers = new MessageHeaderCollection();

            MessageIdProvider.EnsureMessageIdIsInitialized(message, headers);

            headers.Count.Should().Be(1);
            headers.First().Key.Should().Be("x-message-id");
            headers.First().Value.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void EnsureMessageIdIsInitialized_IdHeaderAlreadySet_HeaderPreserved()
        {
            var message = new object();
            var headers = new MessageHeaderCollection
            {
                { "x-message-id", "12345" }
            };

            MessageIdProvider.EnsureMessageIdIsInitialized(message, headers);

            headers.Should().BeEquivalentTo(new MessageHeader("x-message-id", "12345"));
        }
    }
}