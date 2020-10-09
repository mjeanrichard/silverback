﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.Collections.Generic;
using Silverback.Messaging.Broker;

namespace Silverback.Messaging.Messages
{
    /// <summary>
    ///     Represent an envelope that has been already processed by the behaviors. It is used to avoid processing
    ///     again the messages that have been stored in the outbox.
    /// </summary>
    internal class ProcessedOutboundEnvelope : OutboundEnvelope
    {
        public ProcessedOutboundEnvelope(
            byte[]? message,
            IEnumerable<MessageHeader>? headers,
            IProducerEndpoint endpoint)
            : base(message, headers, endpoint)
        {
        }
    }
}